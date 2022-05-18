using System;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public interface ICacheInHttpContext
    {
        T GetForRequest<T>(string key, Func<T> getFunc);
        Task<T> GetForRequestAsync<T>(string key, Func<Task<T>> getFunc);
        void ClearForRequest(string key);
    }
}