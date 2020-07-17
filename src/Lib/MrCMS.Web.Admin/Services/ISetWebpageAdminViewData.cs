using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Admin.Services
{
    public interface ISetWebpageAdminViewData
    {
        void SetViewData<T>(ViewDataDictionary viewData, T webpage) where T : Webpage;
        void SetViewDataForAdd(ViewDataDictionary viewData, string type);
    }
}