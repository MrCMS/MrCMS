namespace MrCMS.Installation
{
    public struct RequiredCheck
    {
        public RequiredCheck(bool checkRead, bool checkWrite, bool checkModify, bool checkDelete) : this()
        {
            CheckRead = checkRead;
            CheckWrite = checkWrite;
            CheckModify = checkModify;
            CheckDelete = checkDelete;
        }

        public bool CheckRead { get; set; }

        public bool CheckWrite { get; set; }

        public bool CheckModify { get; set; }

        public bool CheckDelete { get; set; }
    }
}