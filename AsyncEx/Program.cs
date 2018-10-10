using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncEx
{
    class Program
    {
        static void Main()
        {
            //由于不能跨进程访问 所以只能通过Wait、WhenAll、 Await 来监控异常 
            // Test1();
            // Test2();
            //Test3();
            Test4();
            Console.ReadKey();
        }
        static void Test1()
        {
            try
            {
                var t = Task.Run(() => TestEx());
                t.Wait();
            }
            catch (AggregateException ag)
            {
                foreach (var item in ag.InnerExceptions)
                {
                    Console.WriteLine(item.Message);
                }
            }


        }
        static void Test2()
        {
            var t = Task.Run(() => TestEx())
                .ContinueWith(p =>
                {
                    if (p.Exception != null)
                    {
                        Console.WriteLine(p.Exception.InnerException.Message);
                    }
                });
        }

        static void Test3()
        {
            try
            {
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < 5; i++)
                {
                    var t = Task.Run(() => TestEx());
                    tasks.Add(t);
                }

                // Task.WhenAll(tasks);//WhenAll 会创建一个线程，WaitAll不创建会阻塞当前线程，所以WhenAll无法在catch中捕获异常
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ag)
            {
                foreach (var item in ag.InnerExceptions)
                {
                    Console.WriteLine(item.Message);
                }
            }
        }


        static void Test4()
        {

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                var t = Task.Run(() => TestEx());
                tasks.Add(t);
            }
            Task.WhenAll(tasks)
                .ContinueWith(p =>
                {
                    if (p.Exception != null)
                    {
                        foreach (var item in p.Exception.InnerExceptions)
                        {
                            Console.WriteLine(item.Message);
                        }
                    }
                });

        }
        static void TestEx()
        {
            throw new Exception($"异常啦{DateTime.Now}");
        }
    }
}
