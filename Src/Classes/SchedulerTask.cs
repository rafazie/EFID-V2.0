using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SimplifikasiFID.Classes
{
    public class SchedulerTask
    {


        public static async Task StartAsync()
        {
            try
            {

                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                await scheduler.Start();

                IJobDetail job = JobBuilder.Create<AutoApprove>()
                    .WithIdentity("MyJob1", "Group1")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    //.WithCronSchedule("0 0/5 * 1/1 * ? *")
                    //.WithCronSchedule("0/5 * * ? * *")
                    .WithCronSchedule("0 0/5 * 1/1 * ? *")
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}