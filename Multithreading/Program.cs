using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var sync = new CustomAwaitable(true);
            string result = await sync;
            WriteLine(result);

            var async = new CustomAwaitable(false);
            result = await async;
            WriteLine(result);
        }        

        private static async Task<string> GetInfoAsync(string name, int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            if (name.Contains("Exception"))
            {
                throw new Exception($"Boom from {name}!");
            }

            return
                $"Task {name} is running on a thread id {CurrentThread.ManagedThreadId}." +
                $" Is thread pool thread: {CurrentThread.IsThreadPoolThread}";
        }
    }

    class CustomAwaitable
    {
        private readonly bool completeSynchronously;

        public CustomAwaitable(bool completeSynchronously)
        {
            this.completeSynchronously = completeSynchronously;
        }

        public CustomAwaiter GetAwaiter()
        {
            return new CustomAwaiter(this.completeSynchronously);
        }
    }

    public class CustomAwaiter : INotifyCompletion
    {
        private string _result = "Completed synchronously";
        private readonly bool completeSynchronously;

        public bool IsCompleted => completeSynchronously;

        public CustomAwaiter(bool completeSynchronously)
        {
            this.completeSynchronously = completeSynchronously;
        }

        public string GetResult()
        {
            return _result;
        }

        public void OnCompleted(Action continuation)
        {
            ThreadPool.QueueUserWorkItem(
                state =>
                {
                    Sleep(TimeSpan.FromSeconds(1));
                    _result = GetInfo();
                    continuation?.Invoke();
                });
        }

        private string GetInfo()
        {
            return
                $"Task is running on a thread id {CurrentThread.ManagedThreadId}" +
                $" Is thread pool thread: {CurrentThread.IsThreadPoolThread}";
        }
    }
}
