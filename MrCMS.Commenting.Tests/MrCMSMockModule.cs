using System;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using MrCMS.Helpers;
using Ninject.Modules;

namespace MrCMS.Commenting.Tests
{
    /// <summary>
    ///     Auto mock out all simple MrCMS interfaces
    /// </summary>
    public class MrCMSMockModule : NinjectModule
    {
        public override void Load()
        {
            MethodInfo methodInfo =
                typeof(A).GetMethods(BindingFlags.Public | BindingFlags.Static)
                          .FirstOrDefault(info => info.Name == "Fake" && !info.GetParameters().Any());
            foreach (Type iface in
                TypeHelper.GetAllTypes()
                          .Where(type => type.IsPublic &&
                                 !string.IsNullOrWhiteSpace(type.Namespace) &&
                                 type.Namespace.StartsWith("MrCMS", StringComparison.OrdinalIgnoreCase) &&
                                 type.IsInterface && !type.IsGenericType))
            {
                Bind(iface).ToConstant(methodInfo.MakeGenericMethod(iface).Invoke(null, null));
            }
        }
    }
}