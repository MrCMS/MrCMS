using System.Collections.Generic;
using System.Net.Http;

namespace MrCMS.Tasks
{
    public interface ITriggerUrls
    {
        void Trigger(IEnumerable<string> urls);
    }

    public class TriggerUrls : ITriggerUrls
    {
        public void Trigger(IEnumerable<string> urls)
        {
            if (urls == null)
                return;
            foreach (var url in urls)
            {
                try
                {
                    // we're basically firing and forgetting a request here
                    new HttpClient().GetAsync(url);
                }
                catch
                {
                    
                }
            }
        }
    }
}