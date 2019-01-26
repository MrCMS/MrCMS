using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MrCMS.Indexing.Management
{
    public class GetIndexResult : IGetIndexResult
    {
        private readonly ILogger<GetIndexResult> _logger;

        public GetIndexResult(ILogger<GetIndexResult> logger)
        {
            _logger = logger;
        }

        public IndexResult GetResult(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            var indexResult = new IndexResult();
            try
            {
                action();
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                indexResult.AddError(exception.Message);
            }
            stopwatch.Stop();
            indexResult.ExecutionTime = stopwatch.ElapsedMilliseconds;
            return indexResult;
        }
    }
}