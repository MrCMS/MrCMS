using System;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Website
{
    public class CacheInHttpContext : ICacheInHttpContext
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CacheInHttpContext(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public T GetForRequest<T>(string key, Func<T> getFunc)
        {
            var context = _contextAccessor.HttpContext;
            if (context == null)
                return getFunc();
            if (context.Items.ContainsKey(key))
            {
                return (T) context.Items[key];
            }

            var result = getFunc();
            context.Items[key] = result;
            return result;
        }
    }
}