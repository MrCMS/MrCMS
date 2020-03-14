using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Services
{
    public class UnlockPageService : IUnlockPageService
    {
        private readonly IRepository<Webpage> _repository;
        private readonly IUniquePageService _uniquePageService;
        private readonly IPasswordProtectedPageChecker _checker;

        public UnlockPageService(IRepository<Webpage> repository, IUniquePageService uniquePageService, IPasswordProtectedPageChecker checker)
        {
            _repository = repository;
            _uniquePageService = uniquePageService;
            _checker = checker;
        }
        public Task<Webpage> GetLockedPage(int id)
        {
            return _repository.GetData(id);
        }

        public async Task<UnlockPageResult> TryUnlockPage(UnlockPageModel model, IResponseCookies cookies)
        {
            var page = await GetLockedPage(model.LockedPage);
            // if it's not found, we'll just redirect back and let the GET handle the page not being found
            if (page == null)
            {
                return new UnlockPageResult();
            }

            // if it doesn't have password auth, we'll just redirect back to it
            if (!page.HasCustomPermissions || page.PermissionType != WebpagePermissionType.PasswordBased)
            {
                return new UnlockPageResult { Success = true, LockedPageId = page.Id };
            }

            var isMatch = (model.PagePassword ?? string.Empty).Trim() == page.Password;
            // if it is a match, we'll prepare to set-up auth
            if (isMatch)
            {
                _checker.GiveAccessToPage(page, cookies);
                return new UnlockPageResult { Success = true, LockedPageId = page.Id };
            }

            return new UnlockPageResult { Success = false, LockedPageId = page.Id };
        }

        public async Task<RedirectResult> RedirectToPage(int id)
        {
            var page = await GetLockedPage(id);
            return new RedirectResult($"~/{page.UrlSegment}");

        }

        public async Task<RedirectResult> RedirectBackToPage(UnlockPageModel model)
        {
            return await _uniquePageService.RedirectTo<WebpagePasswordPage>(new { lockedPage = model.LockedPage });
        }
    }
}