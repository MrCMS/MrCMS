using System;
using System.Threading.Tasks;
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
            if (result != null)
            {
                context.Items[key] = result;
            }

            return result;
        }

        public async Task<T> GetForRequestAsync<T>(string key, Func<Task<T>> getFunc)
        {
            var context = _contextAccessor.HttpContext;
            if (context == null)
                return await getFunc();
            if (context.Items.ContainsKey(key))
            {
                return (T) context.Items[key];
            }

            var result = await getFunc();
            if (result != null)
            {
                context.Items[key] = result;
            }

            return result;
        }
    }
}