using System;
using MrCMS.Services;

namespace MrCMS.Tests.TestSupport
{
    public class TestGetNow : IGetNow
    {
        private readonly DateTime _now;

        public TestGetNow(DateTime now)
        {
            _now = now;
        }

        public DateTime Get()
        {
            return _now;
        }
    }
}