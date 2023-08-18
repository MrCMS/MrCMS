using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Resources;
using MrCMS.TestSupport;
using MrCMS.Web.Admin.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MrCMS.Mapping;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Services
{
    public class FormAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly FormAdminService _formAdminService;

        public FormAdminServiceTests()
        {
            _formAdminService = new FormAdminService(Session, A.Fake<IStringResourceProvider>(),
                A.Fake<ILogger<FormAdminService>>(), A.Fake<ISessionAwareMapper>(), A.Fake<IGetCurrentUserCultureInfo>());
        }

        [Fact]
        public async Task FormAdminService_ClearFormData_ShouldDeleteFormPosting()
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

            await Session.TransactAsync(session => session.SaveAsync(posting));

            await _formAdminService.ClearFormData(form);

            (await Session.QueryOver<FormPosting>().RowCountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task FormAdminService_ExportFormData_ShouldReturnByteArray()
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

            byte[] result = await _formAdminService.ExportFormData(form);

            result.Should().BeOfType<byte[]>();
        }
    }
}