using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MrCMS.Services
{
    public class MessageTemplateProcessor : IMessageTemplateProcessor
    {
        public static List<string> GetTokens<T>()
        {
            var type1 = typeof(T);

            // (simple) Properties
            Type[] acceptedTypes = { typeof(String), typeof(Int32), typeof(Decimal), typeof(DateTime), typeof(Boolean), typeof(bool), typeof(float) };
            var list = (from item in type1.GetProperties()
                        where acceptedTypes.Any(x => x == item.PropertyType)
                        select item.Name).ToList();
            // Methods
            list.AddRange(type1.GetMethods().Where(info => !info.IsSpecialName).Select(info => string.Format("{0}()", info.Name)));

            // Extension Methods
            list.AddRange(from type in type1.Assembly.GetTypes()
                          where type.IsSealed && !type.IsGenericType && !type.IsNested
                          from method in type.GetMethods(BindingFlags.Static
                                                         | BindingFlags.Public | BindingFlags.NonPublic)
                          where method.IsDefined(typeof(ExtensionAttribute), false)
                          where method.GetParameters()[0].ParameterType == type1
                          select string.Format("{0}()", method.Name));
            return list;
        }


        public string ReplaceTokensAndMethods<T>(T tokenProvider, string template)
        {
            var processedTemplate = ReplaceTokens(tokenProvider, template);
            processedTemplate = ReplaceMethods(tokenProvider, processedTemplate);
            processedTemplate = ReplaceExtensionMethods(tokenProvider, processedTemplate);
            return processedTemplate;
        }

        public string ReplaceExtensionMethods<T>(T tokenProvider, string template)
        {
            var query = from type in tokenProvider.GetType().Assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                                                       | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == tokenProvider.GetType()
                        select method;

            var replacements = new Dictionary<string, string>();

            foreach (Match item in GetRegexMatches(template))
            {
                if (!item.Value.Contains("()")) continue;

                var cleanMethodName = item.Value.Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", ""); ;
                var method = query.SingleOrDefault(x => x.Name.Contains(cleanMethodName));
                if (method != null)
                    replacements.Add(method.Name + "()", method.Invoke(tokenProvider, new object[] { tokenProvider }).ToString());
            }

            return ReplaceTokensForString(template, replacements);
        }

        public string ReplaceMethods<T>(T tokenProvider, string template)
        {
            var methods = tokenProvider.GetType().GetMethods();
            var replacements = new Dictionary<string, string>();

            foreach (Match item in GetRegexMatches(template))
            {
                if (!item.Value.Contains("()")) continue;

                var cleanMethodName = item.Value.Replace("{", "").Replace("}", "").Replace("(", "").Replace(")", ""); ;
                var method = methods.SingleOrDefault(x => x.Name.Contains(cleanMethodName));
                if (method != null)
                    replacements.Add(method.Name + "()", method.Invoke(tokenProvider, null).ToString());
            }

            return ReplaceTokensForString(template, replacements);
        }

        public string ReplaceTokens<T>(T tokenProvider, string template)
        {
            Type[] acceptedTypes = { typeof(String), typeof(Int32), typeof(Decimal), typeof(DateTime), typeof(Boolean), typeof(bool), typeof(float) };
            var replacements = new Dictionary<string, string>();
            foreach (var item in tokenProvider.GetType().GetProperties())
            {
                if (!acceptedTypes.Any(x => x == item.PropertyType)) continue;

                var value = item.GetValue(tokenProvider, null);
                if (value != null)
                {
                    replacements.Add(item.Name, value.ToString());
                }
            }

            return ReplaceTokensForString(template, replacements);
        }

        public string ReplaceTokensForString(string template, Dictionary<string, string> replacements)
        {
            var regex = new Regex(@"\{([^}]+)}");
            return (regex.Replace(template, delegate(Match match)
                {
                    var key = match.Groups[1].Value;
                    var replacement = replacements.ContainsKey(key) ? replacements[key] : match.Value;
                    return (replacement);
                }));
        }

        public MatchCollection GetRegexMatches(string template)
        {
            var regex = new Regex(@"\{([^}]+)}");
            return regex.Matches(template);
        }
    }
}