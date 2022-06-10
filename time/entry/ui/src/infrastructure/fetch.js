import router from "./router";

const ContentTypes = 
{
    JSON: "application/json"
};

function defaultHandleErrors(err, reject)
{
    // Detect 401 unauthorised response as we will want to redirect
    if( err && err.status === 401 )
    {
        reject("Not authorised - redirecting to login");

        let returnUrl = `${window.location.pathname}${window.location.search}`;
        router.push({ name: "login", params: { nextUrl: returnUrl }});

        return;
    }

    if( !err || !err.text || typeof(err.text) !== "function")
    {
        console.error("Something went wrong", err);
        reject("Something went wrong - check console");
    }

    err.text().then(txt =>
    {
        if( err.status >= 500)
        {
            console.error("Server error", txt);
            reject("Server error");
        }

        if( txt )
        {
            reject(txt);
            return;
        }
        else
        {
            switch(err.status)
            {
                case 403:
                    reject("Forbidden");
                    return;
                case 404:
                    reject("Not found");
                    return;
            }
                
            reject(`Something went wrong ${err.status}`);    
        }    
    });
}

function doFetch(method, url, body, handleErrors)
{
    if( handleErrors === undefined ) handleErrors = true;

    var encodedBody = null;
    var headers = {};
    
    if( method === "PUT" || method === "POST" || method === "DELETE" )
    {
        encodedBody = JSON.stringify(body);
        headers["Content-Type"] = ContentTypes.JSON;
    }

    headers["Accept"] = ContentTypes.JSON;

    var request =
    {
        method: method,
        cache: "no-cache",
        body: encodedBody,
        headers: headers,
    }

    return new Promise((resolve, reject) =>
    {
        fetch(url, request).then
        (
            response =>
            {
                if( !response.ok )
                {
                    if( handleErrors ) defaultHandleErrors(response, reject);
                    else reject(response);
                    
                    return;
                }

                var type = response.headers.get("content-type");
                if( type && type.indexOf("application/json") > -1 )
                {
                    resolve(response.json());
                    return;
                }

                resolve(null);
            }
        );
    });
}

const fetchClient =
{
    doFetch: doFetch,
    get: (url) => doFetch("GET", url, null),
    post: (url, body) => doFetch("POST", url, body),
    put: (url, body) => doFetch("PUT", url, body),
    delete: (url, body) => doFetch("DELETE", url, body)
};

export default fetchClient;


// Remove this
window.fetchClient = fetchClient;