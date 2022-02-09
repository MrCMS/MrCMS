using System.Threading.Tasks;
using MrCMS.Tests.Stubs;
using NHibernate.Criterion;
using Xunit;
using MrCMS.Helpers;
using FluentAssertions;
using MrCMS.TestSupport;

namespace MrCMS.Tests.Helpers
{
    public class SessionHelperTests : InMemoryDatabaseTest
    {

        [Fact]
        public async Task SessionHelper_Paged_WorksWhenPassedAnIQueryOver()
        {
            await Session.TransactAsync(async session =>
            {
                for (int i = 0; i < 100; i++)
                {
                    await session.SaveAsync(new StubWebpage { Name = i.ToString() });
                }
            });

            var pagedList = Session.QueryOver<StubWebpage>().OrderBy(webpage => webpage.Id).Asc.Paged(4, 10);

            pagedList.TotalItemCount.Should().Be(100);
            pagedList.Count.Should().Be(10);
            pagedList[0].Id.Should().Be(31);
            pagedList[9].Id.Should().Be(40);
        }

        [Fact]
        public async Task SessionHelper_Paged_WorksWhenPassedAQueryOver()
        {
            await Session.TransactAsync(async session =>
            {
                for (int i = 0; i < 100; i++)
                {
                    await session.SaveAsync(new StubWebpage { Name = i.ToString() });
                }
            });

            var pagedList = Session.Paged(QueryOver.Of<StubWebpage>().OrderBy(webpage => webpage.Id).Asc, 4, 10);

            pagedList.TotalItemCount.Should().Be(100);
            pagedList.Count.Should().Be(10);
            pagedList[0].Id.Should().Be(31);
            pagedList[9].Id.Should().Be(40);
        }
    }
}