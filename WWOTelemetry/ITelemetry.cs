using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWOTelemetry.Model;

namespace WWOTelemetry
{
    internal interface ITelemetry
    {
        void Track(string name, object value);

        void CreateTag(Tag tag);
    }
}