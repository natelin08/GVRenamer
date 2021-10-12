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

        public static NameRuleSetting NameRuleSetting
        {
            get
            {
                return Configuration.Value.GetSection("NameRuleSetting").Get<NameRuleSetting>();
            }
        }
    }

    public class GeneralSetting
    {
        public string SuccessOutputFolder { get; set; }
        public string FailedOutputFolder { get; set; }
        public bool FailedMove { get; set; }
        public string FileExtensions { get; set; }
        public bool SearchSubFolder { get; set; }
        public string EscapeFolder { get; set; }
    }

    public class NameRuleSetting
    {
        public string FolderRule { get; set; }
        public string NamingRule { get; set; }
        public int MaxTitleLength {  get; set; }
        public string MaxTitleOmitStr { get; set; }

    }
}
