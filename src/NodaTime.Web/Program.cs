using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace NodaTime.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                // Uncomment these lines if startup is failing
                //.CaptureStartupErrors(true)
                //.UseSetting("detailedErrors", "true")
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
