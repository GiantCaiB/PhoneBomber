using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PhoneMaNew.Configs
{
    public class AppConfig : IAppConfig
    {
        public TwilioCredentials TwilioCredentials { get; }

        public BomberConfig BomberConfig { get; }

        public ICollection<string> TargetPhoneNumbers { get; set; }

        public string CronExpression { get; }

        public AppConfig(IConfigurationRoot configuration)
        {
            TwilioCredentials = configuration.GetSection("TwilioCredentials").Get<TwilioCredentials>();
            BomberConfig = configuration.GetSection("BomberConfig").Get<BomberConfig>();
            TargetPhoneNumbers = configuration.GetSection("TargetPhoneNumbers").Get<List<string>>();
            CronExpression = configuration.GetSection("CronExpression").Value;
        }
    }
}
