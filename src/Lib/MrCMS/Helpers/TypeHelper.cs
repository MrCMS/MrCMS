using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities;
using MrCMS.Settings;
using NHibernate.Proxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MrCMS.Helpers
{
    // TODO: refactor this to non-static
    public static class TypeHelper
    {
        private static HashSet<Type> _allTypes;
        static HashSet<Assembly> _mrCMSAssemblies;
        private static bool _initialized = false;
        private static readonly Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();
        private static readonly Dictionary<string, AssemblyName[]> _referencedAssemblies = new Dictionary<string, AssemblyName[]>();


        public static void Initialize(Assembly assembly)
        {
            var referencedAssemblies = assembly.GetReferencedAssemblies().Where(name => !IsSystemAssembly(name)).ToList();

            LoadAssemblies(referencedAssemblies);

            _mrCMSAssemblies = GetMrCMSAssemblies(referencedAssemblies);

            _initialized = true;
        }

        private static HashSet<Assembly> GetMrCMSAssemblies(List<AssemblyName> referencedAssemblies)
        {
            var thisAssembly = typeof(TypeHelper).Assembly;
            var assemblies = new HashSet<Assembly>();
            foreach (var assemblyName in referencedAssemblies)
            {
                var tree = BuildTree(assemblyName);

                foreach (var node in tree.FindTreeNodes(x => x.Data == thisAssembly))
                {
                    var thisNode = node;
                    while (thisNode != null)
                    {
                        assemblies.Add(thisNode.Data);
                        thisNode = thisNode.Parent;
                    }
                }
            }

            return assemblies;
        }

        private static TreeNode<Assembly> BuildTree(AssemblyName assemblyName)
        {
            if (!_loadedAssemblies.ContainsKey(assemblyName.Name))
            {
                return null;
            }

            var tree = new TreeNode<Assembly>(_loadedAssemblies[assemblyName.Name]);
            var referencedAssemblies = _referencedAssemblies[assemblyName.Name];
            foreach (var referencedAssembly in referencedAssemblies)
            {
                var child = BuildTree(referencedAssembly);
                if (child != null)
                {
                    tree.AddChild(child);
                }
            }

            return tree;
        }

        private static void LoadAssemblies(IEnumerable<AssemblyName> referencedAssemblies)
        {
            foreach (var assemblyName in referencedAssemblies)
            {
                if (IsSystemAssembly(assemblyName))
                {
                    continue;
                }

                if (_loadedAssemblies.ContainsKey(assemblyName.Name))
                {
                    continue;
                }

                var assembly = Assembly.Load(assemblyName);
                _loadedAssemblies[assemblyName.Name] = assembly;
                var childReferences = assembly.GetReferencedAssemblies();
                _referencedAssemblies[assemblyName.Name] = childReferences;
                LoadAssemblies(childReferences);
            }
        }

        private static bool IsSystemAssembly(AssemblyName assemblyName)
        {
            return assemblyName.FullName.StartsWith("Microsoft.") || assemblyName.FullName.StartsWith("System.");
        }

        public static HashSet<Assembly> GetAllMrCMSAssemblies()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Must be initialized before use");
            }

            return _mrCMSAssemblies;
        }

        public static HashSet<Type> GetAllTypes()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Must be initialized before use");
            }

            return _allTypes ?? (_allTypes = GetAllMrCMSAssemblies().SelectMany(GetLoadableTypes).Distinct().ToHashSet());
        }

        public static HashSet<Type> MappedClasses => GetAllConcreteTypesAssignableFrom<SystemEntity>().FindAll(type =>
                                                                       !type.GetCustomAttributes(typeof(DoNotMapAttribute), true).Any());

        public static HashSet<Type> GetMappedClassesAssignableFrom<T>()
        {
            return MappedClasses.FindAll(type => typeof(T).IsAssignableFrom(type));
        }

        public static HashSet<Type> GetAllConcreteMappedClassesAssignableFrom<T>()
        {
            return GetMappedClassesAssignableFrom<T>().FindAll(type => !type.IsAbstract);
        }

        public static HashSet<Type> GetAllTypesAssignableFrom<T>()
        {
            return GetAllTypes().FindAll(type => typeof(T).IsAssignableFrom(type));
        }

        public static HashSet<Type> GetAllConcreteTypesAssignableFrom<T>()
        {
            return GetAllTypesAssignableFrom<T>().FindAll(type => !type.IsAbstract);
        }

        public static HashSet<Type> GetAllTypesAssignableFrom(Type type)
        {
            return GetAllTypes().FindAll(t => t.IsImplementationOf(type));
        }

        public static IsImplementationOfOpenGenericResult TypeIsImplementationOfOpenGeneric(Type type, Type implementationOfType)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (implementationOfType == null)
            {
                throw new ArgumentNullException(nameof(implementationOfType));
            }

            if (!implementationOfType.GetTypeInfo().IsGenericTypeDefinition)
            {
                throw new ArgumentException("Implementation of type is not an open generic", nameof(implementationOfType));
            }

            var genericType = GetGenericType(type, implementationOfType);
            return new IsImplementationOfOpenGenericResult(genericType);
        }

        private static Type GetGenericType(Type type, Type openGenericType)
        {
            if (openGenericType.GetTypeInfo().IsInterface)
            {
                return GetBaseTypes(type, true)
                    .FirstOrDefault(
                        t2 => t2.GetTypeInfo().GetInterfaces().Any(
                            tInt => tInt.GetTypeInfo().IsGenericType
                                    &&
                                    tInt.GetGenericTypeDefinition() ==
                                    openGenericType));
            }

            return GetBaseTypes(type)
                .FirstOrDefault(
                    t2 =>
                        t2.GetTypeInfo().IsGenericType &&
                        t2.GetGenericTypeDefinition() == openGenericType);
        }


        public struct IsImplementationOfOpenGenericResult
        {
            public IsImplementationOfOpenGenericResult(Type type)
            {
                MatchedType = type;
            }

            public bool IsImplementationOf => MatchedType != null;
            public Type MatchedType { get; }
        }


        public static bool IsImplementationOf(this Type type, Type baseType)
        {
            if (baseType.IsGenericTypeDefinition)
            {
                return baseType.IsInterface
                           ? type.GetBaseTypes(true)
                              .Any(
                                  t2 => t2.GetInterfaces().Any(
                                      tInt => tInt.IsGenericType
                                              &&
                                              tInt.GetGenericTypeDefinition() ==
                                              baseType))
                           : type.GetBaseTypes()
                              .Any(
                                  t2 =>
                                  t2.IsGenericType &&
                                  t2.GetGenericTypeDefinition() == baseType);
            }
            return baseType.IsAssignableFrom(type);
        }

        public static HashSet<Type> GetAllConcreteTypesAssignableFrom(Type t)
        {
            return GetAllTypesAssignableFrom(t).FindAll(type => !type.IsAbstract);
        }

        private static HashSet<Type> GetLoadableTypes(this Assembly assembly)
        {
            var loadableTypes = new HashSet<Type>();
            if (assembly == null)
            {
                return loadableTypes;
            }

            try
            {
                loadableTypes.AddRange(assembly.GetTypes());
            }
            catch (ReflectionTypeLoadException e)
            {
                loadableTypes.AddRange(e.Types.Where(t => t != null));
            }
            return loadableTypes.FindAll(type => !typeof(INHibernateProxy).IsAssignableFrom(type));
        }
        /// <summary>
        /// Search for a method by name and parameter types.  Unlike GetMethod(), does 'loose' matching on generic
        /// parameter types, and searches base interfaces.
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static MethodInfo GetMethodExt(this Type thisType, string name, params Type[] parameterTypes)
        {
            return GetMethodExt(thisType, name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, parameterTypes);
        }

        /// <summary>
        /// Search for a method by name, parameter types, and binding flags.  Unlike GetMethod(), does 'loose' matching on generic
        /// parameter types, and searches base interfaces.
        /// </summary>
        /// <exception cref="AmbiguousMatchException"/>
        public static MethodInfo GetMethodExt(this Type thisType, string name, BindingFlags bindingFlags, params Type[] parameterTypes)
        {
            MethodInfo matchingMethod = null;

            // Check all methods with the specified name, including in base classes
            GetMethodExt(ref matchingMethod, thisType, name, bindingFlags, parameterTypes);

            // If we're searching an interface, we have to manually search base interfaces
            if (matchingMethod == null && thisType.IsInterface)
            {
                foreach (Type interfaceType in thisType.GetInterfaces())
                {
                    GetMethodExt(ref matchingMethod, interfaceType, name, bindingFlags, parameterTypes);
                }
            }

            return matchingMethod;
        }

        private static void GetMethodExt(ref MethodInfo matchingMethod, Type type, string name, BindingFlags bindingFlags, params Type[] parameterTypes)
        {
            // Check all methods with the specified name, including in base classes
            foreach (MethodInfo methodInfo in type.GetMember(name, MemberTypes.Method, bindingFlags))
            {
                // Check that the parameter counts and types match, with 'loose' matching on generic parameters
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                if (parameterInfos.Length == parameterTypes.Length)
                {
                    int i = 0;
                    for (; i < parameterInfos.Length; ++i)
                    {
                        if (!parameterInfos[i].ParameterType.IsSimilarType(parameterTypes[i]))
                        {
                            break;
                        }
                    }
                    if (i == parameterInfos.Length)
                    {
                        if (matchingMethod == null)
                        {
                            matchingMethod = methodInfo;
                        }
                        else
                        {
                            throw new AmbiguousMatchException("More than one matching method found!");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Special type used to match any generic parameter type in GetMethodExt().
        /// </summary>
        public class T
        { }

        /// <summary>
        /// Determines if the two types are either identical, or are both generic parameters or generic types
        /// with generic parameters in the same locations (generic parameters match any other generic paramter,
        /// but NOT concrete types).
        /// </summary>
        private static bool IsSimilarType(this Type thisType, Type type)
        {
            // Ignore any 'ref' types
            if (thisType.IsByRef)
            {
                thisType = thisType.GetElementType();
            }

            if (type.IsByRef)
            {
                type = type.GetElementType();
            }

            // Handle array types
            if (thisType.IsArray && type.IsArray)
            {
                return thisType.GetElementType().IsSimilarType(type.GetElementType());
            }

            // If the types are identical, or they're both generic parameters or the special 'T' type, treat as a match
            if (thisType == type || ((thisType.IsGenericParameter || thisType == typeof(T)) && (type.IsGenericParameter || type == typeof(T))))
            {
                return true;
            }

            // Handle any generic arguments
            if (thisType.IsGenericType && type.IsGenericType)
            {
                Type[] thisArguments = thisType.GetGenericArguments();
                Type[] arguments = type.GetGenericArguments();
                if (thisArguments.Length == arguments.Length)
                {
                    for (int i = 0; i < thisArguments.Length; ++i)
                    {
                        if (!thisArguments[i].IsSimilarType(arguments[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            return false;
        }

        public static T Unproxy<T>(this T document) where T : SystemEntity
        {
            var proxy = document as INHibernateProxy;
            if (proxy != null)
            {
                return (T)proxy.HibernateLazyInitializer.GetImplementation();
            }

            return document;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(this object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(this object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                TypeConverter destinationConverter = GetCustomTypeConverter(destinationType);
                TypeConverter sourceConverter = GetCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                {
                    return destinationConverter.ConvertFrom(null, culture, value);
                }

                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                {
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                }

                if (destinationType.IsEnum && value is int)
                {
                    return Enum.ToObject(destinationType, (int)value);
                }

                if (!destinationType.IsInstanceOfType(value))
                {
                    return Convert.ChangeType(value, destinationType, culture);
                }
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(this object value)
        {
            return (T)To(value, typeof(T));
        }

        public static TypeConverter GetCustomTypeConverter(this Type type)
        {
            if (type == typeof(List<int>))
            {
                return new GenericListTypeConverter<int>();
            }

            if (type == typeof(List<decimal>))
            {
                return new GenericListTypeConverter<decimal>();
            }

            if (type == typeof(List<string>))
            {
                return new GenericListTypeConverter<string>();
            }

            return TypeDescriptor.GetConverter(type);
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type, bool includeSelf = false)
        {
            if (includeSelf)
            {
                yield return type;
            }

            var baseType = type.BaseType;
            while (baseType != null)
            {
                yield return baseType;
                baseType = baseType.BaseType;
            }
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type, Func<Type, bool> filter)
        {
            return type.GetBaseTypes().Where(filter);
        }

        public static Type GetTypeByName(string typeName)
        {
            return GetAllTypes().FirstOrDefault(type => type.FullName == typeName);
        }

        public static Type GetTypeByClassName(string typeName)
        {
            return GetAllTypes().FirstOrDefault(type => type.Name == typeName);
        }

        public static Type GetGenericTypeByName(string type)
        {
            return
                _mrCMSAssemblies.Select(mrCMSAssembly => mrCMSAssembly.GetType(type))
                                .FirstOrDefault(type1 => type1 != null);
        }
        public static Dictionary<Type, Type> GetSimpleInterfaceImplementationPairings()
        {
            var interfaces = GetAllTypes().Where(x => x.GetTypeInfo().IsInterface).Where(x => !x.IsGenericTypeDefinition);

            var singleImplementationInterfaces = interfaces.Where(x => GetAllConcreteTypesAssignableFrom(x).Count(y => !y.IsGenericTypeDefinition) == 1);

            return singleImplementationInterfaces.ToDictionary(x => x, x => GetAllConcreteTypesAssignableFrom(x).First());
        }

        public static HashSet<Type> GetAllOpenGenericInterfaces()
        {
            var interfaces = GetAllTypes().FindAll(x => x.GetTypeInfo().IsInterface);

            return interfaces.FindAll(x => x.IsGenericTypeDefinition);
        }
    }
}