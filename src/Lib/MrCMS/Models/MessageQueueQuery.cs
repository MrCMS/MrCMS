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
        [DisplayName("Date From")]
        public DateTime? From { get; set; }
        [DisplayName("Date To")]
        public DateTime? To { get; set; }
        [DisplayName("Email From")]
        public string FromQuery { get; set; }
        [DisplayName("Email To")]
        public string ToQuery { get; set; }

        public string Subject { get; set; }
    }
}