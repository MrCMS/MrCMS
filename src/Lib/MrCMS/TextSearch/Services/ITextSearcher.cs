using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MrCMS.TextSearch.Entities;
using X.PagedList;

namespace MrCMS.TextSearch.Services
{
    public interface ITextSearcher
    {
        Task<IList<TextSearchItem>> Search(TextSearcherQuery query);
        Task<IPagedList<TextSearchItem>> SearchPaged(PagedTextSearcherQuery query);

        public class TextSearcherQuery
        {
            public string Term { get; set; }
            public string Type { get; set; }

            public DateTime? CreatedOnFrom { get; set; }
            public DateTime? CreatedOnTo { get; set; }

            public DateTime? UpdatedOnFrom { get; set; }
            public DateTime? UpdatedOnTo { get; set; }

            public int ResultSize { get; set; } = 10;

            public TextSearcherQuerySort SortBy { get; set; }

            public enum TextSearcherQuerySort
            {
                [Description("Updated On Descending")] UpdatedOnDesc,
                [Description("Updated On")] UpdatedOn,
                [Description("Created On Descending")] CreatedOnDesc,
                [Description("Created On")] CreatedOn,
                [Description("Display Name")] DisplayName,

                [Description("Display Name Descending")]
                DisplayNameDesc,
            }
        }

        public class PagedTextSearcherQuery : TextSearcherQuery
        {
            public int Page { get; set; } = 1;
        }

        List<Type> GetTypes();
    }
}