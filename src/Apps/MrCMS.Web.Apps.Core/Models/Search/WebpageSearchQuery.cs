using System;
using System.ComponentModel;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Definitions;
using MrCMS.Indexing.Management;

namespace MrCMS.Web.Apps.Core.Models.Search
{
    public class WebpageSearchQuery
    {
        public WebpageSearchQuery()
        {
            Page = 1;
        }

        public string Term { get; set; }
        public int Page { get; set; }
        public int? ParentId { get; set; }
        public string Type { get; set; }

        [DisplayName("Created On From")]
        public DateTime? CreatedOnFrom { get; set; }

        [DisplayName("Created On To")]
        public DateTime? CreatedOnTo { get; set; }

    }
}