using MrCMS.Entities;
using NHibernate;

namespace MrCMS.Events
{
    public class OnUpdatingArgs
    {
        public SystemEntity Item { get; set; }
        public ISession Session { get; set; }
        public SystemEntity Original { get; set; }
    }
}