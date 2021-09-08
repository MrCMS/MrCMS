using MrCMS.Helpers;
using MrCMS.Services;
using NHibernate;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using NHibernate.Linq;

namespace MrCMS.Website.PushNotifications
{
    public class PushNotificationSubscriptionManager : IPushNotificationSubscriptionManager
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly ISession _session;
        private readonly ISendPushNotification _sendPushNotification;
        private IGetWebPushSettings _getSettings;

        public PushNotificationSubscriptionManager(IGetCurrentUser getCurrentUser, ISession session,
            IGetWebPushSettings getSettings, ISendPushNotification sendPushNotification)
        {
            _getCurrentUser = getCurrentUser;
            _session = session;
            _getSettings = getSettings;
            _sendPushNotification = sendPushNotification;
        }

        public async Task<WebPushResult> CreateOrUpdateSubscription(PushNotificationSubscription subscription)
        {
            var existing = await _session.Query<PushSubscription>().WithOptions(options => options.SetCacheable(true))
                .FirstOrDefaultAsync(x => x.Key == subscription.Key);

            if (existing != null)
            {
                // we'll check if the user needs setting and try to do so   
                if (existing.User != null)
                    return new WebPushResult();

                var currentUser = await _getCurrentUser.Get();
                if (currentUser != null)
                {
                    existing.User = currentUser;
                    await _session.TransactAsync(session => session.UpdateAsync(existing));
                }

                return new WebPushResult();
            }


            var pushSubscription = new Entities.People.PushSubscription
            {
                AuthSecret = subscription.AuthSecret,
                Endpoint = subscription.Endpoint,
                Key = subscription.Key,
                User = await _getCurrentUser.Get()
            };

            await _session.TransactAsync(session => session.SaveAsync(pushSubscription));

            var settings = await _getSettings.GetSettings();
            return await _sendPushNotification.SendNotification(pushSubscription,
                settings.SubscriptionConfirmationMessage);
        }


        public async Task<WebPushResult> RemoveSubscription(string endpoint)
        {
            var pushSubscriptions = await _session.Query<Entities.People.PushSubscription>()
                .Where(x => x.Endpoint == endpoint).ToListAsync();
            await _session.TransactAsync(async session =>
            {
                foreach (var subscription in pushSubscriptions)
                {
                    await session.DeleteAsync(subscription);
                }
            });

            return new WebPushResult();
        }

        public async Task<string> GetServiceWorkerJavaScript()
        {
            var settings = await _getSettings.GetSettings();
            return @"
'use strict';

/* eslint-disable max-len */

const applicationServerPublicKey = 'PUBLIC_KEY';

/* eslint-enable max-len */

function urlB64ToUint8Array(base64String) {
  const padding = '='.repeat((4 - base64String.length % 4) % 4);
  const base64 = (base64String + padding)
    .replace(/\-/g, '+')
    .replace(/_/g, '/');

  const rawData = window.atob(base64);
  const outputArray = new Uint8Array(rawData.length);

  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
  }
  return outputArray;
}

self.addEventListener('push', function(event) {
  console.log('[Service Worker] Push Received.');
  console.log(`[Service Worker] Push had this data: ""${event.data.text()}""`);

  var data = event.data.json();

  const title = data.title;
  const options = {
    body: data.body,
    icon: data.icon,
    badge: data.badge,
    data: { url: data.actionUrl }
  };

  event.waitUntil(self.registration.showNotification(title, options));
});

self.addEventListener('notificationclick', function(event)
{
  console.log('[Service Worker] Notification click Received.', event);

  event.notification.close();

  if (event.notification.data.url) {
    event.waitUntil(
      clients.openWindow(event.notification.data.url)
    );
  }
});

self.addEventListener('pushsubscriptionchange', function(event)
{
  console.log('[Service Worker]: \'pushsubscriptionchange\' event fired.');
  const applicationServerKey = urlB64ToUint8Array(applicationServerPublicKey);
  event.waitUntil( 
    self.registration.pushManager.subscribe({
      userVisibleOnly: true,
      applicationServerKey: applicationServerKey
    })
    .then(function(newSubscription) {
    // TODO: Send to application server
    console.log('[Service Worker] New subscription: ', newSubscription);
    })
  );
});".Replace("PUBLIC_KEY", settings.VapidPublicKey);
        }
    }
}