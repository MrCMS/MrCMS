using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    //public class ImportDocumentsService_GetRootParentUrlTests
    //{
    //    [Fact]
    //    public void InADirectHierarchyAllItemsShouldReturnTheRootUrl()
    //    {
    //        var list = new HashSet<DocumentImportDTO>
    //                       {
    //                           new DocumentImportDTO
    //                               {
    //                                   Name = "Test", UrlSegment = "test", DocumentType = typeof (BasicMappedWebpage).Name
    //                               },
    //                           new DocumentImportDTO
    //                               {
    //                                   Name = "Child", UrlSegment = "child", ParentUrl = "test", DocumentType = typeof (BasicMappedWebpage).Name
    //                               },
    //                           new DocumentImportDTO
    //                               {
    //                                   Name = "Another Child", UrlSegment = "another-child", ParentUrl = "child", DocumentType = typeof (BasicMappedWebpage).Name
    //                               }
    //                       };

    //        foreach (var dto in list)
    //        {
    //            ImportDocumentsService.GetRootParentUrl(dto, list).Should().Be("test");
    //        }
    //    }
    //}
}