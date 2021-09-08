using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web
{
    public class UrlHistory : SiteEntity
    {
        [Required]
        public virtual string UrlSegment { get; set; }

        public virtual Webpage Webpage { get; set; }
        
        public virtual string RedirectUrl { get; set; }
        
        public virtual int FailedLookupCount { get; set; }
        public virtual bool IsGone { get; set; }
        public virtual bool IsIgnored { get; set; }

        public virtual UrlHistoryType UrlHistoryType
        {
            get
            {
                if (IsGone)
                    return UrlHistoryType.Gone;
                if (IsIgnored)
                    return UrlHistoryType.Ignored;
                if (Webpage != null)
                    return UrlHistoryType.ToWebpage;
                if (RedirectUrl != null)
                    return UrlHistoryType.ToUrl;
                return UrlHistoryType.Unhandled;
            }
        }

        public virtual string InitialReferer { get; set; }
    }
}