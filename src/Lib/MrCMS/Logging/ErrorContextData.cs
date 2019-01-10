using System.Collections.Generic;

namespace MrCMS.Logging
{
    public class ErrorContextData
    {
        public string Uri { get; set; }
        public string Method { get; set; }
        public string UserAgent { get; set; }
        public string IPAddress { get; set; }
        public string User { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}