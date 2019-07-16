using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Myconfig
{
    static class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }
        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.development.json")
                    .Build();
        }
    }
}