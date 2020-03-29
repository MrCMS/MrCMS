using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class MergeWebpageAdminService : IMergeWebpageAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IStringResourceProvider _resourceProvider;
        private readonly IWebpageUrlService _webpageUrlService;
        private readonly ICreateMergeBatch _createMergeBatch;
        private readonly IActivePagesLoader _activePagesLoader;

        public MergeWebpageAdminService(IRepository<Webpage> webpageRepository, IStringResourceProvider resourceProvider, IWebpageUrlService webpageUrlService, ICreateMergeBatch createMergeBatch, IActivePagesLoader activePagesLoader)
        {
            _webpageRepository = webpageRepository;
            _resourceProvider = resourceProvider;
            _webpageUrlService = webpageUrlService;
            _createMergeBatch = createMergeBatch;
            _activePagesLoader = activePagesLoader;
        }
        public async Task<IEnumerable<SelectListItem>> GetValidParents(Webpage webpage)
        {
            var children = _webpageRepository.Query().Where(x => x.Parent.Id == webpage.Id).ToList();
            // get the matching pages for all types
            var potentialParents = new HashSet<IEnumerable<Webpage>>();
            ;
            foreach (var child in children)
            {
                var validParentWebpages = await GetValidParentWebpages(child);
                potentialParents.Add(validParentWebpages);
            }

            var validForAll = potentialParents.SelectMany(x => x).Distinct()
                .Where(x => x != webpage && potentialParents.All(y => y.Contains(x))).ToHashSet();

            var items = validForAll.BuildSelectItemList(
                page => string.Format("{0} ({1})", page.Name, page.GetMetadata().Name),
                page => page.Id.ToString(),
                webpage1 => webpage.Parent != null && webpage.ParentId == webpage1.Id, emptyItem: null);

            return items;
        }

        private async Task<IOrderedEnumerable<Webpage>> GetValidParentWebpages(Webpage webpage)
        {
            List<DocumentMetadata> validParentTypes = DocumentMetadataHelper.GetValidParentTypes(webpage);

            List<string> validParentTypeNames =
                validParentTypes.Select(documentMetadata => documentMetadata.Type.FullName).ToList();
            IList<Webpage> potentialParents =
                _webpageRepository.Query()
                    .Where(page => validParentTypeNames.Contains(page.DocumentClrType))
                    .ToList();

            var validParentWebpages = new List<Webpage>();
            foreach (var potentialParent in potentialParents)
            {
                var activePages = await _activePagesLoader.GetActivePages(potentialParent);
                if (activePages.Any(x => x.Id == potentialParent.Id))
                    validParentWebpages.Add(potentialParent);
            }
            return validParentWebpages.OrderBy(x => x.Name);
        }

        public async Task<MergeWebpageResult> Validate(MergeWebpageModel moveWebpageModel)
        {
            var webpage = GetWebpage(moveWebpageModel);
            var parent = GetParent(moveWebpageModel);

            var validParentWebpages = await GetValidParentWebpages(webpage);
            var valid = webpage != null && parent != null && validParentWebpages.Contains(parent);

            return new MergeWebpageResult
            {
                Success = valid,
                Message = valid ? string.Empty : _resourceProvider.GetValue("Sorry, but you can't select that as a parent for this page.")
            };
        }

        private Webpage GetParent(MergeWebpageModel moveWebpageModel)
        {
            return _webpageRepository.LoadSync(moveWebpageModel.MergeIntoId);
        }

        private Webpage GetWebpage(MergeWebpageModel moveWebpageModel)
        {
            return _webpageRepository.LoadSync(moveWebpageModel.Id);
        }

        public async Task<MergeWebpageConfirmationModel> GetConfirmationModel(MergeWebpageModel model)
        {
            var webpage = GetWebpage(model);
            var parent = GetParent(model);


            return new MergeWebpageConfirmationModel
            {
                Webpage = webpage,
                MergedInto = parent,
                ChangedPages = await GetChangedPages(model, webpage, parent)
            };
        }
        private async Task<List<MergeWebpageChangedPageModel>> GetChangedPages(MergeWebpageModel model, Webpage webpage, Webpage parent)
        {
            var webpageHierarchy = GetWebpageHierarchy(webpage).ToList();

            var parentActivePages = await _activePagesLoader.GetActivePages(parent);
            parentActivePages.Reverse();
            List<MergeWebpageChangedPageModel> models = new List<MergeWebpageChangedPageModel>();
            foreach (var page in webpageHierarchy)
            {
                var activePages = await _activePagesLoader.GetActivePages(page);
                var indexOf = activePages.IndexOf(webpage);
                var childActivePages = activePages.Take(indexOf).ToList();
                activePages.Reverse();
                var immediateParent = childActivePages.ElementAtOrDefault(1);
                childActivePages.Reverse();
                var newUrl = GetNewUrl(model, parent, page, immediateParent, models.FirstOrDefault(x => x.Id == immediateParent?.Id));
                models.Add(new MergeWebpageChangedPageModel
                {
                    Id = page.Id,
                    ParentId = immediateParent?.Id ?? model.Id,
                    OldUrl = page.UrlSegment,
                    NewUrl = await newUrl,
                    OldHierarchy = GetHierarchy(activePages),
                    NewHierarchy = GetHierarchy(parentActivePages.Concat(childActivePages))
                });
            }
            return models;
        }

        private async Task<string> GetNewUrl(MergeWebpageModel model, Webpage parent, Webpage page, Webpage immediateParent, MergeWebpageChangedPageModel parentModel)
        {
            if (!model.UpdateUrls)
            {
                return page.UrlSegment;
            }

            if (immediateParent == null)
            {
                return await _webpageUrlService.Suggest(new SuggestParams
                {
                    DocumentType = page.DocumentClrType,
                    PageName = page.Name,
                    Template = page.PageTemplate?.Id,
                    UseHierarchy = true,
                    WebpageId = page.Id,
                    ParentId = parent?.Id
                });
            }

            return await _webpageUrlService.Suggest(new SuggestParams
            {
                DocumentType = page.DocumentClrType,
                PageName = $"{parentModel.NewUrl}/{page.Name}",
                Template = page.PageTemplate?.Id,
                UseHierarchy = false,
                WebpageId = page.Id,
                ParentId = immediateParent.Id
            });
        }

        private string GetHierarchy(IEnumerable<Webpage> webpages)
        {
            return string.Join(" > ", webpages.Select(x => x.Name));
        }
        private IEnumerable<Webpage> GetWebpageHierarchy(Webpage webpage)
        {
            var descendants = _webpageRepository.Query().Where(x => x.Parent.Id == webpage.Id).OrderBy(x => x.DisplayOrder)
                .ToList();
            foreach (var descendant in descendants)
            {
                yield return descendant;
                foreach (var child in GetWebpageHierarchy(descendant))
                {
                    yield return child;
                }
            }
        }


        public async Task<MergeWebpageResult> Confirm(MergeWebpageModel model)
        {
            var confirmationModel = await GetConfirmationModel(model);

            var success = await _createMergeBatch.CreateBatch(confirmationModel);

            return new MergeWebpageResult
            {
                Success = success,
                Message = _resourceProvider.GetValue(success
                    ? "The page has been moved successfully"
                    : "There was an issue creating the batch to complete the merge")
            };
        }

        public MergeWebpageModel GetModel(Webpage webpage)
        {
            return new MergeWebpageModel
            {
                Id = webpage.Id
            };
        }
    }
}