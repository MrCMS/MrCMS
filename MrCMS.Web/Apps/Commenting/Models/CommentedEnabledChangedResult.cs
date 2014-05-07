using System;
using System.Collections.Generic;

namespace MrCMS.Web.Apps.Commenting.Models
{
    public class CommentedEnabledChangedResult
    {
        public CommentedEnabledChangedResult()
        {
            Added = new List<Type>();
            Removed = new List<Type>();
        }
        public List<Type> Added { get; set; }
        public List<Type> Removed { get; set; }
    }
}