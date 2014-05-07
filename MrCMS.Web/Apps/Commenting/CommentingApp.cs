using System.Web.Mvc;
using MrCMS.Apps;
using MrCMS.Entities.Multisite;
using MrCMS.Installation;
using MrCMS.Web.Apps.Commenting.Areas.Admin.Controllers;
using MrCMS.Web.Apps.Commenting.Controllers;
using MrCMS.Web.Apps.Commenting.DbConfiguration.Listeners;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using Ninject;

namespace MrCMS.Web.Apps.Commenting
{
    public class CommentingApp : MrCMSApp
    {
        public const string Name = "Commenting";
        public const string LayoutAreaName = "Comments";

        protected override void RegisterApp(MrCMSAppRegistrationContext context)
        {
            context.MapAreaRoute("Commenting admin controllers", "Admin", "Admin/Apps/Commenting/{controller}/{action}/{id}",
                         new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                         new[] { typeof(CommentingSettingsController).Namespace });
            context.MapRoute("Add Comments Form", "comments/add/{id}",
                             new { controller = "Comments", action = "Add" },
                             new[] { typeof(CommentsController).Namespace });
            context.MapRoute("Show Comment Votes", "comments/votes/{id}",
                             new { controller = "Comments", action = "Votes" },
                             new[] { typeof(CommentsController).Namespace });
            context.MapRoute("Show Comments", "comments/Show/{id}",
                             new { controller = "Comments", action = "Show" },
                             new[] { typeof(CommentsController).Namespace });
            context.MapRoute("Guest Comment", "comments/Post/Guest",
                             new { controller = "Comments", action = "Guest" },
                             new[] { typeof(CommentsController).Namespace });
            context.MapRoute("Logged in Comment", "comments/Post/User",
                             new { controller = "Comments", action = "LoggedIn" },
                             new[] { typeof(CommentsController).Namespace });
            context.MapRoute("Comment - Upvote", "comments/upvote",
                             new { controller = "CommentVoting", action = "Upvote" },
                             new[] { typeof(CommentVotingController).Namespace });
            context.MapRoute("Comment - Downvote", "comments/downvote",
                             new { controller = "CommentVoting", action = "Downvote" },
                             new[] { typeof(CommentVotingController).Namespace });
            context.MapRoute("Comment - Report", "comments/report",
                             new { controller = "CommentReporting", action = "Report" },
                             new[] { typeof(CommentReportingController).Namespace });
        }

        public override string AppName
        {
            get { return Name; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        protected override void RegisterServices(IKernel kernel)
        {
        }

        protected override void OnInstallation(ISession session, InstallModel model, Site site)
        {
        }

        protected override void AppendConfiguration(Configuration configuration)
        {
            configuration.AppendListeners(ListenerType.PostInsert, new IPostInsertEventListener[] { new CommentWidgetAppender() });
        }
    }
}