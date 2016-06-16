using System;
using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Search.Models;

namespace MrCMS.Search
{
    public class UniversalSearchIndexData
    {
        public UniversalSearchItem UniversalSearchItem { get; set; }
        public UniversalSearchIndexAction Action { get; set; }

        public static IEqualityComparer<UniversalSearchIndexData> Comparer
        {
            get
            {
                return
                    new StrictKeyEqualityComparer<UniversalSearchIndexData, UniversalSearchIndexDataComparison>(
                        data => new UniversalSearchIndexDataComparison(data));
            }
        }

        public struct UniversalSearchIndexDataComparison : IEquatable<UniversalSearchIndexDataComparison>
        {
            private readonly UniversalSearchIndexAction _action;
            private readonly string _uniqueKey;

            public UniversalSearchIndexDataComparison(UniversalSearchIndexData data)
            {
                _uniqueKey = data.UniversalSearchItem.UniqueKey;
                _action = data.Action;
            }

            public bool Equals(UniversalSearchIndexDataComparison other)
            {
                return _uniqueKey == other._uniqueKey && _action == other._action;
            }
        }
    }
}