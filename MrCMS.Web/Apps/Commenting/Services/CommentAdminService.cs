using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Paging;
using MrCMS.Web.Apps.Commenting.Entities;
using MrCMS.Web.Apps.Commenting.Models;
using NHibernate;
using MrCMS.Helpers;
using NHibernate.Criterion;
using System.Linq;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public class CommentAdminService : ICommentAdminService
    {
        private readonly ISession _session;
        private readonly Site _site;

        public CommentAdminService(ISession session,Site site )
        {
            _session = session;
            _site = site;
        }

        public IPagedList<Comment> Search(CommentSearchQuery query)
        {
            var queryOver = _session.QueryOver<Comment>().Where(comment => comment.Site.Id == _site.Id);

            switch (query.ApprovalStatus)
            {
                case ApprovalStatus.Pending:
                    queryOver = queryOver.Where(comment => comment.Approved == null);
                    break;
                case ApprovalStatus.Rejected:
                    queryOver = queryOver.Where(comment => comment.Approved == false);
                    break;
                case ApprovalStatus.Approved:
                    queryOver = queryOver.Where(comment => comment.Approved == true);
                    break;
            }
            if (!string.IsNullOrWhiteSpace(query.Email))
                queryOver = queryOver.Where(comment => comment.Email.IsLike(query.Email, MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(query.Message))
                queryOver = queryOver.Where(comment => comment.Message.IsLike(query.Message, MatchMode.Anywhere));
            if (query.DateFrom.HasValue)
                queryOver = queryOver.Where(comment => comment.CreatedOn >= query.DateFrom);
            if (query.DateTo.HasValue)
                queryOver = queryOver.Where(comment => comment.CreatedOn < query.DateTo);

            return queryOver.OrderBy(comment => comment.CreatedOn).Asc.Paged(query.Page);
        }

        public List<SelectListItem> GetApprovalOptions()
        {
            return Enum.GetValues(typeof(ApprovalStatus))
                       .Cast<ApprovalStatus>()
                       .BuildSelectItemList(status => status.ToString(), emptyItem: null);
        }

        public void Update(Comment comment)
        {
            _session.Transact(session => session.Update(comment));
        }

        public void Delete(Comment comment)
        {
            _session.Transact(session => session.Delete(comment));
        }
    }
}