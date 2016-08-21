using System;
using MrCMS.Website;

namespace MrCMS.Services
{
    public interface IGetNow
    {
        DateTime Get();
    }

    public class GetNow : IGetNow
    {
        public DateTime Get()
        {
            // todo: reimplement without CuurentRequestData
            return CurrentRequestData.Now;
        }
    }
}