using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Indexing.Management
{
    public class IndexResult
    {
        public static readonly IndexResult Empty = new IndexResult();
        private readonly List<string> _errors;

        public IndexResult()
        {
            _errors = new List<string>();
        }

        public bool Success => Errors.Any();
        public long ExecutionTime { get; set; }

        public IEnumerable<string> Errors => _errors.AsReadOnly();

        public void AddError(string error)
        {
            _errors.Add(error);
        }
    }
}