using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Common;
using MrCMS.Helpers;

namespace MrCMS.Data
{
    public class HandleDataChanges : IHandleDataChanges
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IGetAllDataSavedHandlers _getHandlers;

        public HandleDataChanges(IServiceProvider serviceProvider, IGetAllDataSavedHandlers getHandlers)
        {
            _serviceProvider = serviceProvider;
            _getHandlers = getHandlers;
        }

        public async Task HandleChanges(ContextChangeData data)
        {
            await _serviceProvider.GetService<IAuditChanges>().Audit(data);
            foreach (var handler in _getHandlers.GetHandlers())
            {
                var handlerType = handler.GetType();
                if (handlerType.IsImplementationOf(typeof(OnDataAdded<>)))
                {
                    foreach (var entityData in data.Added)
                    {
                        var genericAdded = typeof(OnDataAdded<>).MakeGenericType(entityData.Type);
                        if (!handlerType.IsImplementationOf(genericAdded)) continue;

                        MethodInfo methodInfo = handlerType.GetMethod("Execute", new[] { typeof(EntityData) });
                        await (Task)methodInfo.Invoke(handler, new[] { entityData });
                    }
                }
                else if (handlerType.IsImplementationOf(typeof(OnDataUpdated<>)))
                {
                    foreach (var changeInfo in data.Updated)
                    {
                        var genericUpdated = typeof(OnDataUpdated<>).MakeGenericType(changeInfo.Type);
                        if (!handlerType.IsImplementationOf(genericUpdated)) continue;

                        MethodInfo methodInfo = handlerType.GetMethod("Execute", new[] { typeof(ChangeInfo) });
                        await (Task)methodInfo.Invoke(handler, new[] { changeInfo });
                    }
                }
                else if (handlerType.IsImplementationOf(typeof(OnDataDeleted<>)))
                {
                    foreach (var entityData in data.Deleted)
                    {
                        var genericDeleted = typeof(OnDataDeleted<>).MakeGenericType(entityData.Type);
                        if (!handlerType.IsImplementationOf(genericDeleted)) continue;

                        MethodInfo methodInfo = handlerType.GetMethod("Execute", new[] { typeof(EntityData) });
                        await (Task)methodInfo.Invoke(handler, new[] { entityData });
                    }
                }
            }
        }
    }
}