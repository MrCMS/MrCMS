using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IUrlHistoryService
    {
        void Delete(UrlHistory urlHistory);
        void Add(UrlHistory urlHistory);
    }
}