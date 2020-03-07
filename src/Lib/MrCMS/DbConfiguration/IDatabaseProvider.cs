using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace MrCMS.DbConfiguration
{
    public interface IDatabaseProvider
    {
        string Type { get; }
        void SetupAction(IServiceProvider serviceProvider, DbContextOptionsBuilder builder, Assembly migrationsAssembly);
    }
}