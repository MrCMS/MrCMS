using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class MoveWebpageAdminService : IMoveWebpageAdminService
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IStringResourceProvider _resourceProvider;
        private readonly IWebpageUrlService _webpageUrlService;
        private readonly ICreateUpdateUrlBatch _createUpdateUrlBatch;

        public MoveWebpageAdminService(IRepository<Webpage> webpageRepository, IStringResourceProvider resourceProvider, IWebpageUrlService webpageUrlService, ICreateUpdateUrlBatch createUpdateUrlBatch)
        {
            _webpageRepository = webpageRepository;
            _resourceProvider = resourceProvider;
            _webpageUrlService = webpageUrlService;
            _createUpdateUrlBatch = createUpdateUrlBatch;
        }

        public IEnumerable<SelectListItem> GetValidParents(Webpage webpage)
        {
            var webpages = GetValidParentWebpages(webpage);
            List<SelectListItem> result = webpages
                .BuildSelectItemList(page => string.Format("{0} ({1})", page.Name, page.GetMetadata().Name),
                    page => page.Id.ToString(),
                    webpage1 => webpage.Parent != null && webpage.ParentId == webpage1.Id, emptyItem: null);

            if (IsRootAllowed(webpage))
            {
                result.Insert(0, SelectListItemHelper.EmptyItem("Root"));
            }

            return result;
        }

        private IOrderedEnumerable<Webpage> GetValidParentWebpages(Webpage webpage)
        {
            List<DocumentMetadata> validParentTypes = DocumentMetadataHelper.GetValidParentTypes(webpage);

            List<string> validParentTypeNames =
                validParentTypes.Select(documentMetadata => documentMetadata.Type.FullName).ToList();
            IList<Webpage> potentialParents =
                _webpageRepository.Query()
                    .Where(page => validParentTypeNames.Contains(page.DocumentType))
                    .ToList();

            var webpages = potentialParents.Distinct()
                .Where(page => !page.ActivePages.Contains(webpage))
                .OrderBy(x => x.Name);
            return webpages;
        }

        private bool SetParent(Webpage webpage, Webpage parent)
        {
            if (webpage == null)
            {
                return false;
            }

            webpage.Parent = parent;

            _webpageRepository.Update(webpage);
            return true;
        }

        public MoveWebpageResult Validate(MoveWebpageModel moveWebpageModel)
        {
            var webpage = GetWebpage(moveWebpageModel);
            var parent = GetParent(moveWebpageModel);

            if (parent == webpage.Parent.Unproxy())
            {
                return new MoveWebpageResult
                {
                    Success = false,
                    Message = _resourceProvider.GetValue("The webpage already has this parent")
                };
            }

            var validParentWebpages = GetValidParentWebpages(webpage);
            var valid = parent == null ? IsRootAllowed(webpage) : validParentWebpages.Contains(parent);

            return new MoveWebpageResult
            {
                Success = valid,
                Message = valid ? string.Empty : _resourceProvider.GetValue("Sorry, but you can't select that as a parent for this page.")
            };
        }

        private Webpage GetParent(MoveWebpageModel moveWebpageModel)
        {
            return moveWebpageModel.ParentId.HasValue
                ? _webpageRepository.Get(moveWebpageModel.ParentId.Value)
                : null;
        }

        private Webpage GetWebpage(MoveWebpageModel moveWebpageModel)
        {
            return _webpageRepository.Get(moveWebpageModel.Id);
        }

        private bool IsRootAllowed(Webpage webpage)
        {
            return !webpage.GetMetadata().RequiresParent;
        }

        public MoveWebpageConfirmationModel GetConfirmationModel(MoveWebpageModel model)
        {
            var webpage = GetWebpage(model);
            var parent = GetParent(model);


            return new MoveWebpageConfirmationModel
            {
                Webpage = webpage,
                Parent = parent,
                ChangedPages = GetChangedPages(model, webpage, parent)
            };
        }

        private List<MoveWebpageChangedPageModel> GetChangedPages(MoveWebpageModel model, Webpage webpage, Webpage parent)
        {
            var webpageHierarchy = GetWebpageHierarchy(webpage).ToList();

            var parentActivePages = (parent?.ActivePages.Reverse() ?? Enumerable.Empty<Webpage>()).ToList();
            List<MoveWebpageChangedPageModel> models = new List<MoveWebpageChangedPageModel>();
            foreach (var page in webpageHierarchy)
            {
                var activePages = page.ActivePages.ToList();
                var indexOf = activePages.IndexOf(webpage);
                var childActivePages = activePages.Take(indexOf + 1).ToList();
                activePages.Reverse();
                var immediateParent = childActivePages.ElementAtOrDefault(1);
                childActivePages.Reverse();
                var newUrl = GetNewUrl(model, parent, page, immediateParent, models.FirstOrDefault(x => x.Id == immediateParent?.Id));
                models.Add(new MoveWebpageChangedPageModel
                {
                    Id = page.Id,
                    ParentId = immediateParent?.Id,
                    OldUrl = page.UrlSegment,
                    NewUrl = newUrl,
                    OldHierarchy = GetHierarchy(activePages),
                    NewHierarchy = GetHierarchy(parentActivePages.Concat(childActivePages))
                });
            }
            return models;
        }

        private string GetNewUrl(MoveWebpageModel model, Webpage parent, Webpage page, Webpage immediateParent,
            MoveWebpageChangedPageModel parentModel)
        {
            if (!model.UpdateUrls)
            {
                return page.UrlSegment;
            }

            if (immediateParent == null)
            {
                return _webpageUrlService.Suggest(new SuggestParams
                {
                    DocumentType = page.DocumentType,
                    PageName = page.Name,
                    Template = page.PageTemplate?.Id,
                    UseHierarchy = true,
                    ParentId = parent?.Id,
                    WebpageId = page.Id
                });
            }

            return _webpageUrlService.Suggest(
                new SuggestParams
                {
                    DocumentType = page.DocumentType,
                    PageName = $"{parentModel.NewUrl}/{page.Name}",
                    Template = page.PageTemplate?.Id,
                    UseHierarchy = false,
                    ParentId = immediateParent.Id,
                    WebpageId = page.Id
                });
        }

        private string GetHierarchy(IEnumerable<Webpage> webpages)
        {
            return string.Join(" > ", webpages.Select(x => x.Name));
        }

        private IEnumerable<Webpage> GetWebpageHierarchy(Webpage webpage)
        {
            yield return webpage;
            var descendants = _webpageRepository.Query().Where(x => x.Parent.Id == webpage.Id).OrderBy(x => x.DisplayOrder)
                .ToList();
            foreach (var descendant in descendants)
            {
                foreach (var child in GetWebpageHierarchy(descendant))
                {
                    yield return child;
                }
            }
        }

        public MoveWebpageResult Confirm(MoveWebpageModel model)
        {
            var confirmationModel = GetConfirmationModel(model);

            var success = SetParent(confirmationModel.Webpage, confirmationModel.Parent);
            if (!success)
            {
                return new MoveWebpageResult
                {
                    Success = false,
                    Message = _resourceProvider.GetValue("There was an issue setting the parent to the new value")
                };
            }

            success = _createUpdateUrlBatch.CreateBatch(confirmationModel);

            return new MoveWebpageResult
            {
                Success = success,
                Message = _resourceProvider.GetValue(success
                    ? "The page has been moved successfully"
                    : "There was an issue creating the batch to update the page URLs")
            };
        }

        public MoveWebpageModel GetModel(Webpage webpage)
        {
            return new MoveWebpageModel
            {
                Id = webpage.Id
            };
        }
    }
}