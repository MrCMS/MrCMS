using System;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public interface IUpdateUrlHistoryService
    {
        IUpdateUrlHistoryService Initialise();
        void SetUrlHistory(DocumentImportDTO documentDto, Webpage webpage);
        void SaveUrlHistories();
    }
}