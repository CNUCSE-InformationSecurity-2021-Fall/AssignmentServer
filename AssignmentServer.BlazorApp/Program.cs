using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp
{
    public class Program
    {
        public readonly static string ApiBaseUrl;
        public readonly static string JwtSecret;

        static Program()
        {
            ApiBaseUrl = "https://localhost:5001";

            using var rng = RandomNumberGenerator.Create();
            
            var bytes = new byte[64];
            rng.GetNonZeroBytes(bytes);

            JwtSecret = Convert.ToBase64String(bytes);
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
