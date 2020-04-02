using System;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace MrCMS.Website
{
    public interface IGetDateTimeNow
    {
        Task<DateTime> GetLocalNow();
        DateTime UtcNow { get; }
    }
}