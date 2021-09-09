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
                    return null;
                return _serviceProvider.GetRequiredService<IPushNotificationSubscriptionManager>();
            }
        }

        [HttpGet("sw.js")]
        [Route("sw.js")]
        public async Task<ContentResult> ServiceWorkerJavaScript()
        {
            if (Manager == null)
                return Content(string.Empty, "text/javascript");

            return Content(await (Manager.GetServiceWorkerJavaScript()), "text/javascript");
        }

        [HttpPost("push-notifications")]
        public async Task<IActionResult> Create([FromBody] PushNotificationSubscription subscription)
        {
            if (Manager == null)
                return new EmptyResult();

            var result = await Manager.CreateOrUpdateSubscription(subscription);

            return GetResult(result);
        }

        [HttpDelete("push-notifications")]
        public async Task<IActionResult> Delete(string endpoint)
        {
            if (Manager == null)
                return new EmptyResult();
            
            var result = await Manager.RemoveSubscription(endpoint);

            return GetResult(result);
        }

        private static IActionResult GetResult(WebPushResult result)
        {
            return new StatusCodeResult((int)result.StatusCode.GetValueOrDefault(HttpStatusCode.OK));
        }
    }
}