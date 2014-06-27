using MrCMS.Paging;

namespace MrCMS.Web.Areas.Admin.Models
{
    public abstract class AsyncListModel<T>
    {
        private readonly IPagedList<T> _items;
        private readonly int _id;

        protected AsyncListModel(IPagedList<T> items, int id)
        {
            _items = items;
            _id = id;
        }

        public IPagedList<T> Items
        {
            get { return _items; }
        }

        public int Id
        {
            get { return _id; }
        }
    }
}