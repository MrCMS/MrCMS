using System;
using System.Threading.Tasks;
using Dapper;
using NHibernate;

namespace MrCMS.Tasks
{
    /// <summary>
    /// Note, AzureSQLMaintenance proc needs to be installed: https://raw.githubusercontent.com/yochananrachamim/AzureSQL/master/AzureSQLMaintenance.txt
    /// </summary>
    public class ExecuteAzureSqlMaintenanceMsSql : SchedulableTask
    {
        private readonly IStatelessSession _session;

        public ExecuteAzureSqlMaintenanceMsSql(IStatelessSession session)
        {
            _session = session;
        }
        protected override async Task OnExecute()
        {
            await _session.Connection.ExecuteAsync(@"exec  AzureSQLMaintenance 'all', @LogToTable=1", commandTimeout: TimeSpan.FromHours(6).Seconds);
        }
    }
}