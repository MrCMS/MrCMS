using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Helpers;

namespace MrCMS.Web.Apps.Articles.Services
{
    public class GetOtherSectionFormData : IGetOtherSectionFormData
    {
        public List<string> GetFormKeys(ControllerContext context)
        {
            var form = context.HttpContext.Request.Form;
            return form.AllKeys.Where(s => s.StartsWith("other-section-")).ToList();
        }

        public List<int> GetSectionIds(ControllerContext context)
        {
            var form = context.HttpContext.Request.Form;
            var otherSectionKeys = GetFormKeys(context);
            return otherSectionKeys
                .Where(s => form[s].Contains("true", StringComparison.InvariantCultureIgnoreCase))
                .Select(s => s.Replace("other-section-", ""))
                .Select(TryGetId)
                .Where(i => i.HasValue)
                .Select(i => i.Value).ToList();
        }

        private int? TryGetId(string arg)
        {
            int id;
            if (int.TryParse(arg, out id))
            {
                return id;
            }
            return null;

        }
    }
}