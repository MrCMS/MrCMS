using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public class OnEndRequestExecutor : IOnEndRequestExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly Dictionary<Type, Type> OnRequestExecutionTypes = new Dictionary<Type, Type>();

        static OnEndRequestExecutor()
        {
            var executors = TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(ExecuteEndRequestBase<,>));
            foreach (Type type in TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(EndRequestTask<>)).Where(type => !type.ContainsGenericParameters))
            {
                var executorType = executors.FirstOrDefault(x =>
                    typeof(ExecuteEndRequestBase<,>).MakeGenericType(type, type.BaseType.GenericTypeArguments[0])
                        .IsAssignableFrom(x));

                OnRequestExecutionTypes.Add(type, executorType);
            }
        }
        public OnEndRequestExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteTasks(HashSet<EndRequestTask> tasks)
        {
            var tasksGroupedByType = tasks.GroupBy(task => task.GetType())
                .ToDictionary(grouping => grouping.Key, grouping => grouping.ToHashSet());

            foreach (var type in tasksGroupedByType.Keys.OrderByDescending(GetExecutionPriority))
            {
                if (OnRequestExecutionTypes.ContainsKey(type) && OnRequestExecutionTypes[type] != null)
                {
                    var requestBase = _serviceProvider.GetService(OnRequestExecutionTypes[type]) as ExecuteEndRequestBase;
                    if (requestBase != null)
                    {
                        var data = tasksGroupedByType[type].Select(task => task.BaseData).ToHashSet();
                        await requestBase.Execute(data);
                        continue;

                    }
                }

                var message = $"Could not process tasks of type {type.FullName}. Please create a valid executor for the type";
                _serviceProvider.GetRequiredService<ILogger>().LogError(new Exception(message), message);
            }
        }

        private int GetExecutionPriority(Type arg)
        {
            var attribute = arg.GetCustomAttribute<EndRequestExecutionPriorityAttribute>();
            if (attribute == null)
            {
                return int.MinValue;
            }

            return attribute.Priority;
        }
    }
}