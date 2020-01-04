using MrCMS.Helpers;
using MrCMS.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.People;

namespace MrCMS.Website.PushNotifications
{
    public class PushNotificationSubscriptionManager : IPushNotificationSubscriptionManager
    {
        private readonly IRepository<PushSubscription> _repository;
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly ISendPushNotification _sendPushNotification;
        private readonly WebPushSettings _settings;

        public PushNotificationSubscriptionManager(IRepository<PushSubscription> repository, IGetCurrentUser getCurrentUser, IGetWebPushSettings getSettings, ISendPushNotification sendPushNotification)
        {
            _repository = repository;
            _getCurrentUser = getCurrentUser;
            _settings = getSettings.GetSettings();
            _sendPushNotification = sendPushNotification;
        }

        public async Task<WebPushResult> CreateSubscription(PushNotificationSubscription subscription)
        {
            var pushSubscription = new Entities.People.PushSubscription
            {
                AuthSecret = subscription.AuthSecret,
                Endpoint = subscription.Endpoint,
                Key = subscription.Key,
                User = _getCurrentUser.Get()
            };

            await _repository.Add(pushSubscription);

            return await _sendPushNotification.SendNotification(pushSubscription, _settings.SubscriptionConfirmationMessage);
        }


        public async Task<WebPushResult> RemoveSubscription(string endpoint)
        {
            var pushSubscriptions = await _repository.Query().Where(x => x.Endpoint == endpoint).ToListAsync();
            await _repository.DeleteRange(pushSubscriptions);

            return new WebPushResult();
        }

        public string GetServiceWorkerJavaScript()
        {
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
});".Replace("PUBLIC_KEY", _settings.VapidPublicKey);
        }
    }
}