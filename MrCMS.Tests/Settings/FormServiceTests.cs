using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Settings
{
    public class FormServiceTests : InMemoryDatabaseTest
    {
        private ISession _session;
        private IDocumentService _documentService;
        private SiteSettings _siteSettings;
        private MailSettings _mailSettings;

        [Fact]
        public void FormService_GetFormPosting_CallsSessionGetFormPosting()
        {
            var formService = GetFormService(A.Fake<ISession>());

            formService.GetFormPosting(1);

            A.CallTo(() => _session.Get<FormPosting>(1)).MustHaveHappened();
        }

        [Fact]
        public void FormService_GetFormPosting_ReturnsTheResultOfSessionGetFormPosting()
        {
            var formService = GetFormService(A.Fake<ISession>());
            var formPosting = new FormPosting();
            A.CallTo(() => _session.Get<FormPosting>(1)).Returns(formPosting);

            formService.GetFormPosting(1).Should().Be(formPosting);
        }

        private FormService GetFormService(ISession session = null, IDocumentService documentService = null)
        {
            _session = session ?? Session;
            _documentService = documentService ?? A.Fake<IDocumentService>();
            _siteSettings = new SiteSettings();
            _mailSettings = new MailSettings();
            return new FormService(_session, _documentService, _siteSettings, _mailSettings);
        }
    }
}