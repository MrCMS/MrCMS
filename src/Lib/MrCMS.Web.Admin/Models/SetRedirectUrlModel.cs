using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Admin.Models
{
    public class SetRedirectUrlModel
    {
        public int Id { get; set; }

        [Required]
        [Remote("CheckRedirectUrl", "Redirects")]
        public string Url { get; set; }
    }
}