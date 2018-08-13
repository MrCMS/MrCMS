using System;

namespace MrCMS.Indexing.Management
{
    public interface IGetIndexResult
    {
        IndexResult GetResult(Action action);
    }
}