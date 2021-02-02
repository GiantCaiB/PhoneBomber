using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PhoneMaNew.Jobs;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.Types;

namespace PhoneMaNew
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                var bombingJob = JobService.CreateHostBuilder(new string[] { })
                    .Build()
                    .Services.CreateScope()
                    .ServiceProvider.GetRequiredService<BombingJob>();
                bombingJob.Execute(null).Wait();
            }
            catch (Exception e)
            {
                File.WriteAllText("c:\\logFiles\\logFilesFatal.txt", e.ToString());
            }
        }
    }
}
