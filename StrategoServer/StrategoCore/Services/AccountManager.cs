using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoCore.Services
{
    public class AccountManager
    {
        private readonly AccountRepository _accountRepository;

        public AccountManager(AccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public void CreateAccount(string email, string hashedPassword)
        {
            try
            {
                _accountRepository.CreateAccountAsync(email, hashedPassword);
            }
            catch (Exception ex)
            {
                throw new CoreException("Error al crear la cuenta en la capa de lógica de negocio", ex);
            }
        }
    }
}
