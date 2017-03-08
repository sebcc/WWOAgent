using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WWOTelemetry.Model;

namespace WWOTelemetry.Services
{
    internal class JsonWonderwareOnline : IWonderwareOnline
    {
        private HttpClient _httpClient = new HttpClient();
        private string _key;

        public JsonWonderwareOnline(string key)
        {
            _key = key;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
        }

        public async Task SendDataAsync(IEnumerable<Data> data)
        {
            if (!data.Any())
            {
                return;
            }

            var content = new DataRequest();

            var temp = new List<Dictionary<string, object>>();

            foreach (var group in data.GroupBy(d => d.Timestamp))
            {
                var timestampContent = new Dictionary<string, object>();
                timestampContent.Add("dateTime", group.Key);

                foreach (var value in group)
                {
                    timestampContent.Add(value.Name, value.Value);
                }
                temp.Add(timestampContent);
            }

            content.data = temp.ToArray();

            await _httpClient.PostAsync("https://online.wonderware.com/apis/upload/datasource", new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
        }

        public async Task SendTagsAsync(IEnumerable<Tag> tags)
        {
            if (!tags.Any())
            {
                return;
            }

            var content = new TagRequest();
            content.metadata = tags.ToArray();

            await _httpClient.PostAsync("https://online.wonderware.com/apis/upload/datasource", new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
        }
    }
}