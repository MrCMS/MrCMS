using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class LayoutAdminService : ILayoutAdminService
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IGetDocumentsByParent<Layout> _getDocumentsByParent;
        private readonly IUrlValidationService _urlValidationService;
        private readonly ILayoutAreaLoader _layoutAreaLoader;
        private readonly IMapper _mapper;

        public LayoutAdminService(IRepository<Layout> layoutRepository, IGetDocumentsByParent<Layout> getDocumentsByParent, IUrlValidationService urlValidationService,
            ILayoutAreaLoader layoutAreaLoader,
            IMapper mapper)
        {
            _layoutRepository = layoutRepository;
            _getDocumentsByParent = getDocumentsByParent;
            _urlValidationService = urlValidationService;
            _layoutAreaLoader = layoutAreaLoader;
            _mapper = mapper;
        }

        public AddLayoutModel GetAddLayoutModel(int? id)
        {
            return new AddLayoutModel()
            {
                ParentId = id
            };
        }

        public Layout GetLayout(int? id) => id.HasValue ? _layoutRepository.LoadSync(id.Value) : null;

        public async Task<Layout> Add(AddLayoutModel model)
        {
            var layout = _mapper.Map<Layout>(model);
            await _layoutRepository.Add(layout);
            return layout;
        }

        public UpdateLayoutModel GetEditModel(int id)
        {
            var layout = GetLayout(id);
            return _mapper.Map<UpdateLayoutModel>(layout);
        }

        public async Task<List<LayoutArea>> GetLayoutAreas(int id)
        {
            return (await _layoutAreaLoader.GetLayoutAreas(id)).Distinct().ToList();
        }

        public async Task Update(UpdateLayoutModel model)
        {
            var layout = GetLayout(model.Id);
            _mapper.Map(model, layout);
            await _layoutRepository.Update(layout);
        }

        public async Task<Layout> Delete(int id)
        {
            var layout = GetLayout(id);
            await _layoutRepository.Delete(layout);
            return layout;
        }

        public async Task<List<SortItem>> GetSortItems(int? parent)
        {
            return (await _getDocumentsByParent.GetDocuments(GetLayout(parent)))
                .Select(
                    arg => new SortItem { Order = arg.DisplayOrder, Id = arg.Id, Name = arg.Name })
                .OrderBy(x => x.Order)
                .ToList();
        }

        public async Task SetOrders(List<SortItem> items)
        {
            var layouts = items.Select(item =>
           {
               var layout = _layoutRepository.LoadSync(item.Id);
               layout.DisplayOrder = item.Order;
               return layout;
           }).ToList();

            await _layoutRepository.UpdateRange(layouts);
        }

        public async Task<bool> UrlIsValidForLayout(string urlSegment, int? id)
        {
            return await _urlValidationService.UrlIsValidForLayout(urlSegment, id);
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

        public async Task SetParent(int id, int? parentId)
        {
            var layout = GetLayout(id);
            if (layout == null) return;

            layout.ParentId = parentId;

            await _layoutRepository.Update(layout);
        }
    }
}