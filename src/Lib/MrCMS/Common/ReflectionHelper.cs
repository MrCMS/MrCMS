using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MrCMS.Helpers;

namespace MrCMS.Common
{
    public class ReflectionHelper : IReflectionHelper
    {
        public ReflectionHelper(params Assembly[] assemblies)
        {
            if (assemblies == null)
                return;
            Assemblies = new HashSet<Assembly>(assemblies);
            Types = new HashSet<Type>(Assemblies.SelectMany(AssemblyExtensions.GetTypes));
        }

        public HashSet<Assembly> Assemblies { get; } = new HashSet<Assembly>();
        public HashSet<Type> Types { get; } = new HashSet<Type>();

        public IEnumerable<Type> GetAllConcreteImplementationsOf(Type type)
        {
            return Types.Where(t => IsImplementationOf(t, type) && !IntrospectionExtensions.GetTypeInfo(t).IsAbstract);
        }

        public IEnumerable<Type> GetAllConcreteImplementationsOf<T>() => GetAllConcreteImplementationsOf(typeof(T));

        public IEnumerable<Type> GetConcreteTypes()
        {
            return GetAllConcreteImplementationsOf(typeof(object));
        }

        public IEnumerable<Type> GetBaseTypes<T>(bool includeSelf = false)
        {
            return GetBaseTypes(typeof(T), includeSelf);
        }

        public IEnumerable<Type> GetBaseTypes(Type type, bool includeSelf = false)
        {
            if (includeSelf)
                yield return type;
            var baseType = type.GetTypeInfo().BaseType;
            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.GetTypeInfo().BaseType;
            }
        }

        public bool IsImplementationOf(Type type, Type baseType)
        {
            if (baseType.GetTypeInfo().IsGenericTypeDefinition)
            {
                return GetGenericType(type, baseType) != null;
            }
            return baseType.GetTypeInfo().IsAssignableFrom(type);
        }

        public Dictionary<Type, Type> GetSimpleInterfaceImplementationPairings()
        {
            var interfaces = Types.Where(x => x.GetTypeInfo().IsInterface);

            var singleImplementationInterfaces = interfaces.Where(x => GetAllConcreteImplementationsOf(x).Count() == 1);

            return singleImplementationInterfaces.ToDictionary(x => x, x => GetAllConcreteImplementationsOf(x).First());
        }

        public Type GetTypeByFullName(string name)
        {
            return Types.FirstOrDefault(x => StringComparer.OrdinalIgnoreCase.Equals(x.FullName, name));
        }

        private Type GetGenericType(Type type, Type openGenericType)
        {
            if (openGenericType.GetTypeInfo().IsInterface)
                return GetBaseTypes(type, true)
                    .FirstOrDefault(
                        t2 => t2.GetTypeInfo().GetInterfaces().Any(
                            tInt => tInt.GetTypeInfo().IsGenericType
                                    &&
                                    tInt.GetGenericTypeDefinition() ==
                                    openGenericType));
            return GetBaseTypes(type)
                .FirstOrDefault(
                    t2 =>
                        t2.GetTypeInfo().IsGenericType &&
                        t2.GetGenericTypeDefinition() == openGenericType);
        }



        public IEnumerable<Type> GetAllTypesWithAttribute<T>(bool inherit) where T : Attribute
        {
            return Types.Where(type => type.GetTypeInfo().GetCustomAttributes<T>(inherit).Any());
        }

        public TypeHelper.IsImplementationOfOpenGenericResult TypeIsImplementationOfOpenGeneric(Type type, Type implementationOfType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (implementationOfType == null)
                throw new ArgumentNullException(nameof(implementationOfType));

            if (!implementationOfType.GetTypeInfo().IsGenericTypeDefinition)
                throw new ArgumentException("Implementation of type is not an open generic", nameof(implementationOfType));

            var genericType = GetGenericType(type, implementationOfType);
            return new TypeHelper.IsImplementationOfOpenGenericResult(genericType);
        }
    }
}