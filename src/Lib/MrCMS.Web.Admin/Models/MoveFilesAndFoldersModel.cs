using System.Collections.Generic;

namespace MrCMS.Web.Admin.Models
{
    public class MoveFilesAndFoldersModel
    {
        public int Folder { get; set; }
        public IEnumerable<int> Files { get; set; }
        public IEnumerable<int> Folders { get; set; }
    }
    public class DeleteFilesAndFoldersModel
    {
        public IEnumerable<int> Files { get; set; }
        public IEnumerable<int> Folders { get; set; }
    }
}