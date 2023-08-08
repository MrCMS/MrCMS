using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using NHibernate.Linq;
using StackExchange.Profiling.Internal;
using ISession = NHibernate.ISession;

namespace MrCMS.Website.NotFound
{
    public class NotFoundHandler : INotFoundHandler
    {
        private const int MaxLoggedUrlSize = 500;

        static NotFoundHandler()
        {
            KnownRouteCheckers = TypeHelper.GetAllConcreteTypesAssignableFrom<INotFoundRouteChecker>();
        }

        public static IReadOnlyCollection<Type> KnownRouteCheckers { get; }

        private readonly ISession _session;
        private readonly IServiceProvider _serviceProvider;

        public NotFoundHandler(ISession session, IServiceProvider serviceProvider)
        {
            _session = session;
            _serviceProvider = serviceProvider;
        }

        public async Task<(bool IsGone, RedirectResult Result, UrlHistory History)> FindByPathAndQuery(string path,
            string query)
        {
            var history = await _session.Query<UrlHistory>()
                .Where(x => x.UrlSegment == path + query)
                .Where(x => x.Webpage == null || x.Webpage.IsDeleted == false)
                .Take(1)
                .SingleOrDefaultAsync();

            if (history == null)
                return (false, null, null);

            if (history.IsGone)
                return (true, null, history);

            if (history.IsIgnored)
                return (false, null, history);

            if (history.Webpage != null)
            {
                var webpage = history.Webpage.Unproxy();
                if (webpage.Published)
                {
                    return (false, new RedirectResult($"/{webpage.UrlSegment.EncodeParts()}", true), history);
                }
            }
            

            if (!string.IsNullOrWhiteSpace(history.RedirectUrl))
                return (false, new RedirectResult(history.RedirectUrl.EncodeParts(), true), history);

            return (false, null, history);
        }

        public async Task<RedirectResult> FindByPathAndForwardQueryToPage(string path, string query)
        {
            var history = await _session.Query<UrlHistory>()
                .Where(x => x.UrlSegment == path)
                .Where(x => x.Webpage != null && x.Webpage.Published)
                .Fetch(x => x.Webpage)
                .Take(1).SingleOrDefaultAsync();
            if (history?.Webpage == null)
                return null;

            return new RedirectResult($"/{history.Webpage.UrlSegment}{query}", true);
        }

        public async Task<RedirectResult> CheckKnownRoutes(string path, string query, int siteId)
        {
            foreach (var type in KnownRouteCheckers)
            {
                var checker = _serviceProvider.GetRequiredService(type) as INotFoundRouteChecker;
                var result = await checker.Check(path, query);
                if (!result.Success)
                    continue;
                await AddHistoryRecord(path, query, siteId, result);
                return new RedirectResult(result.GetRedirectUrl());
            }

            return null;
        }


        public async Task IncrementFailedLookupCount(UrlHistory history)
        {
            history.FailedLookupCount += 1;
            await _session.Connection.ExecuteAsync(
                @"
                UPDATE [UrlHistory] 
                    SET 
                        [FailedLookupCount] = [FailedLookupCount] + 1,
                        [UpdatedOn] = GETDATE()
                WHERE [Id] = @id
                ",
                new {id = history.Id});
        }

        public async Task AddHistoryRecord(string path, string query, string referer, int siteId)
        {
            // we're going to not log urls longer than max size as they won't fit and are likely some sort of garbage
            var urlSegment = $"{path}{query}";
            if (urlSegment.Length > MaxLoggedUrlSize)
                return;
            // await _session.TransactAsync(session => session.SaveAsync(new UrlHistory
            // {
            //     UrlSegment = urlSegment,
            //     FailedLookupCount = 1,
            //     InitialReferer = referer
            // }));
            await _session.Connection.ExecuteAsync(
                @"
INSERT INTO [UrlHistory] (UrlSegment, Guid, CreatedOn, UpdatedOn, IsDeleted, SiteId, InitialReferer, FailedLookupCount)
values (@urlSegment, NEWID(), GETDATE(), GETDATE(), 0, @siteId, @referer, 1);
                ",
                new
                {
                    urlSegment,
                    referer,
                    siteId
                });
        }

        private async Task AddHistoryRecord(string path, string query, int siteId, NotFoundCheckResult result)
        {
            await _session.Connection.ExecuteAsync(
                @"
INSERT INTO [UrlHistory] (UrlSegment, Guid, CreatedOn, UpdatedOn, IsDeleted, WebpageId, SiteId, RedirectUrl,
                        FailedLookupCount)
values (@urlSegment, NEWID(), GETDATE(), GETDATE(), 0, @webpageId, @siteId, @redirectUrl, 0);
                ",
                new
                {
                    urlSegment = $"{path}{query}",
                    webpageId = result.Webpage?.Id,
                    siteId,
                    redirectUrl = result.Url
                });
            // await _session.TransactAsync(session => session.SaveAsync(new UrlHistory
            // {
            //     UrlSegment = $"{path}{query}",
            //     FailedLookupCount = 0,
            //     Webpage = result.Webpage,
            //     RedirectUrl = result.Url
            // }));
        }
    }
    
    public static class NotFoundHelpers
    {
        /// <summary>
        /// When UTF8 chars in the headers, redirects fails so we need to encode those parts but not the forward slashes otherwise the URL looks wrong.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string EncodeParts (this string url)
        {
            return url.IsNullOrWhiteSpace() ? "" : string.Join("/", url.Split('/').Select(WebUtility.UrlEncode));
        }
    }
}