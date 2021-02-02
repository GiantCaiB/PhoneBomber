using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PhoneMaNew.Configs;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.Types;

namespace PhoneMaNew.Services
{
    public class Bomber : IBomber
    {
        private readonly ILogger<Bomber> _logger;
        private readonly BomberConfig _bomberConfig;

        public Bomber(ILogger<Bomber> logger, IAppConfig appConfig)
        {
            _logger = logger;
            _bomberConfig = appConfig.BomberConfig;
        }

        public async Task Bomb(PhoneNumber target)
        {
            try
            {
                var response = new VoiceResponse();
                response.Say(_bomberConfig.BombContent);
                var sortieIndex = 1;

                while (sortieIndex <= _bomberConfig.TotalSorties)
                {
                    var selectedIndex = new Random().Next(_bomberConfig.OwnedPhoneNumbers.Count);
                    var from = new PhoneNumber(_bomberConfig.OwnedPhoneNumbers.ToList()[selectedIndex]);
                    var call = await CallResource.CreateAsync(target, from,twiml: response.ToString());
                    _logger.LogInformation($"{sortieIndex}/{_bomberConfig.TotalSorties} sortie(s): Call has been made: {call.Sid}");
                    sortieIndex++;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred when bomb.");
            }
        }
    }
}
