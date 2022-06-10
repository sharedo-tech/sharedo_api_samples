import api from "./fetch";
import authState from "./authState"

export function init()
{
    return new Promise((resolve) =>
    {
        api.get("/api/profile").then(
            response =>
            {
                authState.isAuthenticated = true;
                authState.username = response.user;
                resolve();
            },
            err =>
            {
                authState.isAuthenticated = false;
                authState.username = null;
                resolve();
            }
        );
    });
}

