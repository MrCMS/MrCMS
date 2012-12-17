using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Models;
using Xunit;

namespace MrCMS.Tests.Helpers
{
    public class TypeHelperTests
    {
        [Fact]
        public void TypeHelper_GetAllConcreteTypesAssignableFrom_GetsTypesFromAGivenInterface()
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<IAdminMenuItem>();

            types.Should().Contain(typeof (TestAdminMenuItem));
        }

        private class TestAdminMenuItem : IAdminMenuItem
        {
            public string Text { get; private set; }
            public string Url { get; private set; }
            public List<IMenuItem> Children { get; private set; }
            public int DisplayOrder { get; private set; }
        }
    }
}