using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoDataAccess
{
    public class AccountRepository
    {
        private readonly StrategoEntities _context;

        public AccountRepository(StrategoEntities context)
        {
            _context = context;
        }

        public async Task CreateAccountAsync(string email, string hashedPassword)
        {
            if (await AlreadyExistentAccountAsync(email))
            {
                throw new InvalidOperationException("Account already exists");
            }

            var newAccount = new Account
            {
                mail = email,
                password = hashedPassword
            };

            try
            {
                _context.Account.Add(newAccount);
                await _context.SaveChangesAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Data base error", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error has ocurred", ex);
            }
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string hashedPassword)
        {
            var account = await _context.Account.FirstOrDefaultAsync(a => a.mail == email && a.password == hashedPassword);
            return account != null;
        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            return await _context.Account.FirstOrDefaultAsync(a => a.mail == email);
        }

        public async Task<bool> AlreadyExistentAccountAsync(string email)
        {
            return await _context.Account.AnyAsync(a => a.mail == email);
        }
    }
}
