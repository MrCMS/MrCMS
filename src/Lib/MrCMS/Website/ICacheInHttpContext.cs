using System;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public interface ICacheInHttpContext
    {
        Task<T> GetForRequest<T>(string key, Func<Task<T>> getFunc);
    }
}