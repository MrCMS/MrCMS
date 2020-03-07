using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MrCMS.DbConfiguration
{
    public interface ISystemDatabase
    {
        DatabaseFacade Database { get; }
        bool IsMrCMSInstalled { get; }
    }
}