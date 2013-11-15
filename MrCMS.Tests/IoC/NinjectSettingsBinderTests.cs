using FluentAssertions;
using MrCMS.Settings;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.IoC
{
    public class NinjectSettingsBinderTests
    {
        [Fact]
        public void NinjectSettingsBinder_GetMethodExt_ReturnsGenericGetSiteSettingsMethod()
        {
            var methodInfo = typeof(ConfigurationProvider).GetMethodExt("GetSiteSettings");

            methodInfo.Should().NotBeNull();
        }
    }
}
