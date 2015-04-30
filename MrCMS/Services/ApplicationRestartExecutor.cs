using System.Collections.Generic;
using MrCMS.Installation;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class ApplicationRestartExecutor : ExecuteEndRequestBase<ApplicationRestart, int>
    {
        private readonly IRestartApplication _restartApplication;

        public ApplicationRestartExecutor(IRestartApplication restartApplication)
        {
            _restartApplication = restartApplication;
        }

        public override void Execute(IEnumerable<int> data)
        {
            _restartApplication.Restart();
        }
    }
}