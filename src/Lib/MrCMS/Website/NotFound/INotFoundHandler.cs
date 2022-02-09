using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.NotFound
{
    public interface INotFoundHandler
    {
        /// <summary>
        /// Looks for an exact match by path and query for any not found 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<(bool IsGone, RedirectResult Result, UrlHistory History)> FindByPathAndQuery(string path, string query);

        /// <summary>
        /// Looks up on path for histories redirecting to page, and forwards on the query if a match is found
        /// </summary>
        /// <param name="path"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<RedirectResult> FindByPathAndForwardQueryToPage(string path, string query);

        /// <summary>
        /// Checks known route handlers and creates a record if it can, returning the redirect result on success
        /// </summary>
        /// <param name="path"></param>
        /// <param name="query"></param>
        /// <param name="siteId"></param>
        /// <returns></returns>
        Task<RedirectResult> CheckKnownRoutes(string path, string query, int siteId);

        /// <summary>
        /// Saves an increment to the UrlHistory record to help indicate which pages have been not found the most 
        /// </summary>
        /// <param name="history"></param>
        Task IncrementFailedLookupCount(UrlHistory history);

        Task AddHistoryRecord(string path, string query, string referer, int siteId);
    }
}