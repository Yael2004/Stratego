using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategoApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTests
{
    [TestClass]
    public class ValidationsTests
    {
        [TestMethod]
        public void Test_IsValidEmail_ValidEmail_ReturnsTrue()
        {
            var email = "test.email@example.com";

            var result = Validations.IsValidEmail(email);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test_IsValidEmail_InvalidEmailWithoutDomain_ReturnsFalse()
        {
            var email = "test.email@.com";

            var result = Validations.IsValidEmail(email);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Test_IsValidEmail_EmptyEmail_ReturnsFalse()
        {
            var email = "";

            var result = Validations.IsValidEmail(email);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Test_IsValidPassword_ValidPassword_ReturnsTrue()
        {
            var password = "Password123!";

            var result = Validations.IsValidPassword(password);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test_IsValidPassword_InvalidPasswordWithoutSpecialCharacter_ReturnsFalse()
        {
            var password = "Password123";

            var result = Validations.IsValidPassword(password);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Test_IsValidPassword_PasswordTooShort_ReturnsFalse()
        {
            var password = "P@ss1";

            var result = Validations.IsValidPassword(password);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Test_IsValidUsername_ValidUsername_ReturnsTrue()
        {
            var username = "User123";

            var result = Validations.IsValidUsername(username);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Test_IsValidUsername_InvalidUsernameWithSpecialCharacter_ReturnsFalse()
        {
            var username = "User@123";
            var result = Validations.IsValidUsername(username);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Test_IsValidUsername_UsernameTooShort_ReturnsFalse()
        {
            var username = "Usr";

            var result = Validations.IsValidUsername(username);

            Assert.IsFalse(result);
        }
    }
}
