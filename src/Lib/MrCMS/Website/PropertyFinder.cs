using System.Linq.Expressions;
using System.Reflection;

namespace MrCMS.Website
{
    public static class PropertyFinder
    {
        public static PropertyInfo GetProperty(Expression expression)
        {
            return expression is LambdaExpression && (expression as LambdaExpression).Body is MemberExpression &&
                   ((expression as LambdaExpression).Body as MemberExpression).Member is PropertyInfo
                ? ((expression as LambdaExpression).Body as MemberExpression).Member as PropertyInfo
                : null;
        }
    }
}