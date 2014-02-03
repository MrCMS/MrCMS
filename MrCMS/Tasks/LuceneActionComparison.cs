using System;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public struct LuceneActionComparison : IEquatable<LuceneActionComparison>
    {
        public static readonly StrictKeyEqualityComparer<LuceneAction, LuceneActionComparison> Comparer =
            new StrictKeyEqualityComparer<LuceneAction, LuceneActionComparison>(action => new LuceneActionComparison(action));

        private LuceneActionComparison(LuceneAction action)
            : this()
        {
            EntityId = action.Entity == null ? (int?)null : action.Entity.Id;
            Operation = action.Operation;
            Type = action.Type;
            DefinitionName = action.IndexDefinition.SystemName;
        }

        private string DefinitionName { get; set; }

        private Type Type { get; set; }

        private LuceneOperation Operation { get; set; }

        private int? EntityId { get; set; }

        public bool Equals(LuceneActionComparison other)
        {
            return base.Equals(other);
        }
    }
}