using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _01Basic
{
   public class AsyncTest
    {

        public static async void Excute()
        {
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " 开始 Excute " + DateTime.Now);
            await AsyncTest1();// await等待的是线程，不是函数。放回的是int
            var waitTask = AsyncTestRun();//返回的是Task<int>  waitTask并没有run 所以要Start
            waitTask.Start();
            int i = await waitTask;
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " 结束 Excute " + DateTime.Now);
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " 结束 Excute " + DateTime.Now + i);
        }


        public static async Task<int> AsyncTest1()
        {
            Task.Run(() =>
            {
                Console.WriteLine(Thread.CurrentThread.GetHashCode() + " Run1 " + DateTime.Now);
                Thread.Sleep(1000);
            });
            ////.GetAwaiter().GetResult() 会等待
            return 1;
        }

        public static Task<int> AsyncTestRun()
        {
            Task<int> t = new Task<int>(() => {
                Console.WriteLine(Thread.CurrentThread.GetHashCode() + " Run1 " + DateTime.Now);
                Thread.Sleep(1000);
                return 100;
            });

            return t;
        }

        public static async void Excute2()
        {
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " 开始 Excute " + DateTime.Now);
            await SingleAwait2();
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " 结束 Excute " + DateTime.Now);
        }

        public static async Task SingleAwait2()
        {
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " AwaitTest开始 " + DateTime.Now);
            await Task.Run(() =>
            {
                Console.WriteLine(Thread.CurrentThread.GetHashCode() + " Run1 " + DateTime.Now);
                Thread.Sleep(1000);
            });
            await Task.Run(() =>
            {
                Console.WriteLine(Thread.CurrentThread.GetHashCode() + " Run2 " + DateTime.Now);
                Thread.Sleep(1000);
            });
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " AwaitTest结束 " + DateTime.Now);
            return;
        }

        public static async void Excute3()
        {
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " 开始 Excute " + DateTime.Now);
            await SingleNoAwait3();
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " 结束 Excute " + DateTime.Now);

            //SingleNoAwait3第二组，线程重新回到了主线程1中，而第一组SingleAwait2，已经被优化到了线程4中。
        }
        public static async Task SingleNoAwait3()
        {
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " SingleNoAwait开始 " + DateTime.Now);
            Task.Run(() =>
            {
                Console.WriteLine(Thread.CurrentThread.GetHashCode() + " Run1 " + DateTime.Now);
                Thread.Sleep(1000);
            }).GetAwaiter().GetResult();
            Task.Run(() =>
            {
                Console.WriteLine(Thread.CurrentThread.GetHashCode() + " Run2 " + DateTime.Now);
                Thread.Sleep(1000);
            }).GetAwaiter().GetResult();
            Console.WriteLine(Thread.CurrentThread.GetHashCode() + " SingleNoAwait结束 " + DateTime.Now);
            return;
        }
    }
}
