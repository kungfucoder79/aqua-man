using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace OrderingApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {

            IConfigurationRoot config = new ConfigurationBuilder()
                    .AddCommandLine(args)
                    .Build();

            IWebHost  host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
