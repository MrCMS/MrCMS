using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace MrCMS.Indexing.Management
{
    public class IndexResult
    {
        private readonly List<string> _errors;

        private IndexResult()
        {
            _errors = new List<string>();
        }

        public bool Success { get { return Errors.Any(); } }
        public long ExecutionTime { get; set; }

        private void AddError(string error)
        {
            _errors.Add(error);
        }

        public IEnumerable<string> Errors { get { return _errors.AsReadOnly(); } }

        public static IndexResult GetResult(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            var indexResult = new IndexResult();
            try
            {
                action();
            }
            catch (Exception ex)
            {
                indexResult.AddError(ex.Message);
            }
            stopwatch.Stop();
            indexResult.ExecutionTime = stopwatch.ElapsedMilliseconds;
            return indexResult;
        }
    }
}