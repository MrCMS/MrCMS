using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MrCMS.Helpers
{
    public static class Memoizer
    {
        public static Func<R> Memoize<R>(Func<R> func)
        {
            object cache = null;
            return () =>
            {
                if (cache == null)
                    cache = func();
                return (R) cache;
            };
        }

        public static Func<A, R> Memoize<A, R>(Func<A, R> func)
        {
            var cache = new Dictionary<A, R>();
            return a =>
            {
                if (cache.TryGetValue(a, out R value))
                    return value;
                value = func(a);
                cache[a] = value;
                return value;
            };
        }

        public static Func<A, R> ThreadSafeMemoize<A, R>(Func<A, R> func)
        {
            var cache = new ConcurrentDictionary<A, R>();
            return argument => cache.GetOrAdd(argument, func);
        }
    }
}