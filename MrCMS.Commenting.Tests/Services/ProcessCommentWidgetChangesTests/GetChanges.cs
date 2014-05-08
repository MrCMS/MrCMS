using System;
using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Settings;
using Xunit;

namespace MrCMS.Commenting.Tests.Services.ProcessCommentWidgetChangesTests
{
    public class GetChanges
    {
        [Fact]
        public void BothEmptyIfListIsTheSame()
        {
            CommentingSettings previousSettings = TestableCommentingSettings.Get(typeof (BasicMappedWebpage));
            CommentingSettings newSettings = TestableCommentingSettings.Get(typeof (BasicMappedWebpage));
            ProcessCommentWidgetChanges processCommentWidgetChanges = new ProcessCommentWidgetChangesBuilder().Build();

            CommentedEnabledChangedResult result = processCommentWidgetChanges.GetChanges(previousSettings, newSettings);

            result.Added.Should().BeEmpty();
            result.Removed.Should().BeEmpty();
        }

        [Fact]
        public void AddedShouldHaveThoseThatAreInNewButNotInOld()
        {
            CommentingSettings previousSettings = TestableCommentingSettings.Get();
            CommentingSettings newSettings = TestableCommentingSettings.Get(typeof (BasicMappedWebpage));
            ProcessCommentWidgetChanges processCommentWidgetChanges = new ProcessCommentWidgetChangesBuilder().Build();

            CommentedEnabledChangedResult result = processCommentWidgetChanges.GetChanges(previousSettings, newSettings);

            result.Added.Should().BeEquivalentTo(new List<Type> {typeof (BasicMappedWebpage)});
        }

        [Fact]
        public void RemovedShouldHaveThoseThatAreInOldButNotInNew()
        {
            CommentingSettings previousSettings = TestableCommentingSettings.Get(typeof (BasicMappedWebpage));
            CommentingSettings newSettings = TestableCommentingSettings.Get();
            ProcessCommentWidgetChanges processCommentWidgetChanges = new ProcessCommentWidgetChangesBuilder().Build();

            CommentedEnabledChangedResult result = processCommentWidgetChanges.GetChanges(previousSettings, newSettings);

            result.Removed.Should().BeEquivalentTo(new List<Type> {typeof (BasicMappedWebpage)});
        }
    }
}