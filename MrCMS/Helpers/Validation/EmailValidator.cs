using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MrCMS.Helpers.Validation
{
    /// <summary>
    /// validate email in something@somthing.something with a maximum TLD of 6 characters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EmailValidator : RegularExpressionAttribute
    {
        private const string _pattern = @"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?";

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
