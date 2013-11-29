using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class ImportDocumentsService_GetHierarchyDepthTests
    {
        [Fact]
        public void InADirectHierarchyTheDepthsShouldIncrease()
        {
            var list = new HashSet<DocumentImportDTO>
                           {
                               new DocumentImportDTO
                                   {
                                       Name = "Test", UrlSegment = "test", DocumentType = typeof (BasicMappedWebpage).Name
                                   },
                               new DocumentImportDTO
                                   {
                                       Name = "Child", UrlSegment = "child", ParentUrl = "test", DocumentType = typeof (BasicMappedWebpage).Name
                                   },
                               new DocumentImportDTO
                                   {
                                       Name = "Another Child", UrlSegment = "another-child", ParentUrl = "child", DocumentType = typeof (BasicMappedWebpage).Name
                                   }
                           };

            for (int i = 0; i < list.Count; i++)
            {
                ImportDocumentsService.GetHierarchyDepth(list.ElementAt(i), list).Should().Be(i);
            }
        }
    }
}