using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WWOTelemetry;
using WWOTelemetry.Model;
using WWOTelemetryAgent.Facades;
using WWOTelemetryAgent.Metrics;

namespace WWOTelemetryAgent
{
    internal class Program
    {
        private static List<BaseMetric> metrics = new List<BaseMetric>();
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private static void Main(string[] args)
        {
            HandlerRoutine hr = new HandlerRoutine(ConsoleCtrlCheck);
            GC.KeepAlive(hr);
            SetConsoleCtrlHandler(hr, true);

            InitializedInfos();

            var datasourceKey = File.ReadAllText("key.txt");

            var client = new WWOClient(datasourceKey);

            foreach (var tag in metrics.Select(metric => metric.ToTag()))
            {
                client.CreateTag(tag);
            }

            var stopWatch = Stopwatch.StartNew();

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                foreach (var metric in metrics)
                {
                    client.Track(metric.ToTag().TagName, metric.GetValue());
                }

                var waitTime = (int)(1000 - stopWatch.ElapsedMilliseconds);
                Thread.Sleep(waitTime);
                stopWatch.Restart();
            }
            Console.WriteLine("DONE");
        }

        private static void InitializedInfos()
        {
            metrics.Add(new BaseMetric("Cpu", "%", new PerformanceCounterMetric("Processor", "% Processor Time", "_Total")));
            metrics.Add(new BaseMetric("Ram.Free", "Bytes", new PerformanceCounterMetric("Memory", "Available Bytes")));
            metrics.Add(new BaseMetric("Disk", "%", new PerformanceCounterMetric("PhysicalDisk", "% Disk Time", "_Total")));
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            cancellationTokenSource.Cancel();
            return true;
        }

        #region unmanaged

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion unmanaged
    }
}