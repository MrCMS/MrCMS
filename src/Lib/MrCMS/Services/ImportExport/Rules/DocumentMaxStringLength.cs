using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport.Rules
{
    public abstract class DocumentMaxStringLength : IDocumentImportValidationRule
    {
        protected string DisplayName { get; }
        protected Func<DocumentImportDTO, string> Selector { get; }
        protected int Length { get; }

        protected DocumentMaxStringLength(string displayName, Func<DocumentImportDTO, string> selector, int length)
        {
            DisplayName = displayName;
            Selector = selector;
            Length = length;
        }

        public Task<IReadOnlyList<string>> GetErrors(DocumentImportDTO item, IList<DocumentImportDTO> allItems)
        {
            var list = new List<string>();
            var value = Selector(item);
            if (!String.IsNullOrWhiteSpace(value))
            {
                if (value.Length > Length)
                    list.Add(
                            $"{DisplayName} is too long - max length is {Length} characters and your value is {value.Length} characters in length."
                        )
                ;
            }

            return Task.FromResult<IReadOnlyList<string>>(list);
        }
    }
}