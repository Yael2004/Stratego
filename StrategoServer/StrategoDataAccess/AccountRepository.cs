using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoDataAccess
{
    public class AccountRepository
    {
        private readonly StrategoEntities _context;

        public AccountRepository(StrategoEntities context)
        {
            _context = context;
        }

        public Result<string> CreateAccount(string email, string hashedPassword, string playerName)
        {
            if (AlreadyExistentAccount(email))
            {
                return Result<string>.Failure("Account already exists");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var newAccount = new Account
                    {
                        mail = email,
                        password = hashedPassword
                    };

                    _context.Account.Add(newAccount);
                    _context.SaveChanges(); 

                    var newPlayer = new Player
                    {
                        Name = playerName,
                        AccountId = newAccount.IdAccount
                    };

                    _context.Player.Add(newPlayer);
                    _context.SaveChanges(); 

                    transaction.Commit();

                    return Result<string>.Success("Account and player created successfully");
                }
                catch (DbEntityValidationException dbEx)
                {
                    transaction.Rollback();
                    return Result<string>.Failure($"Entity validation error: {dbEx.Message}");
                }
                catch (SqlException sqlEx)
                {
                    transaction.Rollback();
                    return Result<string>.Failure($"Database error: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<string>.Failure($"Unexpected error: {ex.Message}");
                }
            }
        }

        public Result<string> ValidateCredentials(string email, string hashedPassword)
        {
            try
            {
                var account = _context.Account.FirstOrDefault(a => a.mail == email && a.password == hashedPassword);
                return account != null ? Result<string>.Success("Credentials are valid") : Result<string>.Failure("Invalid credentials");
            }
            catch (DbEntityValidationException dbEx)
            {
                return Result<string>.Failure($"Entity validation error: {dbEx.Message}");
            }
            catch (SqlException sqlEx)
            {
                return Result<string>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public Account GetAccountByEmail(string email)
        {
            return _context.Account.FirstOrDefault(a => a.mail == email);
        }

        public bool AlreadyExistentAccount(string email)
        {
            return _context.Account.Any(a => a.mail == email);
        }
    }
}
