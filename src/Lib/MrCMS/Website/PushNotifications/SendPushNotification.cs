using Microsoft.AspNetCore.Mvc;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using WebPush;
using PushSubscription = MrCMS.Entities.People.PushSubscription;

namespace MrCMS.Website.PushNotifications
{
    public class SendPushNotification : ISendPushNotification
    {
        private readonly WebPushSettings _settings;
        private readonly IRepository<PushSubscription> _pushSubscriptionRepository;
        private readonly IRepository<PushNotification> _pushNotificationRepository;
        private readonly IRepository<PushNotificationLog> _pushNotificationLogRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly ICreateBatch _createBatch;
        private readonly IControlBatchRun _controlBatchRun;

        public SendPushNotification(
            IRepository<PushSubscription> pushSubscriptionRepository,
            IRepository<PushNotification> pushNotificationRepository,
            IRepository<PushNotificationLog> pushNotificationLogRepository,
            IUrlHelper urlHelper, IGetWebPushSettings getSettings, ICreateBatch createBatch, IControlBatchRun controlBatchRun)
        {
            _pushSubscriptionRepository = pushSubscriptionRepository;
            _pushNotificationRepository = pushNotificationRepository;
            _pushNotificationLogRepository = pushNotificationLogRepository;
            _urlHelper = urlHelper;
            _createBatch = createBatch;
            _controlBatchRun = controlBatchRun;
            _settings = getSettings.GetSettings();
        }

        public async Task<WebPushResult> SendNotification(PushSubscription subscription, PushNotification notification)
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
                    await _pushNotificationLogRepository.Add(new PushNotificationLog
                    {
                        PushNotification = notification,
                        PushSubscription = subscription
                    });
                }
            }
            catch (WebPushException exception)
            {
                var statusCode = exception.StatusCode;
                return new WebPushResult { StatusCode = statusCode };
            }

            return new WebPushResult();
        }

        public async Task<WebPushResult> SendNotification(SendPushNotificationData data)
        {
            PushSubscription subscription = await _pushSubscriptionRepository.Load(data.SubscriptionId);
            PushNotification notification = await _pushNotificationRepository.Load(data.NotificationId);

            if (subscription == null || notification == null)
            {
                return new WebPushResult { StatusCode = HttpStatusCode.BadRequest };
            }

            return await SendNotification(subscription, notification);
        }

        public async Task<WebPushResult> SendNotification(PushSubscription subscription, string body,
            string url = null, string title = null,
            string icon = null, string badge = null)
        {
            var payload = GetPayload(body, url, title, icon, badge);

            var notification = await CreatePushNotificationEntity(payload);

            return await SendNotification(subscription, notification);
        }

        private async Task<PushNotification> CreatePushNotificationEntity(PushNotificationPayload payload)
        {
            var notification = GetNotification(payload);
            await _pushNotificationRepository.Add(notification);
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

        public async Task<BatchRun> SendNotificationToSelection(List<PushSubscription> subscriptions, string body,
            string url = null, string title = null,
            string icon = null, string badge = null)
        {
            var payload = GetPayload(body, url, title, icon, badge);

            var pushNotification = CreatePushNotificationEntity(payload);

            var jobs = subscriptions.Select(subscription => new SendPushNotificationBatchJob
            {
                Data = JsonConvert.SerializeObject(new SendPushNotificationData
                { SubscriptionId = subscription.Id, NotificationId = pushNotification.Id })
            });

            var result = await _createBatch.Create(jobs);

            await _controlBatchRun.Start(result.InitialBatchRun);

            return result.InitialBatchRun;
        }

        public async Task<BatchRun> SendNotificationToAll(string body, string url = null, string title = null,
            string icon = null,
            string badge = null)
        {
            return await SendNotificationToSelection(await _pushSubscriptionRepository.Query<PushSubscription>().ToListAsync(), body, url, title, icon, badge);
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