using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.TestSupport;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Tests.Stubs;
using System.Collections.Generic;
using AutoMapper;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Services
{
    public class FormAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly FormAdminService _formAdminService;

        public FormAdminServiceTests()
        {
            _formAdminService = new FormAdminService(Session, A.Fake<IStringResourceProvider>(),
                A.Fake<ILogger<FormAdminService>>(), A.Fake<IMapper>(), A.Fake<IGetCurrentUserCultureInfo>());
        }

        [Fact]
        public void FormAdminService_ClearFormData_ShouldDeleteFormPosting()
        {
            var form = new Form();
            var posting = new FormPosting
            {
                IsDeleted = false,
                Form = form,
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

            form.FormPostings = new List<FormPosting> { posting };

            Session.Transact(session => session.Save(posting));

            _formAdminService.ClearFormData(form);

            Session.QueryOver<FormPosting>().RowCount().Should().Be(0);
        }

        [Fact]
        public void FormAdminService_ExportFormData_ShouldReturnByteArray()
        {
            var form = new Form();
            var posting = new FormPosting
            {
                IsDeleted = false,
                Form = form,
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

            form.FormPostings = new List<FormPosting> { posting };

            byte[] result = _formAdminService.ExportFormData(form);

            result.Should().BeOfType<byte[]>();
        }
    }
}