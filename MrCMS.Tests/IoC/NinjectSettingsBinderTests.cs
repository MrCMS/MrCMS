using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using MrCMS.Entities.Multisite;
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
            var methodInfo = typeof (ConfigurationProvider).GetMethodExt("GetSiteSettings");

            methodInfo.Should().NotBeNull();
        }
    }
}
