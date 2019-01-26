using System;

namespace MrCMS.Services.CloneSite
{
    public class CloneSitePartAttribute : Attribute
    {
        public int Order { get; set; }
        public string Name { get; set; }

        public CloneSitePartAttribute(int order)
        {
            Order = order;
        }
    }
}