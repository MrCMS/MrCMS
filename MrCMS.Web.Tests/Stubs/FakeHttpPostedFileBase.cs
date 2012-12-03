using System.IO;
using System.Web;

namespace MrCMS.Web.Tests.Stubs
{
    public class FakeHttpPostedFileBase : HttpPostedFileBase
    {
        private readonly Stream _inputStream;
        private readonly string _fileName;
        private readonly string _contentType;
        private readonly int _contentLength;

        public FakeHttpPostedFileBase(Stream inputStream, string fileName, string contentType, int contentLength)
        {
            _inputStream = inputStream;
            _fileName = fileName;
            _contentType = contentType;
            _contentLength = contentLength;
        }

        public override Stream InputStream
        {
            get { return _inputStream; }
        }

        public override string FileName
        {
            get { return _fileName; }
        }

        public override string ContentType
        {
            get { return _contentType; }
        }

        public override int ContentLength
        {
            get { return _contentLength; }
        }
    }
}