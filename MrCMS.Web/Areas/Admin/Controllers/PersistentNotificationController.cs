using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Notifications;
using MrCMS.Entities.UserProfile;
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
        void MarkAllAsRead();
    }

    public class PersistentNotificationUIService : IPersistentNotificationUIService
    {
        private readonly ISession _session;

        public PersistentNotificationUIService(ISession session)
        {
            _session = session;
        }

        public IList<NotificationModel> GetNotifications()
        {
            var user = CurrentRequestData.CurrentUser;
            var lastReadDate = user.Get<NotificationSettings, DateTime?>(settings => settings.LastMarkedAsRead);
            var queryOver = _session.QueryOver<Notification>();

            if (lastReadDate.HasValue)
                queryOver = queryOver.Where(notification => notification.CreatedOn >= lastReadDate);

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

        public void MarkAllAsRead()
        {
            var notificationSettings = CurrentRequestData.CurrentUser.Get<NotificationSettings>();
            notificationSettings.LastMarkedAsRead = CurrentRequestData.Now;
            _session.Transact(session => session.Update(notificationSettings));
        }
    }
}