using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class LayoutAdminService : ILayoutAdminService
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IGetDocumentsByParent<Layout> _getDocumentsByParent;
        private readonly IUrlValidationService _urlValidationService;

        public LayoutAdminService(IRepository<Layout> layoutRepository, IGetDocumentsByParent<Layout> getDocumentsByParent, IUrlValidationService urlValidationService)
        {
            _layoutRepository = layoutRepository;
            _getDocumentsByParent = getDocumentsByParent;
            _urlValidationService = urlValidationService;
        }

        public Layout GetAddLayoutModel(int? id)
        {
            return new Layout
            {
                Parent = id.HasValue ? _layoutRepository.Get(id.Value) : null
            };
        }

        public void Add(Layout layout)
        {
            _layoutRepository.Add(layout);
        }

        public void Update(Layout layout)
        {
            _layoutRepository.Update(layout);
        }

        public void Delete(Layout layout)
        {
            _layoutRepository.Delete(layout);
        }

        public List<SortItem> GetSortItems(Layout parent)
        {
            return _getDocumentsByParent.GetDocuments(parent)
                .Select(
                    arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                .OrderBy(x => x.Order)
                .ToList();
        }

        public void SetOrders(List<SortItem> items)
        {
            _layoutRepository.Transact(repository => items.ForEach(item =>
            {
                var mediaFile = repository.Get(item.Id);
                mediaFile.DisplayOrder = item.Order;
                repository.Update(mediaFile);
            }));
        }

        public bool UrlIsValidForLayout(string urlSegment, int? id)
        {
            return _urlValidationService.UrlIsValidForLayout(urlSegment, id);
        }

        public IEnumerable<SelectListItem> GetValidParents(Layout doc)
        {
            IList<Layout> potentialParents = _layoutRepository.Query().ToList();
            List<SelectListItem> result = potentialParents.Distinct()
                .Where(page => page.Id != doc.Id)
                .OrderBy(x => x.Name).
                BuildSelectItemList(page => page.Name, page => page.Id.ToString(),
                    webpage1 => doc.Parent != null && doc.ParentId == webpage1.Id, emptyItem: null);

            return result;
        }

        public void SetParent(Layout layout, int? parentId)
        {
            if (layout == null) return;

            Layout parent = parentId.HasValue ? _layoutRepository.Get(parentId.Value) : null;

            layout.Parent = parent;

            _layoutRepository.Update(layout);
        }
    }
}