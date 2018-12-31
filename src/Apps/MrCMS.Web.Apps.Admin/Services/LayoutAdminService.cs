using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class LayoutAdminService : ILayoutAdminService
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IGetDocumentsByParent<Layout> _getDocumentsByParent;
        private readonly IUrlValidationService _urlValidationService;
        private readonly IMapper _mapper;

        public LayoutAdminService(IRepository<Layout> layoutRepository, IGetDocumentsByParent<Layout> getDocumentsByParent, IUrlValidationService urlValidationService,
            IMapper mapper)
        {
            _layoutRepository = layoutRepository;
            _getDocumentsByParent = getDocumentsByParent;
            _urlValidationService = urlValidationService;
            _mapper = mapper;
        }

        public AddLayoutModel GetAddLayoutModel(int? id)
        {
            return new AddLayoutModel()
            {
                ParentId = id
            };
        }

        public Layout GetLayout(int? id) => id.HasValue ? _layoutRepository.Get(id.Value) : null;

        public Layout Add(AddLayoutModel model)
        {
            var layout = _mapper.Map<Layout>(model);
            _layoutRepository.Add(layout);
            return layout;
        }

        public UpdateLayoutModel GetEditModel(int id)
        {
            var layout = GetLayout(id);
            return _mapper.Map<UpdateLayoutModel>(layout);
        }

        public List<LayoutArea> GetLayoutAreas(int id)
        {
            return GetLayout(id).GetLayoutAreas().Distinct().ToList();
        }

        public void Update(UpdateLayoutModel model)
        {
            var layout = GetLayout(model.Id);
            _mapper.Map(model, layout);
            _layoutRepository.Update(layout);
        }

        public Layout Delete(int id)
        {
            var layout = GetLayout(id);
            _layoutRepository.Delete(layout);
            return layout;
        }

        public List<SortItem> GetSortItems(int? parent)
        {
            return _getDocumentsByParent.GetDocuments(GetLayout(parent))
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

        public IEnumerable<SelectListItem> GetValidParents(int id)
        {
            var layout = GetLayout(id);

            IList<Layout> potentialParents = _layoutRepository.Query().ToList();
            List<SelectListItem> result = potentialParents.Distinct()
                .Where(page => page.Id != layout.Id)
                .OrderBy(x => x.Name).
                BuildSelectItemList(page => page.Name, page => page.Id.ToString(),
                    webpage1 => layout.Parent != null && layout.ParentId == webpage1.Id, emptyItem: null);

            return result.Prepend(new SelectListItem { Value = string.Empty, Text = "Root", Selected = layout.Parent == null });
        }

        public void SetParent(int id, int? parentId)
        {
            var layout = GetLayout(id);
            if (layout == null) return;

            Layout parent = parentId.HasValue ? _layoutRepository.Get(parentId.Value) : null;

            layout.Parent = parent;

            _layoutRepository.Update(layout);
        }
    }
}