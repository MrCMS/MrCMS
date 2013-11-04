using System;
using System.ComponentModel;

namespace MrCMS.Models
{
    public class MessageQueueQuery
    {
        public MessageQueueQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        [DisplayName("Sent From")]
        public string FromQuery { get; set; }
        [DisplayName("Sent To")]
        public string ToQuery { get; set; }
    }
}