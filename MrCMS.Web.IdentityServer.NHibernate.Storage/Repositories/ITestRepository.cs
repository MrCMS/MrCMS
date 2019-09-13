using System;
using System.Collections.Generic;
using System.Text;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Repositories
{
    public interface ITestRepository
    {
        string GetTestValue();

    }

    public class TestRepository : ITestRepository
    {
        public string GetTestValue()
        {
            return "My God is Faithful";
        }
    }
}
