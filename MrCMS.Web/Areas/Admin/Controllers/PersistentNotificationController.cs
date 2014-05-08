using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Notifications;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Models.Notifications;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using NHibernate;
using NHibernate.Transform;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class PersistentNotificationController : MrCMSAdminController
    {
        private readonly IPersistentNotificationUIService _service;

        public PersistentNotificationController(IPersistentNotificationUIService service)
        {
            _service = service;
        }

        public PartialViewResult Show()
        {
            return PartialView();
        }

        public JsonResult Get()
        {
            return Json(_service.GetNotifications(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCount()
        {
            return Json(_service.GetNotificationCount(), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Navbar()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult MarkAllAsRead()
        {
            _service.MarkAllAsRead();
            return Json(true);
        }
    }

    public interface IPersistentNotificationUIService
    {
        IList<NotificationModel> GetNotifications();
        int GetNotificationCount();
        void MarkAllAsRead();
    }

    public class PersistentNotificationUIService : IPersistentNotificationUIService
    {
        private readonly ISession _session;
        private readonly IUserService _userService;

        public PersistentNotificationUIService(ISession session, IUserService userService)
        {
            _session = session;
            _userService = userService;
        }

        public IList<NotificationModel> GetNotifications()
        {
            var user = CurrentRequestData.CurrentUser;
            var queryOver = _session.QueryOver<Notification>();

            if (user.LastNotificationReadDate.HasValue)
                queryOver = queryOver.Where(notification => notification.CreatedOn >= user.LastNotificationReadDate);

            NotificationModel notificationModelAlias = null;
            return queryOver.SelectList(
                builder =>
                builder.Select(notification => notification.Message)
                       .WithAlias(() => notificationModelAlias.Message)
                       .Select(notification => notification.CreatedOn)
                       .WithAlias(() => notificationModelAlias.DateValue))
                            .OrderBy(notification => notification.CreatedOn).Desc
                            .TransformUsing(Transformers.AliasToBean<NotificationModel>())
                            .Take(15)
                            .Cacheable()
                            .List<NotificationModel>();
        }

        public int GetNotificationCount()
        {
            var user = CurrentRequestData.CurrentUser;
            var queryOver = _session.QueryOver<Notification>();

            if (user.LastNotificationReadDate.HasValue)
                queryOver = queryOver.Where(notification => notification.CreatedOn >= user.LastNotificationReadDate);

            return queryOver.RowCount();
        }

        public void MarkAllAsRead()
        {
            var user = _userService.GetCurrentUser(CurrentRequestData.CurrentContext);
            user.LastNotificationReadDate = CurrentRequestData.Now;
            _session.Transact(session => session.Update(user));
        }
    }
}