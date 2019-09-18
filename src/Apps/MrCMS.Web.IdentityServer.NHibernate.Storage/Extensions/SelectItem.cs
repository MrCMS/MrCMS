using System;
using System.Collections.Generic;
using System.Text;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Extensions
{
    public class SelectItem
    {
        public SelectItem(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public string Id { get; set; }

        public string Text { get; set; }
    }
}
