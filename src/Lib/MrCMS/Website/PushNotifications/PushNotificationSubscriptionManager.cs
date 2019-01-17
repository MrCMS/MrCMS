using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using Newtonsoft.Json;
using NHibernate;
using System;
using System.Linq;
using WebPush;

namespace MrCMS.Website.PushNotifications
{
    public class PushNotificationSubscriptionManager : IPushNotificationSubscriptionManager
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISystemConfigurationProvider _systemConfigurationProvider;
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly ISession _session;
        private readonly Site _site;
        private readonly IUrlHelper _urlHelper;

        public PushNotificationSubscriptionManager(IConfigurationProvider configurationProvider, ISystemConfigurationProvider systemConfigurationProvider,
            IGetCurrentUser getCurrentUser, ISession session, Site site, IUrlHelper urlHelper)
        {
            _configurationProvider = configurationProvider;
            _systemConfigurationProvider = systemConfigurationProvider;
            _getCurrentUser = getCurrentUser;
            _session = session;
            _site = site;
            _urlHelper = urlHelper;
        }

        public PushSubscriptionResult CreateSubscription(PushNotificationSubscription subscription)
        {
            var pushSubscription = new Entities.People.PushSubscription()
            {
                AuthSecret = subscription.AuthSecret,
                Endpoint = subscription.Endpoint,
                Key = subscription.Key,
                User = _getCurrentUser.Get()
            };

            _session.Transact(session => session.Save(pushSubscription));

            WebPushSettings webPushSettings = GetWebpushSettings();
            var vapidDetails = new VapidDetails(webPushSettings.VapidSubject, webPushSettings.VapidPublicKey, webPushSettings.VapidPrivateKey);

            var webPushClient = new WebPushClient();
            webPushClient.SetVapidDetails(vapidDetails);
            var payload = new PushNotificationPayload
            {
                Title = _site.Name,
                Body = "Thank you for subscribing",
                Icon = EnsureAbsolute(webPushSettings.NotificationIcon),
                Badge = EnsureAbsolute(webPushSettings.NotificationBadge),
            };

            try
            {
                webPushClient.SendNotificationAsync(ToWebPushNotification(pushSubscription), JsonConvert.SerializeObject(payload), vapidDetails).GetAwaiter().GetResult();
            }
            catch (WebPushException exception)
            {
                var statusCode = exception.StatusCode;
                return new PushSubscriptionResult { StatusCode = statusCode };
            }

            return new PushSubscriptionResult();
        }

        private string EnsureAbsolute(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return _urlHelper.Content("~" + url);
            }

            return url;
        }

        public PushSubscriptionResult RemoveSubscription(string endpoint)
        {
            var pushSubscriptions = _session.Query<Entities.People.PushSubscription>().Where(x => x.Endpoint == endpoint).ToList();
            _session.Transact(session => pushSubscriptions.ForEach(session.Delete));

            return new PushSubscriptionResult();
        }

        private PushSubscription ToWebPushNotification(Entities.People.PushSubscription pushSubscription)
        {
            return new PushSubscription(pushSubscription.Endpoint, pushSubscription.Key, pushSubscription.AuthSecret);
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
});".Replace("PUBLIC_KEY", GetWebpushSettings().VapidPublicKey);
        }

        private WebPushSettings GetWebpushSettings()
        {
            var settings = _configurationProvider.GetSiteSettings<WebPushSettings>();

            if (string.IsNullOrWhiteSpace(settings.VapidPrivateKey))
            {
                var mailSettings = _systemConfigurationProvider.GetSystemSettings<MailSettings>();
                var keys = VapidHelper.GenerateVapidKeys();
                settings.VapidPrivateKey = keys.PrivateKey;
                settings.VapidPublicKey = keys.PublicKey;
                settings.VapidSubject = $"mailto:{mailSettings.SystemEmailAddress}";
                _configurationProvider.SaveSettings(settings);
            }
            return settings;
        }
    }
}