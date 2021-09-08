using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Installation.Services;
using MrCMS.Website.PushNotifications;

namespace MrCMS.Website.Controllers
{
    public class PushNotificationSubscriptionsController : MrCMSUIController
    {
        private readonly IServiceProvider _serviceProvider;

        public PushNotificationSubscriptionsController(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        public IPushNotificationSubscriptionManager Manager
        {
            get
            {
                var installed = _serviceProvider.GetRequiredService<IDatabaseCreationService>().IsDatabaseInstalled();
                if (!installed)
                    return NullManager;
                return _serviceProvider.GetRequiredService<IPushNotificationSubscriptionManager>();
            }
        }

        private static readonly IPushNotificationSubscriptionManager NullManager = new NullPushNotificationSubscriptionManager();

        private class NullPushNotificationSubscriptionManager : IPushNotificationSubscriptionManager
        {
            public async Task<WebPushResult> CreateOrUpdateSubscription(PushNotificationSubscription subscription)
            {
                return new WebPushResult();
            }

            public async Task<WebPushResult> RemoveSubscription(string endpoint)
            {
                return new WebPushResult();
            }

            public async Task<string> GetServiceWorkerJavaScript()
            {
                return string.Empty;
            }
        }

        [HttpGet("sw.js")]
        [Route("sw.js")]
        public async Task<ContentResult> ServiceWorkerJavaScript()
        {
            return Content(await Manager.GetServiceWorkerJavaScript(), "text/javascript");
        }

        [HttpPost("push-notifications")]
        public async Task<IActionResult> Create([FromBody] PushNotificationSubscription subscription)
        {
            var result = await Manager.CreateOrUpdateSubscription(subscription);

            return GetResult(result);
        }

        [HttpDelete("push-notifications")]
        public async Task<IActionResult> Delete(string endpoint)
        {
            var result = await Manager.RemoveSubscription(endpoint);

            return GetResult(result);
        }

        private static IActionResult GetResult(WebPushResult result)
        {
            return new StatusCodeResult((int) result.StatusCode.GetValueOrDefault(HttpStatusCode.OK));
        }
    }
}