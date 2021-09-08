using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MrCMS.Tasks
{
    public class TriggerUrls : ITriggerUrls
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TriggerUrls(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task Trigger(IEnumerable<string> urls)
        {
            if (urls == null)
                return;
            foreach (var url in urls)
            {
                try
                {
                    // we're basically firing and forgetting a request here

                    var httpClient = _httpClientFactory.CreateClient();
                    httpClient.Timeout = TimeSpan.FromMinutes(30);
                    httpClient.GetAsync(url);
                }
                catch
                {
                    
                }
            }
        }
    }
}