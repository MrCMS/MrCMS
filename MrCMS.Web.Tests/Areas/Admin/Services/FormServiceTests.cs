using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class FormAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly FormAdminService _formAdminService;

        public FormAdminServiceTests()
        {
            _formAdminService = new FormAdminService(Session, A.Fake<IStringResourceProvider>());
        }

        [Fact]
        public void FormAdminService_ClearFormData_ShouldDeleteFormPosting()
        {
            var webpage = new StubWebpage();
            var posting = new FormPosting
            {
                IsDeleted = false,
                Webpage = webpage,
                FormValues = new List<FormValue>
                {
                    new FormValue
                    {
                        IsDeleted = false,
                        IsFile = false,
                        Key = "Name",
                        Value = "MrCMS"
                    }
                }
            };

            webpage.FormPostings = new List<FormPosting> {posting};

            Session.Transact(session => session.Save(posting));

            _formAdminService.ClearFormData(webpage);

            Session.QueryOver<FormPosting>().RowCount().Should().Be(0);
        }

        [Fact]
        public void FormAdminService_ExportFormData_ShouldReturnByteArray()
        {
            var webpage = new StubWebpage();
            var posting = new FormPosting
            {
                IsDeleted = false,
                Webpage = webpage,
                FormValues = new List<FormValue>
                {
                    new FormValue
                    {
                        IsDeleted = false,
                        IsFile = false,
                        Key = "Name",
                        Value = "MrCMS"
                    }
                }
            };

            webpage.FormPostings = new List<FormPosting> {posting};

            byte[] result = _formAdminService.ExportFormData(webpage);

            result.Should().BeOfType<byte[]>();
        }
    }
}