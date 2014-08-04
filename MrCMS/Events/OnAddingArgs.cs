using MrCMS.Entities;
using NHibernate;
using NHibernate.Engine;

namespace MrCMS.Events
{
    public class OnAddingArgs
    {
        public SystemEntity Item { get; set; }
        public ISession Session { get; set; }
    }
    public class OnUpdatingArgs
    {
        public SystemEntity Item { get; set; }
        public ISession Session { get; set; }
        public SystemEntity Original { get; set; }
    }
    public class OnAddedArgs
    {
        public SystemEntity Item { get; set; }
        public ISession Session { get; set; }
    }
    public class OnUpdatedArgs
    {
        public SystemEntity Item { get; set; }
        public ISession Session { get; set; }
        public SystemEntity Original { get; set; }
    }
}