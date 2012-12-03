using System.Collections.Generic;
using System.Web;

namespace MrCMS.Web.Tests.Stubs
{
    public class FakeHttpFileCollectionBase : HttpFileCollectionBase
    {
        private Dictionary<string, HttpPostedFileBase> _fileList;

        public FakeHttpFileCollectionBase(Dictionary<string, HttpPostedFileBase> fileList)
        {
            _fileList = fileList;
        }

        public Dictionary<string, HttpPostedFileBase> FileList
        {
            get { return _fileList; }
        }

        public override HttpPostedFileBase this[string name]
        {
            get { return FileList[name]; }
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return FileList.Keys.GetEnumerator();
        }
    }
}