using System;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Services.Notifications
{
    public interface INotificationDisabler
    {
        IDisposable Disable();
    }

    public class NotificationDisabler : INotificationDisabler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public NotificationDisabler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public IDisposable Disable()
        {
            return new NotificationDisableContext(_contextAccessor.HttpContext);
        }

        private class NotificationDisableContext : IDisposable
        {
            private readonly HttpContext _context;
            private readonly bool _enableOnDispose;

            public NotificationDisableContext(HttpContext context)
            {
                _context = context;
                if (!_context.AreNotificationsDisabled())
                {
                    _enableOnDispose = true;
                    _context.DisableNotifications();
                }
            }

            public void Dispose()
            {
                if (_enableOnDispose)
                    _context.EnableNotifications();
            }
        }

    }
}