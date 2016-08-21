using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using MrCMS.Web.Tests.TestSupport;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class FormAdminServiceTests 
    {
        private readonly FormAdminService _formAdminService;
        private InMemoryRepository<FormPosting> _formPostingRepository = new InMemoryRepository<FormPosting>();
        private InMemoryRepository<FormProperty> _formPropertyRepository = new InMemoryRepository<FormProperty>();
        private InMemoryRepository<FormListOption> _formListOptionRepository = new InMemoryRepository<FormListOption>();
        private InMemoryRepository<FormValue> _formValueRepository;

        public FormAdminServiceTests()
        {
            _formValueRepository = new InMemoryRepository<FormValue>();
            _formAdminService = new FormAdminService(_formPostingRepository, _formPropertyRepository, _formListOptionRepository, _formValueRepository);
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

            _formPostingRepository.Add(posting);

            _formAdminService.ClearFormData(webpage);

            _formPostingRepository.Query().Count().Should().Be(0);
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