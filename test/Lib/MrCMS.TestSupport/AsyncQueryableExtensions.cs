using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FakeItEasy.Configuration;
using MockQueryable.FakeItEasy;

namespace MrCMS.TestSupport
{
    public static class AsyncQueryableExtensions
    {
        public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> enumerable) where T : class
        {
            return enumerable.AsQueryable().BuildMock();
        }

        public static IAfterCallConfiguredWithOutAndRefParametersConfiguration<IReturnValueConfiguration<IQueryable<T>>> ReturnsAsAsyncQueryable<T>(this IReturnValueConfiguration<IQueryable<T>> configuration, params T[] values)
            where T : class   {
            return configuration.Returns(values.AsAsyncQueryable());
        }
    }

}