using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using WWOTelemetry;
using WWOTelemetry.Model;
using WWOTelemetryAgent.Facades;
using WWOTelemetryAgent.Metrics;

namespace WWOTelemetryAgent
{
    internal class Program
    {
        private static List<BaseMetric> metrics = new List<BaseMetric>();

        private static void Main(string[] args)
        {
            var datasourceKey = File.ReadAllText("key.txt");

            InitializedInfos();
            var client = new WWOClient(datasourceKey);

            foreach (var tag in metrics.Select(metric => metric.ToTag()))
            {
                client.CreateTag(tag);
            }

            var stopWatch = Stopwatch.StartNew();
            while (true)
            {
                foreach (var metric in metrics)
                {
                    client.Track(metric.ToTag().TagName, metric.GetValue());
                }

                var waitTime = (int)(1000 - stopWatch.ElapsedMilliseconds);
                Thread.Sleep(waitTime);
                stopWatch.Restart();
            }
        }

        private static void InitializedInfos()
        {
            metrics.Add(new BaseMetric("Cpu", "%", new PerformanceCounterMetric("Processor", "% Processor Time", "_Total")));
            metrics.Add(new BaseMetric("Ram.Free", "Bytes", new PerformanceCounterMetric("Memory", "Available Bytes")));
            metrics.Add(new BaseMetric("Disk", "%", new PerformanceCounterMetric("PhysicalDisk", "% Disk Time", "_Total")));
        }
    }
}