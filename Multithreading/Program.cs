using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using static System.Threading.Thread;

namespace Multithreading
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = AsynchronousProcessing();
            t.Wait();

            Console.ReadKey();
        }

        static async Task AsynchronousProcessing()
        {
            Func<string, Task<string>> asyncLambda = async name=>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                return
                    $"Task {name} is running on a thread id {CurrentThread.ManagedThreadId}."+
                    $" Is thread pool thread: {CurrentThread.IsThreadPoolThread}";
            };

            string result = await asyncLambda("async lambda");

            WriteLine(result);
        }
    }
}
