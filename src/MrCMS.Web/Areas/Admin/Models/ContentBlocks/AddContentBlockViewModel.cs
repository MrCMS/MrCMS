using System;
using System.ComponentModel.DataAnnotations;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Models.ContentBlocks
{
    public class AddContentBlockViewModel
    {
        public int WebpageId { get; set; }
        [Required]
        public string Name { get; set; }
        public string BlockType { get; set; }

        public Type ClrType => TypeHelper.GetTypeByName(BlockType);

        public string TypeDisplayName => ClrType?.Name.BreakUpString();
    }
}