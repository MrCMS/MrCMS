using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Web.Apps.Commenting.Settings;

namespace MrCMS.Commenting.Tests.Services.ProcessCommentWidgetChangesTests
{
    public class TestableCommentingSettings : CommentingSettings
    {
        public static CommentingSettings Get(params Type[] types)
        {
            return new TestableCommentingSettings {SpecifiedAllowedTypes = types.ToList()};
        }
        public List<Type> SpecifiedAllowedTypes { get; set; }

        public override IEnumerable<Type> AllowedTypes
        {
            get
            {
                return SpecifiedAllowedTypes ?? base.AllowedTypes;
            }
        }
    }
}