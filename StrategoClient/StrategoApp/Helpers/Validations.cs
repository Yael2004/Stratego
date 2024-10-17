using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoApp.Helpers
{
    public static class Validations
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern);
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,32}$";
            return System.Text.RegularExpressions.Regex.IsMatch(password, passwordPattern);
        }

        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            string usernamePattern = @"^[a-zA-Z0-9]{4,16}$";
            return System.Text.RegularExpressions.Regex.IsMatch(username, usernamePattern);
        }
    }
}
