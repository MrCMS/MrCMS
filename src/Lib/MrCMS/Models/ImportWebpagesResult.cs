using System.Collections.Generic;
using MrCMS.Batching.Entities;

namespace MrCMS.Models
{
    public class ImportWebpagesResult
    {
        public ImportWebpagesResult()
        {
            Errors = new Dictionary<string, List<string>>();
        }
        public int? BatchId { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
        public bool Success => BatchId != null;

        public static ImportWebpagesResult Successful(Batch batch)
        {
            return new ImportWebpagesResult {BatchId = batch?.Id};
        }
        public static ImportWebpagesResult Failure(Dictionary<string, List<string>> errors)
        {
            return new ImportWebpagesResult {Errors = errors};
        }
    }
}