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
        public string UniqueKey { get { return string.Format("{0}-{1}", SystemType, Id); } }
        public DateTime CreatedOn { get; set; }
        public IEnumerable<string> PrimarySearchTerms { get; set; }
        public IEnumerable<string> SecondarySearchTerms { get; set; }
    }
}