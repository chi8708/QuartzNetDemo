
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

            //使用3：每分钟的第1,10,14,25,35,50秒执行一次。
            //那么上面显然是不能满足的。这是我就把cron-like（由7段构成：秒 分 时 日 月 星期 年（可选））表达式引入进来，以实现各种时间纬度的调用
            //"-" ：表示范围  MON-WED表示星期一到星期三
            // "," ：表示列举 MON, WEB表示星期一和星期三
            //"*" ：表是“每”，每月，每天，每周，每年等
            //"/" :表示增量：0 / 15（处于分钟段里面） 每15分钟，在0分以后开始，3 / 20 每20分钟，从3分钟以后开始
            //"?" :只能出现在日，星期段里面，表示不指定具体的值
            //"L" :只能出现在日，星期段里面，是Last的缩写，一个月的最后一天，一个星期的最后一天（星期六）
            //"W" :表示工作日，距离给定值最近的工作日
            //"#" :表示一个月的第几个星期几，例如："6#3"表示每个月的第三个星期五（1 = SUN...6 = FRI,7 = SAT）
            DateTimeOffset endTime = DateBuilder.NextGivenSecondDate(DateTime.Now.AddYears(2), 3);
            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create().StartAt(startTime).EndAt(endTime)
                                        .WithCronSchedule("1,10,14,25,35,50 * * * * ? ")
                                        .Build();
            //4.加入作业调度池中
            sched.ScheduleJob(job, trigger);

            //加入第二个作业 1个job只能绑定在1个trigger
            IJobDetail job2 = JobBuilder.Create<JobDemo2>().Build();
            ISimpleTrigger trigger2 = (ISimpleTrigger)TriggerBuilder.Create().
                WithSimpleSchedule(x => x.WithIntervalInSeconds(3).WithRepeatCount(2)).Build();
            sched.ScheduleJob(job2, trigger2);

            //5.开始运行
            sched.Start();
            ////挂起3分钟
            //Thread.Sleep(TimeSpan.FromMinutes(3));
            ////3分钟后关闭作业调度，将不在执行
            //sched.Shutdown();

            //1分钟后移除trigger2
            Thread.Sleep(TimeSpan.FromMinutes(1));
            sched.UnscheduleJob(trigger2.Key);//移除trigger2

            Console.ReadKey();
            //参考https://www.cnblogs.com/jys509/p/4628926.html

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
            return Task.Run(() => Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")));
        }
    }


    public class JobDemo2 : IJob
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
            return Task.Run(() => Console.WriteLine($"Job2 {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}"));
        }
    }
}
