using System;
using System.Collections.Generic;
using FakeItEasy;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Settings;
using Xunit;

namespace MrCMS.Commenting.Tests.Services.ProcessCommentWidgetChangesTests
{
    public class Process
    {
        [Fact]
        public void AddsWidgetsToAllAddedTypes()
        {
            var result = new CommentedEnabledChangedResult
                             {
                                 Added = new List<Type>
                                             {
                                                 typeof (BasicMappedWebpage),
                                                 typeof (BasicMappedWebpage2)
                                             }
                             };
            var addCommentWidgets = A.Fake<IAddCommentWidgets>();
            ProcessCommentWidgetChanges processCommentWidgetChanges =
                new ProcessCommentWidgetChangesBuilder().WithChangedResult(result)
                                                        .WithAddCommentWidgets(addCommentWidgets)
                                                        .Build();
            processCommentWidgetChanges.Process(A<CommentingSettings>._, A<CommentingSettings>._);

            result.Added.ForEach(type => A.CallTo(() => addCommentWidgets.AddWidgets(type)).MustHaveHappened());
        }

        [Fact]
        public void RemovesWidgetsFromAllRemovedTypes()
        {
            var result = new CommentedEnabledChangedResult
                             {
                                 Removed = new List<Type>
                                               {
                                                   typeof (BasicMappedWebpage),
                                                   typeof (BasicMappedWebpage2)
                                               }
                             };
            var removeCommentWidgets = A.Fake<IRemoveCommentWidgets>();
            ProcessCommentWidgetChanges processCommentWidgetChanges =
                new ProcessCommentWidgetChangesBuilder().WithChangedResult(result)
                                                        .WithRemoveCommentWidgets(removeCommentWidgets)
                                                        .Build();
            processCommentWidgetChanges.Process(A<CommentingSettings>._, A<CommentingSettings>._);

            result.Added.ForEach(type => A.CallTo(() => removeCommentWidgets.RemoveWidgets(type)).MustHaveHappened());
        }
    }
}