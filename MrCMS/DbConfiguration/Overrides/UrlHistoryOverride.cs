using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class UrlHistoryOverride : IAutoMappingOverride<UrlHistory>
    {
        public void Override(AutoMapping<UrlHistory> mapping)
        {
            mapping.Map(x => x.UrlSegment).Index("idx_UrlSegment_UrlHistory");
        }
    }
}
