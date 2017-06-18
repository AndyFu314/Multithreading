using System;
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
            // scenario 1
            WriteLine("1. Single exception");

            try
            {
                string result = await GetInfoAsync("Task 1", 2);
                WriteLine(result);
            }
            catch (Exception ex)
            {
                WriteLine($"Exception details: {ex}");
            }

            // scenario 2
            WriteLine();
            WriteLine("2. Multiple exceptions");

            Task<string> t1 = GetInfoAsync("Task 1", 3);
            Task<string> t2 = GetInfoAsync("Task 2", 5);
            try
            {
                string[] results = await Task.WhenAll(t1, t2);
                WriteLine(results.Length);
            }
            catch (Exception ex)
            {
                WriteLine($"Exception details: {ex}");
            }

            // scenario 3
            WriteLine();
            WriteLine("3. Multiple exceptions with AggregateException");

            t1 = GetInfoAsync("Task 1", 3);
            t2 = GetInfoAsync("Task 2", 2);
            Task<string[]> t3 = Task.WhenAll(t1, t2);
            try
            {
                string[] results = await t3;
                WriteLine(results.Length);
            }
            catch
            {
                var ae = t3.Exception.Flatten();
                var exceptions = ae.InnerExceptions;
                WriteLine($"Exceptions caught: {exceptions.Count}");
                foreach (var ex in exceptions)
                {
                    WriteLine($"Exception details: {ex}");
                    WriteLine();
                }
            }

            // scenario 4
            WriteLine();
            WriteLine("4. await in catch an finally blocks");

            try
            {
                string result = await GetInfoAsync("Task 1", 2);
                WriteLine(result);
            }
            catch (Exception ex)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                WriteLine($"Catch block with await: Exception details: {ex}");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                WriteLine("Finally block");
            }
        }

        private static async Task<string> GetInfoAsync(string name, int seconds)
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
            throw new Exception($"Boom from {name}!");
        }
    }
}
