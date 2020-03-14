using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.DbConfiguration;
using MrCMS.Website.PushNotifications;

namespace MrCMS.Website.Controllers
{
    public class PushNotificationSubscriptionsController : MrCMSUIController
    {
        private readonly IServiceProvider _serviceProvider;

        public PushNotificationSubscriptionsController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        [HttpGet("sw.js")]
        public async Task<IActionResult> ServiceWorkerJavaScript()
        {
            if (Manager == null)
                return BadRequest();
            return Content(await Manager.GetServiceWorkerJavaScript() ?? "", "text/javascript");
        }

        [HttpPost("push-notifications")]
        public async Task<IActionResult> Create([FromBody] PushNotificationSubscription subscription)
        {
            if (Manager == null)
                return BadRequest();
            var result = await Manager.CreateSubscription(subscription);

            return GetResult(result);
        }

        [HttpDelete("push-notifications")]
        public async Task<IActionResult> Delete(string endpoint)
        {
            if (Manager == null)
                return BadRequest();
            var result = await Manager.RemoveSubscription(endpoint);

            return GetResult(result);
        }


        private IPushNotificationSubscriptionManager Manager =>
            _serviceProvider.GetRequiredService<ICheckInstallationStatus>().IsInstalled()
                ? _serviceProvider.GetRequiredService<IPushNotificationSubscriptionManager>()
                : null;

        //private class PreInstallSubscriptionsManager : IPushNotificationSubscriptionManager
        //{
        //    public Task<WebPushResult> CreateSubscription(PushNotificationSubscription subscription)
        //    {
        //        return Task.FromResult<WebPushResult>(null);
        //    }

        //    public Task<WebPushResult> RemoveSubscription(string endpoint)
        //    {
        //        return Task.FromResult<WebPushResult>(null);
        //    }

        //    public string GetServiceWorkerJavaScript()
        //    {
        //        return string.Empty;
        //    }
        //}

        private static IActionResult GetResult(WebPushResult result)
        {
            return new StatusCodeResult((int?)result?.StatusCode ?? (int)HttpStatusCode.OK);
        }
    }
}