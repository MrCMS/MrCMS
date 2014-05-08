using System;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Commenting.Entities;

namespace MrCMS.Commenting.Tests.Support
{
    public class CommentBuilder
    {
        private bool? _approved;
        private DateTime? _createdOn;
        private string _email = "test@example.com";
        private string _message = "test comment";
        private Webpage _webpage;
        private Comment _inReplyTo;
        private Site _site;

        public Comment Build()
        {
            var comment = new Comment
                              {
                                  Approved = _approved,
                                  Email = _email,
                                  Message = _message,
                                  Webpage = _webpage,
                                  InReplyTo = _inReplyTo,
                                  Site = _site
                              };
            if (_createdOn.HasValue)
                comment.CreatedOn = _createdOn.Value;
            return comment;
        }

        public CommentBuilder IsApproved()
        {
            _approved = true;
            return this;
        }

        public CommentBuilder IsNotApproved()
        {
            _approved = false;
            return this;
        }

        public CommentBuilder IsPending()
        {
            _approved = null;
            return this;
        }

        public CommentBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public CommentBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public CommentBuilder WithCreatedOn(DateTime createdOn)
        {
            _createdOn = createdOn;
            return this;
        }

        public CommentBuilder ForWebpage(Webpage webpage)
        {
            _webpage = webpage;
            return this;
        }

        public CommentBuilder InReplyTo(Comment inReplyTo)
        {
            _inReplyTo = inReplyTo;
            return this;
        }

        public CommentBuilder  WithSite(Site site)
        {
            _site = site;
            return this;
        }
    }
}