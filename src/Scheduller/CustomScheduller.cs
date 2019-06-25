using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WMIdataCollector.Job;

namespace WMIdataCollector.Scheduller
{
    internal class CustomScheduller : IJob
    {
        public static async void Start()
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            
            //IJobDetail job = JobBuilder.Create<CustomScheduller>().Build();
            IJobDetail job = JobBuilder.Create<CustomScheduller>()
                .WithIdentity("job_data_collector", "Collector").Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger_job_collector","group1")
                .StartNow()
                .WithSimpleSchedule(s => s.WithIntervalInSeconds(6) 
                                        .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
            await scheduler.Start();
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            WMIdataGetter getter = new WMIdataGetter();
            return Task.Run(() => {
                getter.WmiCollectorExecution();
            });
        }
    }
}
