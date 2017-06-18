﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private static Label _label;

        [STAThread]
        static void Main(string[] args)
        {
            var app = new Application();
            var win = new Window();
            var panel = new StackPanel();
            var button = new Button();
            _label = new Label();
            _label.Height = 200;
            _label.FontSize = 32;
            button.Height = 100;
            button.FontSize = 32;
            button.Content = new TextBlock { Text = "Start asynchronous operations" };
            button.Click += Click;
            panel.Children.Add(_label);
            panel.Children.Add(button);
            win.Content = panel;
            app.Run(win);

            ReadLine();
        }

        static async void Click(object sender, EventArgs e)
        {
            _label.Content = new TextBlock { Text = "Calculating..." };
            TimeSpan resultWithContext = await Test();
            TimeSpan resultNoContext = await TestNoContext();
            //TimeSpan resultNoContext = await TestNoContext().ConfigureAwait(false);

            var sb = new StringBuilder();
            sb.AppendLine($"With the context: {resultWithContext}");
            sb.AppendLine($"Without the context: {resultNoContext}");
            sb.AppendLine("Ratio: " +
                $"{resultWithContext.TotalMilliseconds / resultNoContext.TotalMilliseconds:0.00}");
            _label.Content = new TextBlock { Text = sb.ToString() };
        }

        private static async Task<TimeSpan> Test()
        {
            const int iterationsNumber = 100000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterationsNumber; i++)
            {
                var t = Task.Run(() => { });
                await t;
            }
            sw.Stop();
            return sw.Elapsed;
        }

        private static async Task<TimeSpan> TestNoContext()
        {
            const int iterationsNumber = 100000;
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < iterationsNumber; i++)
            {
                var t = Task.Run(() => { });
                await t.ConfigureAwait(
                    continueOnCapturedContext: false);
            }
            sw.Stop();
            return sw.Elapsed;
        }
    }
}
