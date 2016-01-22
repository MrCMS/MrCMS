using System.Collections.Generic;
using MrCMS.Batching.Entities;

namespace MrCMS.Models
{
    public class ImportDocumentsResult
    {
        private ImportDocumentsResult()
        {
            Errors = new Dictionary<string, List<string>>();
        }
        public Batch Batch { get; private set; }
        public Dictionary<string, List<string>> Errors { get; private set; }
        public bool Success { get { return Batch != null; } }

        public static ImportDocumentsResult Successful(Batch batch)
        {
            return new ImportDocumentsResult { Batch = batch };
        }
        public static ImportDocumentsResult Failure(Dictionary<string, List<string>> errors)
        {
            return new ImportDocumentsResult { Errors = errors };
        }
    }
}