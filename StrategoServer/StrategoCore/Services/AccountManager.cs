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

        public Result<string> CreateAccount(string email, string password, string playername)
        {
            var result = _accountRepository.CreateAccount(email, password, playername);
            return result;
        }

        public Result<string> LogInAccount(string email, string password)
        {
            var result = _accountRepository.ValidateCredentials(email, password);
            return result;
        }

    }
}
