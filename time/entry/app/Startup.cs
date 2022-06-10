using System.Text.Json.Serialization;
using App.Infrastructure;
using App.Sharedo;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace App
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure components for DI
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<IHttpTokenClient, HttpTokenClient>();
            services.AddScoped<IUserApi, UserApi>();
            services.AddScoped<IMyMattersApi, MyMattersApi>();
            services.AddScoped<ITimeApi, TimeApi>();

            services
                .AddHttpClient()
                .AddControllers(opts =>{ })
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services
                .AddAuthentication(a =>
                {
                    a.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    a.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    a.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(o =>
                {
                    o.Events = new CookieAuthenticationEvents()
                    {
                        OnRedirectToLogin = context =>
                        {
                            // Don't redirect - just 401 instead
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddOpenIdConnect(o =>
                {
                    o.Authority = Program.IdentityUrl;
                    o.ClientId = Program.ClientId;
                    o.ClientSecret = Program.ClientSecret;

                    // Require sharedo API access
                    o.Scope.Add("sharedo");

                    // Get a refresh token so we can renew the access token
                    o.Scope.Add("offline_access");
                    
                    o.ResponseType = OpenIdConnectResponseType.Code;
                    o.ResponseMode = OpenIdConnectResponseMode.FormPost;
                    o.SaveTokens = true;

                    // Redirect back to an API url so it doesn't deliver the SPA
                    o.CallbackPath = "/api/oidc";

                    // Sharedo doesn't support PKCE yet
                    o.UsePkce = false;
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            // NON API ROUTES - USE SPA and assets
            app.MapWhen(ctx => !ctx.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase), spaApp =>
            {
                if (env.IsDevelopment())
                {
                    spaApp.UseSpa(spa =>
                    {
                        spa.UseProxyToSpaDevelopmentServer("https://localhost:7101");
                    });
                }
                else
                {
                    spaApp.UseSpa(spa =>
                    {
                        spa.Options.SourcePath = "wwwroot";
                        spa.Options.DefaultPage = new PathString("/index.html");
                    });
                }
            });

            // API ROUTES - Use WebAPI
            app.MapWhen(ctx => ctx.Request.Path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase), apiApp =>
            {
                apiApp.UseRouting();

                apiApp.UseAuthentication();
                apiApp.UseAuthorization();

                apiApp.UseEndpoints(endpoints => endpoints.MapControllers());
            });
        }
    }


}