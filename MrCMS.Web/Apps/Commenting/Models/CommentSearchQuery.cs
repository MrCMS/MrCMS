using System;
using System.ComponentModel;

namespace MrCMS.Web.Apps.Commenting.Models
{
    public class CommentSearchQuery
    {
        public CommentSearchQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }

        [DisplayName("Approval Status")]
        public ApprovalStatus ApprovalStatus { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }

        [DisplayName("Date From")]
        public DateTime? DateFrom { get; set; }
        [DisplayName("Date To")]
        public DateTime? DateTo { get; set; }
    }
}