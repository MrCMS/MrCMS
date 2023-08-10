using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Website.CMS
{
    public static class CMSActionFilterExtensions
    {
        private class MrCMSRequestStatusFeature
        {
            public bool IsCMSRequest { get; set; }
            public bool IsPreview { get; set; }
        }

        public static bool IsCMSRequest(this ActionContext context)
        {
            return context.HttpContext.IsCMSRequest();
        }

        public static bool IsCMSRequest(this IHtmlHelper helper)
        {
            return helper.ViewContext.HttpContext.IsCMSRequest();
        }

        public static bool IsCMSRequest(this HttpContext context)
        {
            var status = context.GetMrCMSRequestStatusFeature();
            return status.IsCMSRequest;
        }

        public static void MakeCMSRequest(this HttpContext context)
        {
            var status = context.GetMrCMSRequestStatusFeature();
            status.IsCMSRequest = true;
        }

        public static bool IsPreview(this ActionContext context)
        {
            return context.HttpContext.IsPreview();
        }

        public static bool IsPreview(this IHtmlHelper helper)
        {
            return helper.ViewContext.HttpContext.IsPreview();
        }

        public static bool IsPreview(this HttpContext context)
        {
            var status = context.GetMrCMSRequestStatusFeature();
            return status.IsPreview;
        }

        public static void MakePreview(this HttpContext context)
        {
            var status = context.GetMrCMSRequestStatusFeature();
            status.IsPreview = true;
        }


        private static MrCMSRequestStatusFeature GetMrCMSRequestStatusFeature(this HttpContext context)
        {
            // get existing feature
            var existing = context.Features.Get<MrCMSRequestStatusFeature>();

            // if it exists, return it
            if (existing != null)
                return existing;

            // create a new feature
            var newFeature = new MrCMSRequestStatusFeature();

            // add it to the context
            context.Features.Set(newFeature);

            // return it
            return newFeature;
        }
    }
}
