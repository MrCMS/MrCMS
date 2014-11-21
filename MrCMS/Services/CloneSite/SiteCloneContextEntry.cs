using System;
using MrCMS.Entities;

namespace MrCMS.Services.CloneSite
{
    public class SiteCloneContextEntry
    {
        public Type Type { get; set; }
        public int Id { get; set; }

        public SystemEntity NewEntity { get; set; }

        public SystemEntity Original { get; set; }
    }
}