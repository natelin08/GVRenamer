using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GVRenamer.Model
{
    public class AppSettingsModel
    {
        public static Lazy<IConfiguration> Configuration = new Lazy<IConfiguration>(() =>
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
        });

        public static GeneralSetting GeneralSetting
        {
            get
            {
                return Configuration.Value.GetSection("GeneralSetting").Get<GeneralSetting>();
            }
        }
    }

    public class GeneralSetting
    {
        public string SuccessOutputFolder { get; set; }
        public string FailedOutputFolder { get; set; }
        public string FileExtensions { get; set; }
    }
}
