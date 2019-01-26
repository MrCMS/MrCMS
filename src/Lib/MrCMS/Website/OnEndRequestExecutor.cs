using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MrCMS.Website
{
    public class OnEndRequestExecutor : IOnEndRequestExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private static readonly Dictionary<Type, Type> OnRequestExecutionTypes = new Dictionary<Type, Type>();

        static OnEndRequestExecutor()
        {
            foreach (Type type in TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(EndRequestTask<>)).Where(type => !type.ContainsGenericParameters))
            {
                var executorType = TypeHelper.GetAllConcreteTypesAssignableFrom(
                    typeof(ExecuteEndRequestBase<,>).MakeGenericType(type, type.BaseType.GenericTypeArguments[0])).FirstOrDefault();


                OnRequestExecutionTypes.Add(type, executorType);
            }
        }
        public OnEndRequestExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ExecuteTasks(HashSet<EndRequestTask> tasks)
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
                        requestBase.Execute(data);
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