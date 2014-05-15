using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using MrCMS.Helpers.Validation;
using Xunit;

namespace MrCMS.Tests.Helpers.Validation
{
    public class EmailValidatorTests
    {
        [Fact]
        public void StandardEmailPassesValidation()
        {
            Regex.IsMatch("test@example.com", EmailValidator.EmailValidatorPattern).Should().BeTrue();
        }

        [Fact]
        public void InvalidEmailFailsValidation()
        {
            Regex.IsMatch("test.example.com", EmailValidator.EmailValidatorPattern).Should().BeFalse();
        }

        [Fact]
        public void EmailWithPlusShouldPassValidation()
        {
            Regex.IsMatch("test+testing@example.com", EmailValidator.EmailValidatorPattern).Should().BeTrue();
        }
    }
}
