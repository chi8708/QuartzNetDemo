using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01Basic
{
    public class TriggerAndJobState
    {

        public static void Run()
        {
            ISchedulerFactory schedf = new StdSchedulerFactory();
            IScheduler sched = schedf.GetScheduler().Result;

            //IJobDetail job = JobBuilder.Create<JobDemo>().WithIdentity("job1","group1").Build();
            //job.JobDataMap.Put("k1", "test");

            //ISimpleTrigger trigger =(ISimpleTrigger)TriggerBuilder.Create()
            //    .WithSimpleSchedule(p=>p.WithIntervalInSeconds(3).WithRepeatCount(5)).Build();


            //IJobDetail job2 = JobBuilder.Create<JobDemo>().WithIdentity("job2", "group2").Build();
            //ICronTrigger trigger2 = (ICronTrigger)TriggerBuilder.Create()
            //  .StartAt(DateTime.Now).WithCronSchedule("1,10,14,25,35,50 * * * * ? ").Build();


            //Console.WriteLine($"trigerRC:{trigger.RepeatCount}trigerRInt:{trigger.RepeatInterval}");

            //sched.ScheduleJob(job, trigger);
            //sched.ScheduleJob(job2, trigger2);
         
            IJobDetail job = JobBuilder.Create<JobDemo3>().WithIdentity("job1", "group").Build();
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithSimpleSchedule(p => p.WithIntervalInSeconds(1).WithRepeatCount(3)).Build();
            sched.ScheduleJob(job, trigger);

            //StartAt(DateTime.Now.AddMilliseconds(500))// 防止重复调用
            IJobDetail job2 = JobBuilder.Create<JobDemo3>().WithIdentity("job2", "group").Build();
            ISimpleTrigger trigger2 = (ISimpleTrigger)TriggerBuilder.Create().StartAt(DateTime.Now.AddMilliseconds(500))
                .WithSimpleSchedule(p => p.WithIntervalInSeconds(1).WithRepeatCount(3)).Build();
            sched.ScheduleJob(job2, trigger2);

            sched.Start();
        }


        public class JobDemo3 : IJob
        {
            /// <summary>
            /// 这里是作业调度每次定时执行方法
            /// </summary>
            /// <param name="context"></param>
            //public void Execute(IJobExecutionContext context)
            //{
            //    Console.WriteLine(DateTime.Now.ToString("r"));
            //}
            //同一个job 即使被多个trigger使用 静态变量仅初始化一次
            static bool r = false;
            Task IJob.Execute(IJobExecutionContext context)
            {
                if (r)
                {
                    context.Scheduler.DeleteJobs(
                        new List<JobKey>() {
                        new JobKey("job2", "group"),
                        new JobKey("job1", "group") });
                    return Task.Run(() => Console.WriteLine("over"));

                }
                
                return Task.Run( () => 
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"));
                    r = true;
                });
            }
        }

    }
}
