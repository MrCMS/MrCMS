using System;
using System.Collections.Generic;

namespace MrCMS.Search.Models
{
    public class UniversalSearchItem
    {
        public string SystemType { get; set; }
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string ActionUrl { get; set; }
        public Guid? SearchGuid { get; set; }
        public IEnumerable<string> SearchTerms { get; set; }
    }
}