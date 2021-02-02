using System;
using System.Collections.Generic;
using System.Text;

namespace PhoneMaNew.Configs
{
    public interface IAppConfig
    {
        TwilioCredentials TwilioCredentials { get; }

        BomberConfig BomberConfig { get; }
        
        public ICollection<string> TargetPhoneNumbers { get; set; }
        string CronExpression { get; }
    }
}
