using System.Collections.Generic;
using FakeItEasy;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Settings;
using NHibernate;

namespace MrCMS.Commenting.Tests.Services.CommentsUIServiceTests
{
    public class CommentsUIServiceBuilder
    {
        private bool _guestCommentsEnabled;
        private ISession _session = A.Fake<ISession>();
        private CommentApprovalType _approvalType = CommentApprovalType.None;
        private string _commentApproved;
        private string _commentPending;
        private int? _limit;

        public CommentsUIService Build()
        {
            var settings = new CommentingSettings
                               {
                                   AllowGuestComments = _guestCommentsEnabled,
                                   CommentApprovalType = _approvalType,
                               };
            //if (_limit.HasValue)
            //    settings.InitialNumberOfCommentsToShow = _limit.Value;
            if (!string.IsNullOrWhiteSpace(_commentApproved))
                settings.CommentAddedMessage = _commentApproved;
            if (!string.IsNullOrWhiteSpace(_commentPending))
                settings.CommentPendingApprovalMessage = _commentPending;
            return new CommentsUIService(settings, _session, new List<IOnCommentAdded>());
        }

        public CommentsUIServiceBuilder WithGuestCommentsDisabled()
        {
            _guestCommentsEnabled = false;
            return this;
        }

        public CommentsUIServiceBuilder WithGuestCommentsEnabled()
        {
            _guestCommentsEnabled = true;
            return this;
        }
        public CommentsUIServiceBuilder WithApprovalType(CommentApprovalType approvalType)
        {
            _approvalType = approvalType;
            return this;
        }

        public CommentsUIServiceBuilder WithSession(ISession session)
        {
            _session = session;
            return this;
        }

        public CommentsUIServiceBuilder WithApprovedMessage(string commentApproved)
        {
            _commentApproved = commentApproved;
            return this;
        }

        public CommentsUIServiceBuilder WithPendingMessage(string commentPending)
        {
            _commentPending = commentPending;
            return this;
        }

        public CommentsUIServiceBuilder WithCommentLimit(int limit)
        {
            _limit = limit;
            return this;
        }
    }
}