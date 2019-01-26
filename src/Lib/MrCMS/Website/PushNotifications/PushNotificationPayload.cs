using Newtonsoft.Json;

namespace MrCMS.Website.PushNotifications
{
    public class PushNotificationPayload
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("badge")]
        public string Badge { get; set; }

        [JsonProperty("actionUrl")]
        public string ActionUrl { get; set; }
    }
}