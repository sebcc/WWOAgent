using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WWOTelemetry.Model;
using WWOTelemetry.Services;

namespace WWOTelemetry.Engines
{
    internal class SenderEngine
    {
        private BlockingCollection<Tag> _tagsBuffer;
        private BlockingCollection<Data> _dataBuffer;
        private Thread senderThread;
        private IWonderwareOnline _wonderwareOnline;

        public SenderEngine(BlockingCollection<Tag> tagsBuffer, BlockingCollection<Data> dataBuffer, IWonderwareOnline wonderwareOnline)
        {
            _tagsBuffer = tagsBuffer;
            _dataBuffer = dataBuffer;
            _wonderwareOnline = wonderwareOnline;
            senderThread = new Thread(ThreadExection);
            senderThread.Start();
        }

        private void ThreadExection()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                stopwatch.Restart();

                PurgeTags(_tagsBuffer);
                PurgeData(_dataBuffer);

                var waitTime = (int)(10000 - stopwatch.ElapsedMilliseconds);
                if (waitTime > 0)
                {
                    Thread.Sleep(waitTime);
                }
            }
        }

        private void PurgeTags(BlockingCollection<Tag> tagsBuffer)
        {
            var tagsToSend = new List<Tag>();

            Tag tag;
            while (tagsBuffer.TryTake(out tag))
            {
                tagsToSend.Add(tag);
            }

            _wonderwareOnline.SendTagsAsync(tagsToSend.ToArray());
        }

        private void PurgeData(BlockingCollection<Data> dataBuffer)
        {
            var dataToSend = new List<Data>();

            Data data;
            while (dataBuffer.TryTake(out data))
            {
                dataToSend.Add(data);
            }

            _wonderwareOnline.SendDataAsync(dataToSend.ToArray());
        }
    }
}