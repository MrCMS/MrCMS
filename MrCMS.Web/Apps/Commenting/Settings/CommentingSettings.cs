using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Commenting.Settings
{
    public class CommentingSettings : SiteSettingsBase
    {
        public CommentingSettings()
        {
            CommentApprovalType = CommentApprovalType.Guest;
            //InitialNumberOfCommentsToShow = 10;
            AllowVoting = true;
            NotifyCommentAddedEmail = string.Empty;

            CommentAddedMessage = "Thanks for posting!";
            CommentPendingApprovalMessage = "Thanks for posting! Your comment is pending approval by admins, and will be shown shortly.";
        }

        [DisplayName("Allow guest comments?")]
        public bool AllowGuestComments { get; set; }

        [DisplayName("Type of comments that require approval")]
        public CommentApprovalType CommentApprovalType { get; set; }

        [DisplayName("Allow voting?")]
        public bool AllowVoting { get; set; }

        [DisplayName("Notify comments posted email(s)")]
        public string NotifyCommentAddedEmail { get; set; }

        //[DisplayName("Initial number of comments to show")]
        //public int InitialNumberOfCommentsToShow { get; set; }

        public string AllowedPageTypes { get; set; }

        public string CommentAddedMessage { get; set; }
        public string CommentPendingApprovalMessage { get; set; }

        public IEnumerable<string> EmailsToNotify
        {
            get
            {
                if (string.IsNullOrWhiteSpace(NotifyCommentAddedEmail))
                    yield break;
                foreach (
                    string email in NotifyCommentAddedEmail.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    yield return email;
            }
        }

        public virtual IEnumerable<Type> AllowedTypes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AllowedPageTypes))
                    yield break;
                string[] types = AllowedPageTypes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string type in types)
                {
                    yield return TypeHelper.GetTypeByName(type);
                }
            }
        }

        public CommentingSettings SetAllowedPageTypes(params Type[] types)
        {
            AllowedPageTypes = string.Join(",", types.Select(type => type.FullName));
            return this;
        }

        public bool IsAllowedType(Webpage webpage)
        {
            return webpage != null && AllowedTypes.Contains(webpage.GetType());
        }
    }

    public enum CommentApprovalType
    {
        None,
        Guest,
        All
    }
}