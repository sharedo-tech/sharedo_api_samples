# Providing a time capture UI

This sample demonstrates using the time API to record time against a work item in Sharedo. It shows how to resolve the data capture required to successfully post time, display this to the user to complete, and then use the time API to then submit time.

> Topics covered;
> - ASP.NET Core web app authenticating via sharedo and auth code flow
> - Using access and refresh tokens to interface to sharedo APIs
> - Back end for front end pattern - core app has it's own API that calls the sharedo APIs using the user's current access token (and refreshes the access token when it expires)
> - Leveraging the work item find by query API
> - Various aspects of the time API
>   - Getting available time categories to book to
>   - Getting the defined data capture for a time category + work item combination (the time capture classification)
>   - Sending a new time entry to sharedo

## Running the sample
In your sharedo instance, create a new oAuth client using the authorisation code flow. Set a robust secret, access tokens to 15 mins, refresh tokens to a few hours, and ensure "use reference tokens" is switched on to avoid sending full JWT tokens over the wire.

 If you're running this sample locally, add a reply url of https://localhost:7100/api/oidc

You can then run the sample from the command line.

### Start the vue.js client app
From a command line:
```ps
cd ui

npm run dev
```

This will start the vite dev server at https://localhost:7101. We'll be proxying the .net app to this during development. You'll need to open this URL in your browser to trust the vite SSL cert if you haven't already done this.

### Start the .net core app
Open another command line and run this:
```ps
cd app

dotnet watch run -- --sharedo https://yourSharedoUrl --identity https://yourIdentityUrl --client yourClientAppId --secret yourClientSecret`
```

Where `yourSharedoUrl` is the primary URL of your sharedo tenant, `yourIdentityUrl` is the identity URL of your sharedo tenant and `yourClientAppId` and `yourClientSecret` are the values you configured in the oAuth configuration screen when you set up a client for this sample in sharedo.

## Building the sample as a stand alone
If you want to simply build the vue.js app into the dotnet app and launch everything from there, you can do that too.

From a command line:
```ps
cd ui

npm run build

cd ..\app

dotnet run --launch-profile prod -- --sharedo https://yourSharedoUrl --identity https://yourIdentityUrl --client yourClientAppId --secret yourClientSecret`
```

That will build the vue.js SPA assets into the wwwroot directory of the dotnet app, and then start the dot net app in production mode, which does not proxy to vite, and instead delivers the assets we've just built.

## Application tour
### The premise
This small web app is an example of a quick time entry tool. It allows the user to select a case to book time to, from a list of the most recently updated ones, and then specify the time category and capture classificiation before choosing a quick time value (15 mins/30 mins/1 hour) to book to the case.

### Logging in
When you first start the app you will see this screen.

![](assets/pre-login.png?raw=true)

You could skip this if required quite easily - take a look at the `init()` method of `auth.js` - simply redirect to the login controller if it returns a `401 - Unauthorised`.

### Sign into sharedo
Clicking login will then redirect you to your sharedo instance, where you should see something similar to this (your instance may be different depending on how you've got authentication configured):

![](assets/login.png?raw=true)

Sign in with your user.

### Consent
If you've configured your client app in sharedo oAuth to require consent, you will then be asked to consent to the usage of the API by the sample app:

![](assets/consent.png?raw=true)

Notice the "Offline access" permission is requested. This allows the client to receive both an access token to call the APIs (and is usually configured to be short lived), and a refresh token (longer lived) to be able to request new access tokens. Without offline access, the access token will be valid for 15 minutes (if you've configured it as per the above) after which, the calls to the sharedo API will start to fail and you will be signed out of the sample app.

Choose "Yes, Allow" to proceed.

### Choose a case
The screen then calls the `findByQuery` API to find the 5x most recently updated cases in sharedo and lists them. Clicking any card will open a blade to record time against it, clicking the link "Open in sharedo" will open the case in the sharedo UI.

![](assets/list.png?raw=true)

### Book time
After choosing a case, the time entry blade will open allowing you to choose the time category to book time against, along with the data capture required for the category you have chosen for the work item you are booking against.

Time capture in sharedo is dynamic in that you can define different categories with different capture forms based on rules. Hence, the capture you see here could be very different as you book time to different cases.

![](assets/capture.png?raw=true)

Note here that I've chosen a matter and the billable category - in this case it resolves to capturing a "task/phase" code, an "activity" code and a memo field for comments. The task/phase code  is hierarchical (hence the 3 levels of drop down).

Once I've filled in the details on the capture that are mandatory (as defined by the configuration), I can then choose one of the time buttons to book 15 mins, 30 mins or 1 hour to this combination.

Once I've clicked the button, the time entry will be sent to sharedo via it's APIs and appear in the time list. The blade will automatically close after 3 seconds, allowing you to select another case to book more time to.

![](assets/done.png?raw=true)

The entry in sharedo:
![](assets/entry-in-sharedo.png?raw=true)

## Building a time entry form
The v2 time API no longer creates time entries based on a single code. This makes the creation of a time capture UI a little more cumbersome in that it needs to be dynamic based on the definition to be used for the time category and work item combination.

The construction of the capture UI will therefore require a number of things and several API calls.

1. A work item to book time against
2. A category to which time will be booked
3. A call to get the form definition for capturing time against 1 and 2
4. A call to the time entry API to record time

### (1) Getting a work item to book time against
You can use various mechanisms for this, the sample here allows the user to select a work item from a list to obtain the work item id.

The UI code is in `ui/src/modules/List.vue`, `ui/src/modules/agents/matterAgent.js` which calls the back end API at `app/Modules/MattersController.cs` and in turn uses `app/Sharedo/MyMattersApi.cs` to call the sharedo find by query API.

### (2) Getting the categories
Getting a list of categories is simple, a `GET` to the public API at `/api/v2/public/time/categories` will return a list of all the time categories available for recording. This is covered in the UI at `ui/src/modules/blades/AddTimeBlade.vue`, `ui/src/modules/agents/timeAgent.js` which calls the back end API at `app/Modules/TimeController.cs` which in turn uses `app/Sharedo/TimeApi.cs` to call the sharedo API to get the list of categories. These are then bound to a drop down list in the UI.

### (3) Getting the capture definition
Getting the capture definition is where we ask sharedo for what fields we should ask the user to fill in based on the rules configured for time capture for the given work item id and the category selected.

In the UI code, as the category is changed in `ui/src/modules/blades/AddTimeBlade.vue` you will see it then calls, via the agent, the back end api passing through the selected work item and category. This is passed up to the public API at `/api/v2/public/time/capture/{category}/{workItemId}` which returns the capture definition to use. This is passed back through to the UI SPA.

The key part of this response is the segments. Each segment represents one piece of data to be captured to record a time entry against this work item/category combination. The segments are returned in the order to capture them on screen, have flags indicating if they are mandatory or not and have a type in the field `captureType`

If the `captureType` for a segment is `Memo` then we simply capture a text input for the user to enter notes against this segment.

If however the `captureType` for the segment is `TimeCodeSet`, then it will contain a list of the time codes to be presented, as a hierarchical tree. In this sample, that hierarchy is rendered as nested drop down selections until a leaf node is selected, but it could easily be presented as a tree selection etc.

### (4) Saving the time entry
Once all the segments have been rendered to the screen, with mandatory validation based on the configuration, the rest of the time capture is relatively static. In our sample we simply allow a duration to be clicked to book the time, assuming it ends now, starts at end - amount of time to book.

The API to create an entry is a `POST` to `/api/v2/public/time/entry'. We pass in standard information for the time entry itself - the duration, start/end date/time (in UTC), the work item being booked against, and the category being used. The dynamic capture classification data is then sent along with this payload as an array of segment values which contain the resolved segment id, and the value entered (for a memo field) or time code selected (for a time code set field).

In the UI code, see `ui/src/modules/blades/AddTimeBlade.vue`, it's agent at `ui/src/modules/agents/timeAgent.js`. This calls the back end API in `app/Modules/TimeController.cs` which in turn uses `app/Sharedo/TimeApi.cs` to call the sharedo API to record the time. The UI model is kept very light and simple, and transformed to the more complex API request model in the BeFe tier.

## Code tour
The source to the .net web app is in the `app` folder. This is a simple .net web app, used as a back-end for the front-end of the app.

### Startup.cs
Calling out the important parts of the Startup.cs - this is where the authentication for the sample is configured to use sharedo as an Open Id Connect (OIDC) provider. Once authenticated via OIDC, the authorisation code flow will automatically happen to obtain the access and refresh tokens for your sharedo user, which are then stored in the authentication context (i.e. encrypted in the auth cookie for the sample app).

Cookie auth is then added to allow the successful OIDC authentication with sharedo to be stored in a cookie, so once you're signed in via sharedo, you are signed in to the sample for the duration of your session without having to go back to sharedo's login page.

The thing of note in the `AddCookie` call is the override of the `OnRedirectToLogin` - this is called where a call to the API fails and preserves the 401 response. The default behaviour is to redirect the user to the login url via a 301 response - this doesn't make sense in respect to an API call which, will follow a redirect, but would be pointless given it's response content would then be 200 (Ok) and contain the login page for sharedo!

```c#
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
```

The next part of `startup.cs` to call out is this:

```c#
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
```

We configure two different OWIN pipelines depending on the URL and whether we're running in dev mode or not. The first `MapWhen` will cover any URL that does not begin with `/api/`. This will, in development mode, proxy any requests for content/script/assets etc through to the vite server that you're running via `npm run dev` from the `ui` folder. This allows the vue.js app to be modified on the fly and hot reloaded in the application.

When it's not in development mode though, it serves static content from the `wwwroot` folder, and, where the url is not matched to a static file there, it returns the `index.html` page, which is the compiled vue.js SPA.

Lastly, where the url does start with `/api`, we're looking to call the API's we're exposing from controllers, so this path uses standard web api routing, authentication, authorization and maps the controller endpoints to the api.

### Controller and IxxxApi's
From there, the back end is a typical web api type application - controllers are defined, and those controllers map inbound requests onto a wrapper for the upstream sharedo API's, transforming front end requests to API requests and vice versa.

Taking `MattersController.cs` for example - this is typical of the pattern in use here;

```c#
[ApiController]
[Route("/api/matters")]
public class MattersController : ControllerBase
{
    private readonly IUserApi _userInfo;
    private readonly IMyMattersApi _matters;

    public MattersController(IUserApi userInfo, IMyMattersApi matters)
    {
        _userInfo = userInfo;
        _matters = matters;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMatterList()
    {
        var userId = await _userInfo.GetCurrentUserId();
        var matters = await _matters.GetUserMatters(userId);

        return new JsonResult(matters);
    }
}
```

The controller leverages two sharedo APIs - the profile API (encapsulated as `IUserApi`) and the find by query API (encapsulated as `IMyMattersApi`). Where the front end request `/api/matters` to get a list of work items it can book time to, the controller gets the current user's Id (which could have been better served by storing it as a claim during authentication), and then uses this to request the 5 most recently updated matters that the user has access to.

It then sends the resultant data back to the front end to render the list.

Nothing particularly special here. Indeed, there's nothing special in the API encapsulations either at first glance;

```c#
public class MyMattersApi : IMyMattersApi
{
    private readonly IHttpTokenClient _tokenClient;

    public MyMattersApi(IHttpTokenClient tokenClient)
    {
        _tokenClient = tokenClient;
    }

    public async Task<IEnumerable<MatterInfo>> GetUserMatters(Guid userId)
    {
        var request = new HttpRequestMessage
        (
            HttpMethod.Post, 
            $"{Program.SharedoUrl}/api/v1/public/workItem/findByQuery"
        );
        request.Headers.Add("accept", "application/json");

        request.Content = JsonContent.Create(new
        {
            Search = new
            {
                // << Some parameters snipped for brevity >>
                Ownership = new
                {
                    MyScope = new
                    {
                        OwnerIds = new[] { userId },
                        Primary = true,
                        Secondary = false
                    }
                }
            },
            Enrich = new []
            {
                new { Path = "reference" },
                new { Path = "title" }
            }
        });

        var response = await _tokenClient.SendWithTokensAsync(request);
        response.EnsureSuccessStatusCode();

        var queryResponse = await response.Content.ReadFromJsonAsync<FindByQueryResponse>();
        if (queryResponse == null) throw new InvalidOperationException();

        return queryResponse.Results.Select(item => new MatterInfo
        {
            Id = item.Id,
            Reference = item.Data.Reference,
            Title = item.Data.Title,
            Url = $"{Program.SharedoUrl}/sharedo/{item.Id}"
        });
    }
}
```

Quite simple, it sends a request to the sharedo public API at `api/v1/public/workItem/findByQuery`, passing through a number of search predicates. It then reads the response and transforms this into an array of structured results to use on the front end.

What is critical here though is the use of `IHttpTokenClient`, which you can find in the `app/infrastructure/HttpTokenClient.cs` file.

### HttpTokenClient
This is a wrapper around a `HttpClient` that can be used to make the upstream API calls to sharedo using the access and refresh tokens in the user's current authentication context. It will automatically add the access token as the HTTP `Authorization` header as a bearer token, and, if the call fails with a 401 (Unauthorised) status code, indicating the access token is no longer valid, it will automatically refresh it using the refresh token and store that back in the authentication context. If it fails to refresh at that point, the request fails and the user is signed out of the sample.

```c#
public class HttpTokenClient : IHttpTokenClient
{
    private readonly ITokenManager _tokens;
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<HttpTokenClient> _log;

    public HttpTokenClient(ITokenManager tokens, IHttpClientFactory clientFactory, ILogger<HttpTokenClient> log)
    {
        _tokens = tokens;
        _clientFactory = clientFactory;
        _log = log;
    }

    public async Task<HttpResponseMessage> SendWithTokensAsync(HttpRequestMessage request)
    {
        // Grab the access token from the current authentication context
        var accessToken = await _tokens.GetAccessTokenFromContextAsync();
        var hasAccessToken = !string.IsNullOrWhiteSpace(accessToken);
        if (!hasAccessToken)
        {
            _log.LogDebug("Call to {url} - we do not have an access token - trying call anyway", request.RequestUri);
        }

        // Make the API call attempt
        _log.LogDebug("Calling {url} with access token - attempt 1", request.RequestUri);
        var response = await Send(request, accessToken);

        // If we didn't have an access token, or response was anything other than a 401 = we're done
        if (!hasAccessToken || response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        // We got a 401 - AT might be expired or revoked
        _log.LogDebug("Call to {url} with current access token was unauthorised - starting refresh", request.RequestUri);

        // Request new tokens
        accessToken = await _tokens.GetNewAccessTokenAsync();
        hasAccessToken = !string.IsNullOrWhiteSpace(accessToken);
        if (!hasAccessToken)
        {
            // Couldn't renew - just return original response
            _log.LogDebug("Call to {url} couldn't get a new access token", request.RequestUri);
            return response;
        }

        // Try another attempt now we have a new access token and just return 
        // it's response, come what may....
        _log.LogDebug("Got new access token - retrying {url}", request.RequestUri);
        return await Send(CloneRequest(request), accessToken);
    }

    private Task<HttpResponseMessage> Send(HttpRequestMessage request, string accessToken)
    {
        request.Headers.Remove("Authorization");
        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        return _clientFactory.CreateClient().SendAsync(request);
    }

    private HttpRequestMessage CloneRequest(HttpRequestMessage original)
    {
        // Wish the API could just do this for us!
        var request = new HttpRequestMessage(original.Method, original.RequestUri);
        request.Content = request.Content;
        request.Headers.Clear();
        foreach (var h in request.Headers)
        {
            request.Headers.Add(h.Key, h.Value);
        }

        return request;
    }
}
```

The `HttpTokenClient` is responsible for using the tokens, understanding they need renewal, and so on, but to actually interface with the token API in the sharedo identity service, it uses the `TokenManager` class. 

### Signing out
When the user clicks the "sign out" button, you will note that is not an SPA url - it goes directly to `/api/logout`, which maps to the `Logout` method on the `AuthController`. This  does not have an `[Authorize]` attribute - meaning anyone can go to this url.

```c#
[HttpGet("logout")]
public async Task<IActionResult> Logout()
{
    if( User.Identity?.IsAuthenticated ?? false )
    {
        await _tokenManager.RevokeTokensAsync();
        await HttpContext.SignOutAsync();
    }

    return Redirect("/");
}
```

When this endpoint is hit, and the user is authenticated, it does two things. First of all it uses the `TokenManager` to send the access token reference and refresh token stored in the current authentication context back to the sharedo identity server to immediately revoke them. This is just an extra layer of security to avoid having tokens orphaned that are still valid for a short period of time.

It then calls the usual `SignOutAsync` which will clear the current authentication context in the cookie and finally redirects the user back to the SPA which should start the whole process off again.