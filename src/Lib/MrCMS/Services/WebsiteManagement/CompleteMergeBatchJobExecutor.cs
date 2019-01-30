using MrCMS.Batching;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ISession = NHibernate.ISession;

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

        private readonly ISession _session;
        private readonly IServiceProvider _serviceProvider;
        private readonly INotificationDisabler _notificationDisabler;
        private static readonly Dictionary<Type, HashSet<Type>> CompleteMergeTypes;

        public CompleteMergeBatchJobExecutor(ISession session, IServiceProvider serviceProvider, INotificationDisabler notificationDisabler)
        {
            _session = session;
            _serviceProvider = serviceProvider;
            _notificationDisabler = notificationDisabler;
        }


        protected override BatchJobExecutionResult OnExecute(CompleteMergeBatchJob batchJob)
        {
            using (_notificationDisabler.Disable())
            {
                var webpage = _session.Get<Webpage>(batchJob.WebpageId);
                if (webpage == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.WebpageId);
                }
                var mergeInto = _session.Get<Webpage>(batchJob.MergedIntoId);
                if (mergeInto == null)
                {
                    return BatchJobExecutionResult.Failure("Could not find the webpage with id " + batchJob.MergedIntoId);
                }

                _session.Transact(session =>
                {
                    ApplyCustomMergeLogic(webpage, mergeInto);

                    var urlSegment = webpage.UrlSegment;
                    session.Delete(webpage);
                    var urlHistory = new UrlHistory
                    {
                        UrlSegment = urlSegment,
                        Webpage = mergeInto,
                    };
                    mergeInto.Urls.Add(urlHistory);
                    session.Save(urlHistory);
                });


                return BatchJobExecutionResult.Success();
            }
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

        protected override Task<BatchJobExecutionResult> OnExecuteAsync(CompleteMergeBatchJob batchJob)
        {
            throw new System.NotImplementedException();
        }
    }
}