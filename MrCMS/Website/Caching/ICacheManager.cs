using System;

namespace MrCMS.Website.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key, Func<T> func, TimeSpan time);
    }
}