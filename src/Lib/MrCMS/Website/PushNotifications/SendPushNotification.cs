using Microsoft.AspNetCore.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using Newtonsoft.Json;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WebPush;
using PushSubscription = MrCMS.Entities.People.PushSubscription;

namespace MrCMS.Website.PushNotifications
{
    public class SendPushNotification : ISendPushNotification
    {
        private readonly WebPushSettings _settings;
        private readonly IUrlHelper _urlHelper;
        private readonly ISession _session;
        private readonly ICreateBatch _createBatch;
        private readonly IControlBatchRun _controlBatchRun;

        public SendPushNotification(IUrlHelper urlHelper, IGetWebPushSettings getSettings, ISession session, ICreateBatch createBatch, IControlBatchRun controlBatchRun)
        {
            _urlHelper = urlHelper;
            _session = session;
            _createBatch = createBatch;
            _controlBatchRun = controlBatchRun;
            _settings = getSettings.GetSettings();
        }

        public WebPushResult SendNotification(PushSubscription subscription, PushNotification notification)
        {
            var vapidDetails = new VapidDetails(_settings.VapidSubject, _settings.VapidPublicKey,
                _settings.VapidPrivateKey);

            var webPushClient = new WebPushClient();
            webPushClient.SetVapidDetails(_settings.VapidSubject, _settings.VapidPublicKey,
                _settings.VapidPrivateKey);

            try
            {
                webPushClient.SendNotification(ToWebPushNotification(subscription),
                    JsonConvert.SerializeObject(GetPayload(notification)), vapidDetails);

                if (_settings.LogNotifications)
                {
                    _session.Transact(session => session.Save(new PushNotificationLog
                    {
                        PushNotification = notification,
                        PushSubscription = subscription
                    }));
                }
            }
            catch (WebPushException exception)
            {
                var statusCode = exception.StatusCode;
                return new WebPushResult { StatusCode = statusCode };
            }

            return new WebPushResult();
        }

        public WebPushResult SendNotification(SendPushNotificationData data)
        {
            PushSubscription subscription = _session.Get<PushSubscription>(data.SubscriptionId);
            PushNotification notification = _session.Get<PushNotification>(data.NotificationId);

            if (subscription == null || notification == null)
            {
                return new WebPushResult { StatusCode = HttpStatusCode.BadRequest };
            }

            return SendNotification(subscription, notification);
        }

        public WebPushResult SendNotification(PushSubscription subscription, string body,
            string url = null, string title = null,
            string icon = null, string badge = null)
        {
            var payload = GetPayload(body, url, title, icon, badge);

            var notification = CreatePushNotificationEntity(payload);

            return SendNotification(subscription, notification);
        }

        private PushNotification CreatePushNotificationEntity(PushNotificationPayload payload)
        {
            var notification = GetNotification(payload);
            _session.Transact(session => session.Save(notification));
            return notification;
        }

        private PushNotification GetNotification(PushNotificationPayload payload)
        {
            return new PushNotification
            {
                ActionUrl = payload.ActionUrl,
                Badge = payload.Badge,
                Body = payload.Body,
                Icon = payload.Icon,
                Title = payload.Title
            };
        }
        private PushNotificationPayload GetPayload(PushNotification notification)
        {
            return GetPayload(notification.Body, notification.ActionUrl, notification.Title, notification.Icon,
                notification.Badge);
        }

        public BatchRun SendNotificationToSelection(List<PushSubscription> subscriptions, string body, string url = null, string title = null,
            string icon = null, string badge = null)
        {
            var payload = GetPayload(body, url, title, icon, badge);

            var pushNotification = CreatePushNotificationEntity(payload);

            var jobs = subscriptions.Select(subscription => new SendPushNotificationBatchJob
            {
                Data = JsonConvert.SerializeObject(new SendPushNotificationData
                { SubscriptionId = subscription.Id, NotificationId = pushNotification.Id })
            });

            var result = _createBatch.Create(jobs);

            _controlBatchRun.Start(result.InitialBatchRun);

            return result.InitialBatchRun;
        }

        public BatchRun SendNotificationToAll(string body, string url = null, string title = null, string icon = null,
            string badge = null)
        {
            return SendNotificationToSelection(_session.Query<PushSubscription>().ToList(), body, url, title, icon, badge);
        }

        private PushNotificationPayload GetPayload(string body, string url, string title, string icon, string badge)
        {
            return new PushNotificationPayload
            {
                Body = body,
                Title = title ?? _settings.DefaultNotificationTitle,
                ActionUrl = url,
                Icon = EnsureAbsolute(icon ?? _settings.DefaultNotificationIcon),
                Badge = EnsureAbsolute(badge ?? _settings.DefaultNotificationBadge)
            };
        }

        private string EnsureAbsolute(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                var request = _urlHelper.ActionContext.HttpContext.Request;
                var uriBuilder = new UriBuilder { Scheme = request.Scheme, Host = request.Host.Host, Path = url };
                if (request.Host.Port.HasValue)
                {
                    uriBuilder.Port = request.Host.Port.Value;
                }

                return uriBuilder.ToString();
            }

            return url;
        }

        private WebPush.PushSubscription ToWebPushNotification(PushSubscription pushSubscription)
        {
            return new WebPush.PushSubscription(pushSubscription.Endpoint, pushSubscription.Key,
                pushSubscription.AuthSecret);
        }
    }
}