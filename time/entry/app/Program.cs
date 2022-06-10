using Microsoft.AspNetCore;

namespace App
{
    public class Program
    {
        public static string SharedoUrl{ get; private set; }
        public static string IdentityUrl{ get; private set; }
        public static string ClientId{ get; private set; }
        public static string ClientSecret{ get; private set; }

        public static async Task Main(string[] args)
        {
            if( !ValidateAndSetConfig(args) ) return;

            var host = WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

            await host.RunAsync();
        }

        private static string GetArg(string[] args, string @switch)
        {
            var index = Array.IndexOf(args, @switch);
            if( index == -1 ) return null;

            index++;

            if( index > (args.Length -1)) return null;

            return args[index];
        }
        
        private static bool ValidateAndSetConfig(string[] args)
        {
            SharedoUrl = GetArg(args,  "--sharedo");
            IdentityUrl = GetArg(args, "--identity");
            ClientId = GetArg(args, "--client");
            ClientSecret = GetArg(args, "--secret");

            var ok = true;

            if( string.IsNullOrWhiteSpace(SharedoUrl))
            {
                Console.WriteLine("Specify the Sharedo instance to use with --sharedo https://yoururl");
                ok = false;
            }

            if( string.IsNullOrWhiteSpace(IdentityUrl))
            {
                Console.WriteLine("Specify the Identity instance to use with --identity http://yoururl");
                ok = false;
            }

            if( string.IsNullOrWhiteSpace(ClientId))
            {
                Console.WriteLine("Specify the client id to connect as with --client YOURID");
                ok = false;
            }

            if( string.IsNullOrWhiteSpace(ClientSecret))
            {
                Console.WriteLine("Specify the client secret with --secret YOURSECRET");
                ok = false;
            }

            return ok;
        }
    }
}