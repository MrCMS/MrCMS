using System.Collections.Generic;

namespace MrCMS.Services
{
    public interface IAppStylesheetList
    {
        IEnumerable<string> UIStylesheets { get; }
        IEnumerable<string> AdminStylesheets { get; }
    }
}