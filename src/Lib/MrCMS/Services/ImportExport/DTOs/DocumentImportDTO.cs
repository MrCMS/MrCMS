using System;
using System.Collections.Generic;

namespace MrCMS.Services.ImportExport.DTOs
{
    public class DocumentImportDTO
    {
        public DocumentImportDTO()
        {
            Tags=new List<string>();
            UrlHistory=new List<string>();
        }

        public string UrlSegment { get; set; }
        public string ParentUrl { get; set; }
        public string DocumentType { get; set; }
        public string Name { get; set; }
        public string BodyContent { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public bool RevealInNavigation { get; set; }
        public int DisplayOrder { get; set; }
        public bool RequireSSL { get; set; }
        public DateTime? PublishDate { get; set; }

        public List<string> Tags { get; set; }
        public List<string> UrlHistory { get; set; }
    }
}