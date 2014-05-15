using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MrCMS.Helpers.Validation
{
    /// <summary>
    /// validate email in something@somthing.something with a maximum TLD of 6 characters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class EmailValidator : RegularExpressionAttribute
    {
        private const string _pattern = @"^\b[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}\b$";

        static EmailValidator()
        {
            // necessary to enable client side validation
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(EmailValidator), typeof(RegularExpressionAttributeAdapter));
        }

        public EmailValidator()
            : base(EmailValidatorPattern)
        {
            ErrorMessage = "Please enter a valid email.";
        }

        public static string EmailValidatorPattern
        {
            get { return _pattern; }
        }
    }

}
