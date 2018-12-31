using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Services.Widgets;

namespace MrCMS.Services
{
    public class WidgetModelService : IWidgetModelService
    {
        public static readonly Dictionary<string, Type> WidgetModelRetrievers;
        private readonly IServiceProvider _serviceProvider;

        static WidgetModelService()
        {
            WidgetModelRetrievers = new Dictionary<string, Type>();

            foreach (
                Type type in
                    TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Widget>()
                        .Where(type => !type.ContainsGenericParameters))
            {
                HashSet<Type> types =
                    TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(GetWidgetModelBase<>).MakeGenericType(type));
                if (types.Any())
                {
                    WidgetModelRetrievers.Add(type.FullName, types.First());
                }
            }
        }

        public WidgetModelService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetModel(Widget widget)
        {
            GetWidgetModelBase retriever = GetRetriever(widget);
            return retriever.GetModel(widget);
        }
        
        private GetWidgetModelBase GetRetriever(Widget widget)
        {
            //using (MiniProfiler.Current.Step("Get Retriever"))
            {
                if (widget == null) return DefaultGetWidgetModel.Instance;
                GetWidgetModelBase retriever = null;
                string typeName = widget.GetType().FullName;
                if (WidgetModelRetrievers.ContainsKey(typeName))
                {
                    retriever = _serviceProvider.GetRequiredService(WidgetModelRetrievers[typeName]) as GetWidgetModelBase;
                }
                return retriever ?? DefaultGetWidgetModel.Instance;
            }
        }
    }
}