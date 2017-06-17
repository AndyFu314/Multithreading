﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            Task<string> t1 = GetInfoAsync("Task 1", 3);
            Task<string> t2 = GetInfoAsync("Task 2", 5);

            string[] results = await Task.WhenAll(t1, t2);
            foreach (var result in results)
            {
                WriteLine(result);
            }
        }

        private static async Task<string> GetInfoAsync(string name, int seconds)
        {
            // the same worker thread
            await Task.Delay(TimeSpan.FromSeconds(seconds));

            // different worker threads
            //await Task.Run(() =>
            //    Thread.Sleep(TimeSpan.FromSeconds(seconds)));

            return
                $"Task {name} is running on a thread id {CurrentThread.ManagedThreadId}" +
                $" Is thread pool thread: {CurrentThread.IsThreadPoolThread}";
        }
    }
}
