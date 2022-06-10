# Providing a time capture UI

This sample demonstrates using the time API to record time against a work item in Sharedo. It shows how to resolve the data capture required to successfully post time, display this to the user to complete, and then use the time API to then submit time.

As a result it also demonstrates the use of the OAuth authorisation code flow to login to the app using sharedo and to obtain tokens that allow the API to be called.

## Running the sample
In your sharedo instance, create a new oAuth client using the authorisation code flow. If you're running this sample locally, add a reply url of https://localhost:7100/api/oidc

You can then run the sample from the command line

`npm run dev`

`dotnet watch run -- --sharedo https://your-sharedo-url --identity https://your-identity-url --client yourclientapp --secret yourclientsecret`

## Building the sample as a stand alone
`npm run build`

`dotnet run --launch-profile prod -- --sharedo https://your-sharedo-url --identity https://your-identity-url --client yourclientapp --secret yourclientsecret`

