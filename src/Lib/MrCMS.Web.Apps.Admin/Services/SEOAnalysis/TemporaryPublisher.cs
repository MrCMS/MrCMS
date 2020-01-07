using System;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Services.SEOAnalysis
{
    public class TemporaryPublisher : ITemporaryPublisher //: IDisposable
    {
        private readonly IRepository<Webpage> _repository;

        public TemporaryPublisher(IRepository<Webpage> repository)
        {
            _repository = repository;
        }

        public async Task<IAsyncDisposable> TemporarilyPublish(Webpage webpage)
        {
            if (!webpage.Published)
            {
                webpage.Published = true;
                await _repository.Update(webpage);
                return new UnpublishOnDispose(_repository, webpage);
            }
            return new DoNothing();
        }

        private class DoNothing : IAsyncDisposable
        {
            public ValueTask DisposeAsync()
            {
                return new ValueTask();
            }
        }

        private class UnpublishOnDispose : IAsyncDisposable
        {
            private readonly IRepository<Webpage> _repository;
            private readonly Webpage _webpage;

            public UnpublishOnDispose(IRepository<Webpage> repository, Webpage webpage)
            {
                _repository = repository;
                _webpage = webpage;
            }

            public async ValueTask DisposeAsync()
            {
                _webpage.Published = false;
                await _repository.Update(_webpage);
            }
        }
    }
}