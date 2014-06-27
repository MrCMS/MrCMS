using System;

namespace MrCMS.Models
{
    public class RequiredFieldException : Exception
    {
        public RequiredFieldException(string message)
            : base(message)
        {

        }
    }
}