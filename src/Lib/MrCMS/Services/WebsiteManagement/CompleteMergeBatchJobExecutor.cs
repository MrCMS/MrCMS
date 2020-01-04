using MrCMS.Batching;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;

namespace MrCMS.Services.WebsiteManagement
{
    public class CompleteMergeBatchJobExecutor : BaseBatchJobExecutor<CompleteMergeBatchJob>
    {
        static CompleteMergeBatchJobExecutor()
        {
            CompleteMergeTypes = new Dictionary<Type, HashSet<Type>>();

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>().Where(type => !type.ContainsGenericParameters))
            {
                var hashSet = new HashSet<Type>();

                var thisType = type;
                while (typeof(Webpage).IsAssignableFrom(thisType))
                {
                    foreach (var assignType in TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof(OnMergingWebpagesBase<>).MakeGenericType(thisType)))
                    {
                        hashSet.Add(assignType);
                    }
                    thisType = thisType.BaseType;

                }

                CompleteMergeTypes.Add(type, hashSet);
            }

        }

        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IRepository<UrlHistory> _urlHistoryRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationDisabler _notificationDisabler;
        private static readonly Dictionary<Type, HashSet<Type>> CompleteMergeTypes;

        public CompleteMergeBatchJobExecutor(
            IRepository<Webpage> webpageRepository,
            IRepository<UrlHistory> urlHistoryRepository,
            IServiceProvider serviceProvider, INotificationDisabler notificationDisabler)
        {
            _webpageRepository = webpageRepository;
            _urlHistoryRepository = urlHistoryRepository;
            _serviceProvider = serviceProvider;
            _notificationDisabler = notificationDisabler;
        }


        private void ApplyCustomMergeLogic(Webpage webpage, Webpage mergeInto)
        {
            var type = webpage.Unproxy().GetType();

            if (!CompleteMergeTypes.ContainsKey(type))
            {
                return;
            }

            foreach (var binderType in CompleteMergeTypes[type].Select(assignViewDataType => _serviceProvider.GetService(assignViewDataType)))
            {
                var completedBase = binderType as OnMergingWebpagesBase;
                completedBase?.MergeCompleted(webpage, mergeInto);
            }
        }

        protected override async Task<BatchJobExecutionResult> OnExecuteAsync(CompleteMergeBatchJob batchJob,
            CancellationToken token1)
        {
            using (_notificationDisabler.Disable())
            {
                var webpage = await _webpageRepository.Load(batchJob.WebpageId);
                if (webpage == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.WebpageId);
                }
                var mergeInto = await _webpageRepository.Load(batchJob.MergedIntoId);
                if (mergeInto == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.MergedIntoId);
                }

                await _webpageRepository.Transact(async (repo, token) =>
                {
                    ApplyCustomMergeLogic(webpage, mergeInto);

                    var urlSegment = webpage.UrlSegment;
                    await repo.Delete(webpage, token);
                    var urlHistory = new UrlHistory
                    {
                        UrlSegment = urlSegment,
                        Webpage = mergeInto,
                    };
                    mergeInto.Urls.Add(urlHistory);
                    await _urlHistoryRepository.Add(urlHistory, token);
                });


                return BatchJobExecutionResult.Success();
            }
        }
    }
}