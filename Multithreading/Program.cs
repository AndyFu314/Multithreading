using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

            ReadLine();
        }
        
        static async Task AsynchronousProcessing()
        {
            string result = await GetDynamicAwaitableObject(true);
            WriteLine(result);

            result = await GetDynamicAwaitableObject(false);
            WriteLine(result);
        }

        private static dynamic GetDynamicAwaitableObject(bool completeSynchronously)
        {
            dynamic result = new ExpandoObject();
            dynamic awaiter = new ExpandoObject();

            awaiter.Message = "Completed synchronously";
            awaiter.IsCompleted = completeSynchronously;
            awaiter.GetResult = (Func<string>)(() => awaiter.Message);

            awaiter.OnCompleted = (Action<Action>)(
                callback => ThreadPool.QueueUserWorkItem(
                    state =>
                    {
                        Sleep(TimeSpan.FromSeconds(1));
                        awaiter.Message = GetInfo();
                        callback?.Invoke();
                    }));

            IAwaiter<string> proxy = Impromptu.ActLike(awaiter);

            result.GetAwaiter = (Func<dynamic>)(() => proxy);

            return result;
        }

        static string GetInfo()
        {
            return
                $"Task is running on a thread id {CurrentThread.ManagedThreadId}." +
                $" Is thread pool thread: {CurrentThread.IsThreadPoolThread}";
        }
    }

    public interface IAwaiter<T> : INotifyCompletion
    {
        bool IsCompleted { get; }

        T GetResult();
    }
}
