using System;

namespace MrCMS.Website
{
    public interface IGetDateTimeNow
    {
        DateTime LocalNow { get; }
        DateTime UtcNow { get; }
    }
}