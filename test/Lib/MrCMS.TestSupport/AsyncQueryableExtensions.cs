using System.Collections.Generic;
using System.Linq;
using FakeItEasy.Configuration;

namespace MrCMS.TestSupport
{
    public static class AsyncQueryableExtensions
    {
        public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> enumerable)
        {
            return new TestAsyncEnumerable<T>(enumerable);
        }

        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<IQueryable<T>>> ReturnsAsAsyncQueryable<T>(this IReturnValueConfiguration<IQueryable<T>> configuration, params T[] values)
        {
            return configuration.ReturnsLazily(x => values.AsAsyncQueryable());
        }
    }

}