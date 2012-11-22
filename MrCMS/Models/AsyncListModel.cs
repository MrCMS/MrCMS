using MrCMS.Paging;

namespace MrCMS.Models
{
    public abstract class AsyncListModel<T>
    {
        private readonly PagedList<T> _items;
        private readonly int _id;

        protected AsyncListModel(PagedList<T> items, int id)
        {
            _items = items;
            _id = id;
        }

        public PagedList<T> Items
        {
            get { return _items; }
        }

        public int Id
        {
            get { return _id; }
        }
    }
}