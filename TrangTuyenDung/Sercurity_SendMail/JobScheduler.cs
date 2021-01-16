using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz.Impl;


namespace TrangTuyenDung.Sercurity_SendMail {
    public class JobScheduler {

        public static void Start() {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();


            
        IJobDetail job = JobBuilder.Create<EmailJob>().Build();



            ITrigger trigger = TriggerBuilder.Create()

                .WithDailyTimeIntervalSchedule

                  (s =>

                     s.WithIntervalInHours(24)

                    .OnEveryDay()

                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(15, 15))

                  )

                .Build();



            scheduler.ScheduleJob(job, trigger);

        }

    }
}