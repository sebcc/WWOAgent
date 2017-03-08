using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWOTelemetry.Engines;
using WWOTelemetry.Model;
using WWOTelemetry.Services;

namespace WWOTelemetry
{
    public class WWOClient : ITelemetry
    {
        private BlockingCollection<Data> _datas = new BlockingCollection<Data>();
        private BlockingCollection<Tag> _tags = new BlockingCollection<Tag>();

        private IWonderwareOnline _wonderwareOnline;

        private SenderEngine _senderEngine;

        public WWOClient(string key)
        {
            _wonderwareOnline = new JsonWonderwareOnline(key);
            _senderEngine = new SenderEngine(_tags, _datas, _wonderwareOnline);
        }

        public void Track(string name, object value)
        {
            var newData = new Data()
            {
                Name = name,
                Value = value,
                Timestamp = DateTime.UtcNow.ToString("O")
            };

            _datas.Add(newData);
        }

        public void CreateTag(Tag tag)
        {
            _tags.Add(tag);
        }
    }
}