using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Application.Pages;
using MrCMS.Web.Areas.Admin.Controllers;
using NHibernate;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class InPageAdminControllerTests
    {
        private IDocumentService documentService;
        private ISession session;
        
        InPageAdminController GetInPageAdminController()
        {
            documentService = A.Fake<IDocumentService>();
            session = A.Fake<ISession>();
            return new InPageAdminController(documentService, session);
        }
    }
}