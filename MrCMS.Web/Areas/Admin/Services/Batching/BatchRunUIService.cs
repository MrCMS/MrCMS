using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using FluentNHibernate.Testing.Values;
using MrCMS.Batching.Entities;
using MrCMS.Batching.Services;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Mapping;

namespace MrCMS.Web.Areas.Admin.Services.Batching
{
    public class BatchRunUIService : IBatchRunUIService
    {
        private readonly IControlBatchRun _controlBatchRun;
        private readonly IExecuteNextBatchJob _executeNextBatchJob;
        private readonly UrlHelper _urlHelper;
        private readonly HttpContextBase _context;
        private readonly ISession _session;

        public BatchRunUIService(ISession session, IControlBatchRun controlBatchRun,
            IExecuteNextBatchJob executeNextBatchJob, UrlHelper urlHelper, HttpContextBase context)
        {
            _session = session;
            _controlBatchRun = controlBatchRun;
            _executeNextBatchJob = executeNextBatchJob;
            _urlHelper = urlHelper;
            _context = context;
        }

        public IList<BatchRunResult> GetResults(BatchRun batchRun)
        {
            return GetResultsQuery(batchRun)
                .OrderBy(result => result.ExecutionOrder)
                .Asc.Cacheable()
                .List();
        }

        public int? Start(BatchRun run)
        {
            var start = _controlBatchRun.Start(run) ? run.Id : (int?)null;
            if (start != null)
                ExecuteRequestForNextTask(run);
            return start;
        }

        public bool Pause(BatchRun run)
        {
            return _controlBatchRun.Pause(run);
        }

        public BatchCompletionStatus GetCompletionStatus(BatchRun batchRun)
        {
            var timeTaken =
                GetResultsQuery(batchRun)
                    .Where(result => result.MillisecondsTaken != null)
                    .Select(Projections.Sum<BatchRunResult>(result => result.MillisecondsTaken))
                    .Cacheable()
                    .FutureValue<decimal?>();
            var averageTimeTaken =
                GetResultsQuery(batchRun)
                    .Where(result => result.MillisecondsTaken != null)
                    .Select(Projections.Avg<BatchRunResult>(result => result.MillisecondsTaken))
                    .Cacheable()
                    .FutureValue<double?>();
            var pending =
                GetResultsQuery(batchRun)
                    .Where(result => result.Status == JobExecutionStatus.Pending)
                    .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();
            var failed =
                GetResultsQuery(batchRun)
                    .Where(result => result.Status == JobExecutionStatus.Failed)
                    .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();
            var succeeded =
                GetResultsQuery(batchRun)
                    .Where(result => result.Status == JobExecutionStatus.Succeeded)
                    .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();
            var total =
                GetResultsQuery(batchRun)
                     .Select(Projections.Count<BatchRunResult>(result => result.Id))
                    .Cacheable()
                    .FutureValue<int>();


            return new BatchCompletionStatus
            {
                Total = total.Value,
                Failed = failed.Value,
                Pending = pending.Value,
                Succeeded = succeeded.Value,
                TimeTaken = TimeSpan.FromMilliseconds(Convert.ToDouble(timeTaken.Value.GetValueOrDefault())),
                AverageTimeTaken = averageTimeTaken.Value.GetValueOrDefault().ToString("0.00ms")
            };
        }

        public void ExecuteRequestForNextTask(BatchRun run)
        {
            var cookieContainer = new CookieContainer();
            var cookies = _context.Request.Cookies;
            HttpCookie cookie = cookies[".AspNet.ApplicationCookie"];
            cookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, _context.Request.Url.Host));
            var httpClientHandler = new HttpClientHandler { UseCookies = true, CookieContainer = cookieContainer };
            var httpClient = new HttpClient(httpClientHandler);
            var url = _urlHelper.Action("ExecuteNext", "BatchExecution", new { id = run.Guid }, "http");
            httpClient.GetAsync(url);
        }

        public int? ExecuteNextTask(BatchRun run)
        {
            return _executeNextBatchJob.Execute(run) ? run.Id : (int?)null;
        }

        private IQueryOver<BatchRunResult, BatchRunResult> GetResultsQuery(BatchRun batchRun)
        {
            IQueryOver<BatchRunResult, BatchRunResult> queryOver = _session.QueryOver<BatchRunResult>();
            if (batchRun != null)
                return queryOver.Where(result => result.BatchRun.Id == batchRun.Id);
            // query to return 0;
            return queryOver.Where(result => result.Id < 0);
        }
    }
}