using System.Collections.Generic;
using MrCMS.Batching.Entities;

namespace MrCMS.Models
{
    public class ImportDocumentsResult
    {
        public ImportDocumentsResult()
        {
            Errors = new Dictionary<string, List<string>>();
        }
        public int? BatchId { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
        public bool Success => BatchId != null;

        public static ImportDocumentsResult Successful(Batch batch)
        {
            return new ImportDocumentsResult {BatchId = batch?.Id};
        }
        public static ImportDocumentsResult Failure(Dictionary<string, List<string>> errors)
        {
            return new ImportDocumentsResult {Errors = errors};
        }
    }
}