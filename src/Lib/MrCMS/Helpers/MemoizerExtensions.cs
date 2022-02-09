using System;

namespace MrCMS.Helpers
{
    public static class MemoizerExtensions
    {
        public static Func<R> Memoize<R>(this Func<R> func)
        {
            return Memoizer.Memoize(func);
        }

        public static Func<A, R> Memoize<A, R>(this Func<A, R> func)
        {
            return Memoizer.Memoize(func);
        }

        public static Func<A, R> ThreadSafeMemoize<A, R>(this Func<A, R> func)
        {
            return Memoizer.ThreadSafeMemoize(func);
        }
    }
}