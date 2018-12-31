using System.Collections.Generic;

namespace MrCMS.Website.CMS
{
    public interface IQuerySerializer
    {
        IDictionary<string, object> GetRoutingData(object obj);
        string AppendToUrl(string url, IDictionary<string, object> routingData);
    }
}