using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWOTelemetry.Model;
using WWOTelemetryAgent.Facades;

namespace WWOTelemetryAgent.Metrics
{
    internal class BaseMetric
    {
        private string _name;
        private string _unit;

        private IComputerInfo _computerInfo;

        public BaseMetric(string name, string unit, IComputerInfo computerInfo)
        {
            _name = name;
            _unit = unit;
            _computerInfo = computerInfo;
        }

        public object GetValue()
        {
            return this._computerInfo.GetValue();
        }

        public Tag ToTag()
        {
            return new Tag()
            {
                TagName = _name,
                EngUnit = _unit
            };
        }
    }
}