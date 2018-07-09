
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace _01Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString("r"));
            //1.首先创建一个作业调度池
            ISchedulerFactory schedf = new StdSchedulerFactory();
            IScheduler sched = schedf.GetScheduler().Result;
            //2.创建出来一个具体的作业
            IJobDetail job = JobBuilder.Create<JobDemo>().Build();
            //3.创建并配置一个触发器
            //ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create().WithSimpleSchedule(x => x.WithIntervalInSeconds(3).WithRepeatCount(3)).Build();

            //NextGivenSecondDate：如果第一个参数为null则表名当前时间往后推迟2秒的时间点。
            //使用2：假如没执行完1分钟后不执行
            DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(DateTime.Now.AddSeconds(1), 2);
            //DateTimeOffset endTime = DateBuilder.NextGivenSecondDate(DateTime.Now.AddMinutes(1), 3);
            //ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create().StartAt(startTime).EndAt(endTime).
            //    WithSimpleSchedule(x => x.WithIntervalInSeconds(3).WithRepeatCount(100)).Build();

            //使用3：每分钟的第1，10，14,15秒执行一次。
            //那么上面显然是不能满足的。这是我就把cron-like表达式引入进来，以实现各种时间纬度的调用
            DateTimeOffset endTime = DateBuilder.NextGivenSecondDate(DateTime.Now.AddYears(2),3);
            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().StartAt(startTime).EndAt(endTime)
                                        .WithCronSchedule("1,10,14,15 * * * * ? ")
                                        .Build();
            //4.加入作业调度池中
            sched.ScheduleJob(job, trigger);

            //5.开始运行
            sched.Start();

            //挂起3分钟
            Thread.Sleep(TimeSpan.FromMinutes(3));
            //3分钟后关闭作业调度，将不在执行
            sched.Shutdown();

            Console.ReadKey();

        }
    }

    public class JobDemo : IJob
    {
        /// <summary>
        /// 这里是作业调度每次定时执行方法
        /// </summary>
        /// <param name="context"></param>
        //public void Execute(IJobExecutionContext context)
        //{
        //    Console.WriteLine(DateTime.Now.ToString("r"));
        //}

        Task IJob.Execute(IJobExecutionContext context)
        {
           return Task.Run(()=>Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")));
        }
    }
}
