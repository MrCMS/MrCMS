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
        public void NinjectSettingsBinder_GetMethodExt_ReturnsGenericGetSettingsMethod()
        {
            var methodInfo = typeof (ConfigurationProvider).GetMethodExt("GetSettings", 
                                                                         typeof (Site));

            methodInfo.Should().NotBeNull();
        }
    }
}
