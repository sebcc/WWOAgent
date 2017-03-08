using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWOTelemetryAgent.Facades
{
    internal class PerformanceCounterMetric : IComputerInfo
    {
        private PerformanceCounter performanceCounter;

        public PerformanceCounterMetric(string category, string name, string instance = null)
        {
            if (!string.IsNullOrWhiteSpace(instance))
            {
                this.performanceCounter = new PerformanceCounter(category, name, instance);
            }
            else
            {
                this.performanceCounter = new PerformanceCounter(category, name);
            }
        }

        public object GetValue()
        {
            return this.performanceCounter.NextValue();
        }
    }
}