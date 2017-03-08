using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWOTelemetry.Model;

namespace WWOTelemetry.Services
{
    internal interface IWonderwareOnline
    {
        Task SendTagsAsync(IEnumerable<Tag> tags);

        Task SendDataAsync(IEnumerable<Data> data);
    }
}