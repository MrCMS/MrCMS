using System;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public interface IGetDateTimeNow
    {
        Task<DateTime> GetLocalNow();
        DateTime UtcNow { get; }
    }
}