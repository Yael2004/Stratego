using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoCore.Services
{
    public class AccountManager
    {
        private readonly AccountRepository _accountRepository;

        public AccountManager(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<Result<string>> CreateAccountAsync(string email, string password)
        {
            var result = await _accountRepository.CreateAccountAsync(email, password);
            return result;
        }

        public async Task<Result<string>> LogInAccountAsync(string email, string password)
        {
            var result = await _accountRepository.ValidateCredentialsAsync(email, password);
            return result;
        }

    }
}
