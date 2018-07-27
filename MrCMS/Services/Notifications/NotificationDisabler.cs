using System;
using Microsoft.AspNetCore.Http;
using MrCMS.Website;

namespace MrCMS.Services.Notifications
{
    public class NotificationDisabler : IDisposable
    {
        private readonly HttpContext _context;
        private readonly bool _enableOnDispose;

        public NotificationDisabler(HttpContext context)
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