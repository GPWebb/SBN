using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace SBN
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                //.UseKestrel()         //TODO sort
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}
