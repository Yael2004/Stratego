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
        private readonly PlayerRepository _playerRepository;

        public AccountManager(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Task<Result<string>> CreateAccountAsync(string email, string password, string playername)
        {
            var result = _accountRepository.CreateAccountAsync(email, password, playername);
            return result;
        }

        public Task<Result<int>> LogInAccountAsync(string email, string password)
        {
            var result = _accountRepository.ValidateCredentialsAsync(email, password);
            return result;
        }

        public async Task<Result<Player>> GetLogInAccountAsync(int accountId)
        {
            var result = await _playerRepository.GetPlayerByAccountIdAsync(accountId);
            return result;
        }

    }
}
