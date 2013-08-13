using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using NHibernate;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services
{
    public class FormServiceTests : InMemoryDatabaseTest
    {
        private readonly ISession _session;
        private readonly IDocumentService _documentService;
        private readonly IFileService _fileService;
        private readonly SiteSettings _siteSettings;
        private readonly MailSettings _mailSettings;
        private readonly FormService _formService;

        public FormServiceTests()
        {
            _documentService = A.Fake<DocumentService>();
            _fileService = A.Fake<FileService>();
            _siteSettings = A.Fake<SiteSettings>();
            _mailSettings = A.Fake<MailSettings>();
            _formService = new FormService(Session,_documentService, _fileService, _siteSettings, _mailSettings);
        }

        [Fact]
        public void FormService_ClearFormData_ShouldDeleteFormPosting()
        {
            var webpage = new BasicMappedWebpage();
            var posting = new FormPosting()
                {
                    IsDeleted = false,
                    Webpage = webpage,
                    FormValues = new List<FormValue>()
                        {
                            new FormValue()
                                {
                                    IsDeleted = false,
                                    IsFile = false,
                                    Key = "Name",
                                    Value = "MrCMS"
                                }
                        }
                };

            webpage.FormPostings = new List<FormPosting>() {posting};

            Session.Transact(session => session.Save(posting));

            _formService.ClearFormData(webpage);

            Session.QueryOver<FormPosting>().RowCount().Should().Be(0);
        }

        [Fact]
        public void FormService_ExportFormData_ShouldReturnByteArray()
        {
            var webpage = new BasicMappedWebpage();
            var posting = new FormPosting()
            {
                IsDeleted = false,
                Webpage = webpage,
                FormValues = new List<FormValue>()
                        {
                            new FormValue()
                                {
                                    IsDeleted = false,
                                    IsFile = false,
                                    Key = "Name",
                                    Value = "MrCMS"
                                }
                        }
            };

            webpage.FormPostings = new List<FormPosting>() { posting };

            var result=_formService.ExportFormData(webpage);

            result.Should().BeOfType<byte[]>();
        }
    }
}