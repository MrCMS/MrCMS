using System;
using System.Collections.Generic;

namespace MrCMS.Models
{
    public class ACLGroup
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public string AppName { get; set; }

        public List<ACLOperation> Operations { get; set; }
    }
}