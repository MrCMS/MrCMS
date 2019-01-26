using System.Collections.Generic;

namespace MrCMS.ACL
{
    public abstract class ACLRule
    {
        public string Name => GetType().FullName;

        public abstract string DisplayName { get; }
    }
}