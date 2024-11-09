using StrategoDataAccess;
using System;
using System.Collections.Generic;
using Utilities;

namespace StrategoServices.Logic
{
    public class PasswordManager
    {
        private readonly Lazy<AccountRepository> _accountRepository;
        private readonly Dictionary<string, string> _verificationCodes = new Dictionary<string, string>();

        public PasswordManager(Lazy<AccountRepository> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Result<bool> AlreadyExistentAccount(string email)
        {
            return _accountRepository.Value.AlreadyExistentAccount(email);
        }

        public Result<string> ChangePassword(string email, string newHashedPassword)
        {
            return _accountRepository.Value.ChangePassword(email, newHashedPassword);
        }

        public string GenerateVerificationCode(string email)
        {
            var code = GenerateRandomCode();
            _verificationCodes[email] = code;
            return code;
        }

        public Result<bool> ValidateVerificationCode(string email, string code)
        {
            if (_verificationCodes.TryGetValue(email, out var storedCode) && storedCode == code)
            {
                _verificationCodes.Remove(email);
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure("Invalid verification code");
        }

        private string GenerateRandomCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
