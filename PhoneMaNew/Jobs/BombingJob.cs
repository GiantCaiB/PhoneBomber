using Quartz;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PhoneMaNew.Configs;
using PhoneMaNew.Services;
using Twilio.Types;

namespace PhoneMaNew.Jobs
{
    public class BombingJob : IJob
    {
        private readonly IBomber _bomber;
        private readonly ILogger<BombingJob> _logger;
        private readonly IAppConfig _appConfig;

        public BombingJob(IBomber bomber,
            ILogger<BombingJob> logger,
            IAppConfig appConfig)
        {
            _bomber = bomber;
            _logger = logger;
            _appConfig = appConfig;
        }

        public Task Execute(IJobExecutionContext context)
        {
            Parallel.ForEach(_appConfig.TargetPhoneNumbers, target =>
            {
                var targetNumber = new PhoneNumber(target);
                _bomber.Bomb(targetNumber);
            });
            _logger.LogInformation("BombingJob Executed.");
            return Task.CompletedTask;
        }
    }
}
