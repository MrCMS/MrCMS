using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Commenting.Tests.Support
{
    public static class RepeatedOperationExtensions
    {
        public static void Times(this int i, Action action)
        {
            for (int j = 0; j < i; j++)
                action();
        }
        public static void Times(this int i, Action<int> action)
        {
            for (int j = 0; j < i; j++)
                action(j);
        }

        public static void SaveAndAddEntityTo<T>(this ISession session, T entity, List<T> entities = null) where T : SystemEntity
        {
            session.Transact(sess => sess.Save(entity));
            if (entities != null) entities.Add(entity);
        }
    }
}