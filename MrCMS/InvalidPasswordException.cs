using System;

namespace MrCMS
{
    [Serializable]
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException(string message)
            : base(message)
        {

        }
    }
}