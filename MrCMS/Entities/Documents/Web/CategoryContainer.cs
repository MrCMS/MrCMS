using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Paging;

namespace MrCMS.Entities.Documents.Web
{
    [DoNotMap]
    public class CategoryContainer<T> : Webpage, IDocumentContainer<T> where T : IContainerItem
    {
        private readonly IDocumentContainer<T> _container;
        private readonly string _category;
        private readonly int _page;
        private string _documentType;

        public override string DocumentType
        {
            get { return _documentType; }
        }

        public CategoryContainer(IDocumentContainer<T> container, string category, int page)
        {
            // This is required to make the layout work correctly (i.e. the type needs to be a webpage
            var propertyInfos = typeof(Webpage).GetProperties().Where(info => info.CanWrite).ToList();
            foreach (var source in propertyInfos)
            {
                source.SetValue(this, source.GetValue(container, null), null);
            }
            _container = container;
            _category = category;
            _page = page;
            _documentType = container.DocumentType;
        }

        public IPagedList<T> ChildItemsPaged
        {
            get
            {
                var blogPages = ChildItemsToShow;
                return blogPages.Any()
                           ? (IPagedList<T>) new PagedList<T>(blogPages, Page,
                                                              _container.AllowPaging
                                                                  ? _container.PageSize
                                                                  : blogPages.Count())
                           : new StaticPagedList<T>(new List<T>(), 1, 1, 1);
            }
        }

        public IEnumerable<T> ChildItemsToShow
        {
            get
            {
                return Category == null
                           ? _container.ChildItems
                           : _container.ChildItems.Where(
                               page =>
                               page.Tags.Select(tag => tag.Name).Contains(Category,
                                                                          StringComparer.OrdinalIgnoreCase));
            }
        }

        public string Category
        {
            get { return _category; }
        }

        private int Page
        {
            get { return _page; }
        }

        public int PageSize { get { return _container.PageSize; } }
        public bool AllowPaging { get { return _container.AllowPaging; } }

        public IEnumerable<T> ChildItems { get { return _container.ChildItems; } }
    }
}