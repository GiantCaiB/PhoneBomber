using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using PhoneMaNew.Configs;
using System.Configuration;
using NLog.Extensions.Logging;
using PhoneMaNew.Jobs;
using PhoneMaNew.Services;
using Quartz;
using Twilio;

namespace PhoneMaNew
{
    public class JobService : BackgroundService
    {
        private const string BombJobName = "Bombing";
        private readonly ILogger<JobService> _logger;
        private readonly string _cronExpression;
        private readonly IServiceProvider _serviceProvider;
        public JobService(IAppConfig appConfig,
            ILogger<JobService> logger,
            IServiceProvider serviceProvider
        )
        {
            _serviceProvider = serviceProvider;
            _cronExpression = appConfig.CronExpression;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                IConfigurationBuilder configurationBuilder = ConfigurationBuilder();
                IConfigurationRoot configuration = configurationBuilder.Build();
                ConfigureServices(services, configuration);
                services.AddHostedService<JobService>();
            });


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        private static IConfigurationBuilder ConfigurationBuilder()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
#if DEBUG
            {
                environment = "Development";
            }
#endif
            if (string.IsNullOrEmpty(environment))
            {
                throw new ConfigurationErrorsException("system environment variable [ASPNETCORE_ENVIRONMENT] not configured");
            }
            string assemblyPath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(assemblyPath);
            if (environment.Equals("Development", StringComparison.CurrentCultureIgnoreCase))
            {
                configurationBuilder.AddJsonFile("appsettings.json", false, true);
            }
            else
            {
                configurationBuilder.AddJsonFile($"appsettings.{environment}.json", false, true);
            }

            return configurationBuilder;
        }

        private static void ConfigureServices(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            var appConfig = new AppConfig(configuration);
            serviceCollection.AddSingleton<IAppConfig>(x => appConfig);
            // Configure NLog
            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(new NLogLoggingConfiguration(configuration.GetSection("NLog")));
            });

            // Initiate TwilioClient
            TwilioClient.Init(appConfig.TwilioCredentials.AccountSid, appConfig.TwilioCredentials.AuthToken);
            // DI
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton<BombingJob>();
            serviceCollection.AddTransient<IBomber, Bomber>();

            serviceCollection.AddSingleton<Func<string, IJob>>(s =>
            {
                IJob Func(string input)
                {
                    return input switch
                    {
                        BombJobName => s.GetRequiredService<BombingJob>(),
                        _ => throw new NotSupportedException($"unknown job name: {input}"),
                    };
                }
                return Func;
            });
        }
    }
}
