using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Services.Widgets;
using Ninject;

namespace MrCMS.Services
{
    public class WidgetModelService : IWidgetModelService
    {
        public static readonly Dictionary<string, Type> WidgetModelRetrievers;
        private readonly IKernel _kernel;

        static WidgetModelService()
        {
            WidgetModelRetrievers = new Dictionary<string, Type>();

            foreach (
                Type type in
                    TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Widget>()
                        .Where(type => !type.ContainsGenericParameters))
            {
                HashSet<Type> types =
                    TypeHelper.GetAllConcreteTypesAssignableFrom(typeof (GetWidgetModelBase<>).MakeGenericType(type));
                if (types.Any())
                {
                    WidgetModelRetrievers.Add(type.FullName, types.First());
                }
            }
        }

        public WidgetModelService(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object GetModel(Widget widget)
        {
            if (widget == null) return null;
            GetWidgetModelBase retriever = null;
            string typeName = widget.GetType().FullName;
            if (WidgetModelRetrievers.ContainsKey(typeName))
            {
                retriever = _kernel.Get(WidgetModelRetrievers[typeName]) as GetWidgetModelBase;
            }
            retriever = retriever ?? DefaultGetWidgetModel.Instance;
            return retriever.GetModel(widget);
        }
    }
}