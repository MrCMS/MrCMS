using System;
using MrCMS.Entities.Multisite;
using Directory = Lucene.Net.Store.Directory;

namespace MrCMS.Indexing.Management
{
    public interface IGetLuceneDirectory
    {
        Directory Get(Site site, string folderName);
    }
}