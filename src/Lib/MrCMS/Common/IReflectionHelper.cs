using System;
using System.Collections.Generic;
using MrCMS.Helpers;

namespace MrCMS.Common
{
    public interface IReflectionHelper
    {
        IEnumerable<Type> GetAllConcreteImplementationsOf(Type type);
        IEnumerable<Type> GetAllConcreteImplementationsOf<T>();
        IEnumerable<Type> GetConcreteTypes();
        IEnumerable<Type> GetBaseTypes<T>(bool includeSelf = false);
        IEnumerable<Type> GetBaseTypes(Type type, bool includeSelf = false);
        bool IsImplementationOf(Type type, Type baseType);
        Dictionary<Type, Type> GetSimpleInterfaceImplementationPairings();
        Type GetTypeByFullName(string name);

        IEnumerable<Type> GetAllTypesWithAttribute<T>(bool inherit) where T : Attribute;

        TypeHelper.IsImplementationOfOpenGenericResult TypeIsImplementationOfOpenGeneric(Type type, Type implementationOfType);

    }
}