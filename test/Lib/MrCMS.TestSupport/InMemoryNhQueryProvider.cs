using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Type;

namespace MrCMS.TestSupport
{
    /// <summary>
    /// Runs LINQ queries in memory while satisfying NHibernate's async LINQ extensions
    /// (ToListAsync, FirstOrDefaultAsync, ...), which refuse providers that are not INhQueryProvider.
    /// </summary>
    public class InMemoryNhQueryProvider : INhQueryProvider
    {
        private readonly IQueryProvider _inner;

        private InMemoryNhQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public static IQueryable<T> Wrap<T>(IEnumerable<T> source)
        {
            var queryable = source.AsQueryable();
            return new Queryable<T>(new InMemoryNhQueryProvider(queryable.Provider), queryable.Expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = expression.Type.GetGenericArguments().First();
            return (IQueryable)Activator.CreateInstance(typeof(Queryable<>).MakeGenericType(elementType), this,
                expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
            new Queryable<TElement>(this, expression);

        public object Execute(Expression expression) => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) =>
            Task.FromResult(Execute<TResult>(expression));

        public int ExecuteDml<T>(QueryMode queryMode, Expression expression) => throw new NotSupportedException();

        public Task<int> ExecuteDmlAsync<T>(QueryMode queryMode, Expression expression,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression) =>
            throw new NotSupportedException();

        public IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression) =>
            throw new NotSupportedException();

        public void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression,
            IDictionary<string, Tuple<object, IType>> parameters) => throw new NotSupportedException();

        private class Queryable<T> : IOrderedQueryable<T>
        {
            private readonly InMemoryNhQueryProvider _provider;

            public Queryable(InMemoryNhQueryProvider provider, Expression expression)
            {
                _provider = provider;
                Expression = expression;
            }

            public Type ElementType => typeof(T);
            public Expression Expression { get; }
            public IQueryProvider Provider => _provider;

            public IEnumerator<T> GetEnumerator() =>
                _provider._inner.CreateQuery<T>(Expression).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
