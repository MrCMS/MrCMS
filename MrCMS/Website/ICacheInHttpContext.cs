using System;

namespace MrCMS.Website
{
    public interface ICacheInHttpContext
    {
        T GetForRequest<T>(string key, Func<T> getFunc);
    }
}