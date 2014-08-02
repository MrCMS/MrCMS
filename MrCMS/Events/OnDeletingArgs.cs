using MrCMS.Entities;
using NHibernate;

namespace MrCMS.Events
{
    public class OnDeletingArgs
    {
        public SystemEntity Item { get; set; }
        public ISession Session { get; set; }
    }
    public class OnDeletedArgs
    {
        public SystemEntity Item { get; set; }
        public ISession Session { get; set; }
    }
}