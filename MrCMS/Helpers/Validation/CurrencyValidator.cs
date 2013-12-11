using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MrCMS.Helpers.Validation
{
    /// <summary>
    /// This validator will validate input in 10.00, 10,00, 10,000.00, 1234567.89, 1.234.567,89. You should still validator on server side for culture aware formatting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CurrencyValidator : RegularExpressionAttribute
    {
        private const string pattern = @"^([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$";

        static CurrencyValidator()
        {
            // necessary to enable client side validation
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(CurrencyValidator), typeof(RegularExpressionAttributeAdapter));
        }

        public CurrencyValidator()
            : base(pattern)
        {
            ErrorMessage = "This field must be in a currency format.";
        }
    }

}
