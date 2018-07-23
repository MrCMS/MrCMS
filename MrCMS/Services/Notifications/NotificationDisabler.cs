using System;
using Microsoft.AspNetCore.Http;
using MrCMS.Website;

namespace MrCMS.Services.Notifications
{
    public class NotificationDisabler : IDisposable
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly bool _enableOnDispose;

        public NotificationDisabler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            if (!_contextAccessor.HttpContext.AreNotificationsDisabled())
            {
                _enableOnDispose = true;
                _contextAccessor.HttpContext.DisableNotifications();
            }
        }

        public void Dispose()
        {
            if (_enableOnDispose)
                _contextAccessor.HttpContext.EnableNotifications();
        }
    }
}