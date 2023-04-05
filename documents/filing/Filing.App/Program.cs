using System.Security.Claims;
using Filing.App;
using Filing.App.Auth;
using Filing.App.Sharedo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

var authSettingsSection = builder.Configuration.GetSection("SharedoIdentity");
var authSettings = authSettingsSection?.Get<SharedoAuthSettings>() ??
    throw new InvalidOperationException("Missing settings 'SharedoIdentity'");

// Add services to the container.
builder.Services
    .AddRazorPages()
    .AddRazorRuntimeCompilation();

builder.Services
    .Configure<SharedoAuthSettings>(authSettingsSection);

// http client for authentication of sharedo
builder.Services
    .AddHttpClient(Clients.SharedoIdentity, client =>
    {
        client.BaseAddress = new Uri(authSettings.Url);
    });

// http client for sharedo api
builder.Services
    .AddHttpClient(Clients.SharedoApi, client =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetRequiredValue<string>("SharedoApi:Url"));
        client.DefaultRequestHeaders.Add("accept", "application/json");
    })
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services
    .AddAuthentication(a =>
    {
        a.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        a.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(o =>
    {
        o.LoginPath = "/login";
        o.LogoutPath = "/logout";

        o.Events = new CookieAuthenticationEvents
        {
            // this is called after the auth cookie has been validated.  Check if access_token is close to expiring and refresh if necessary.
            OnValidatePrincipal = async x =>
            {
                var signOut = async () =>
                {
                    x.RejectPrincipal();
                    await x.HttpContext.SignOutAsync();
                };

                var expiresAt = x.Properties.GetTokenValue("expires_at");

                if (!DateTimeOffset.TryParse(expiresAt, null, System.Globalization.DateTimeStyles.AssumeUniversal, out var accessTokenExpiration))
                {
                    await signOut();
                    return;
                }

                var timeRemaining = accessTokenExpiration.Subtract(DateTimeOffset.UtcNow);

                // any tokens that expire in next 5 minutes will be refreshed.
                var refreshThreshold = TimeSpan.FromMinutes(5);

                if (timeRemaining < refreshThreshold)
                {
                    var identity = (ClaimsIdentity)x.Principal!.Identity!;

                    var refreshToken = x.Properties.GetTokenValue("refresh_token");
                    if(string.IsNullOrEmpty(refreshToken))
                    {
                        await signOut();
                        return;
                    }

                    var tokenRefresher = x.HttpContext.RequestServices.GetRequiredService<ITokenRefesher>();
                    var token = await tokenRefresher.Refresh(refreshToken);

                    if (token.IsError)
                    {
                        await signOut();
                        return;
                    }

                    x.Properties.UpdateTokenValue("access_token", token.AccessToken);
                    x.Properties.UpdateTokenValue("refresh_token", token.RefreshToken);
                    x.Properties.UpdateTokenValue("expires_at", token.ExpiresOn.ToString());

                    // renew cookie since principal has been modified
                    x.ShouldRenew = true;
                }
            }
        };
    })
    .AddOpenIdConnect(o =>
    {

        o.Authority = authSettings.Url;
        o.ClientId = authSettings.ClientId;
        o.ClientSecret = authSettings.ClientSecret;

        // Require sharedo API access
        o.Scope.Add("sharedo");

        // Get a refresh token so we can renew the access token
        o.Scope.Add("offline_access");

        o.ResponseType = OpenIdConnectResponseType.Code;
        o.ResponseMode = OpenIdConnectResponseMode.FormPost;

        o.SaveTokens = true;
        o.CallbackPath = "/signin-oidc";

        // Sharedo doesn't support PKCE yet
        o.UsePkce = false;
    });

builder.Services
    .AddScoped<AuthHeaderHandler>()
    .AddScoped<ITokenRefesher, TokenRefresher>()
    .AddScoped<IDocumentManagement, DocumentManagement>()
    .AddScoped<IMyWorkItems, MyWorkItems>()
    .AddScoped<IWorkItemSearches, WorkItemSearches>();

builder.Services
    .AddHttpContextAccessor()
    .AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
