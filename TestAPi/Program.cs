﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TestAPi
{
    public class Program
    {
        public static IWebHostBuilder WebHostBuilder{ get; private set; }
        public static void Main(string[] args)
        {
            WebHostBuilder = CreateWebHostBuilder(args);
            WebHostBuilder.Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://127.0.0.1:5000")
                .UseStartup<Startup>();
    }
}
