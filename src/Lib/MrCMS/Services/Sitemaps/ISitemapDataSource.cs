using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Services.Sitemaps
{
    public interface ISitemapDataSource
    {
        Task<IEnumerable<SitemapData>> GetAdditionalData();
        IEnumerable<Type> TypesToRemove { get; }
    }
}