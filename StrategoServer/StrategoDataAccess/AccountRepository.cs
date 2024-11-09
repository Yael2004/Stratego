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
        private readonly Lazy<StrategoEntities> _context;

        public AccountRepository(Lazy<StrategoEntities> context)
        {
            _context = context;
        }

        public Result<string> CreateAccount(string email, string hashedPassword, string playerName)
        {
            var existenceCheckResult = AlreadyExistentAccount(email);

            if (!existenceCheckResult.IsSuccess)
            {
                return Result<string>.Failure(existenceCheckResult.Error);
            }

            if (existenceCheckResult.Value)
            {
                return Result<string>.Failure("Account already exists");
            }

            using (var transaction = _context.Value.Database.BeginTransaction())
            {
                try
                {
                    var newAccount = new Account
                    {
                        mail = email,
                        password = hashedPassword
                    };

                    _context.Value.Account.Add(newAccount);
                    _context.Value.SaveChanges();

                    const int defaultPictureId = 1;
                    const int defaultLabelId = 1;

                    var newPlayer = new Player
                    {
                        Name = playerName,
                        PictureId = defaultPictureId,  
                        IdLabel = defaultLabelId,      
                        AccountId = newAccount.IdAccount
                    };

                    _context.Value.Player.Add(newPlayer);
                    _context.Value.SaveChanges();

                    var playerStatistics = new Games
                    {
                        WonGames = 0,
                        DeafeatGames = 0,
                        AccountId = newAccount.IdAccount
                    };

                    _context.Value.Games.Add(playerStatistics);
                    _context.Value.SaveChanges();

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

        public Result<int> ValidateCredentials(string email, string hashedPassword)
        {
            try
            {
                var account = _context.Value.Account.FirstOrDefault(a => a.mail == email && a.password == hashedPassword);

                if (account == null)
                {
                    return Result<int>.Failure("Invalid credentials");
                }

                return Result<int>.Success(account.IdAccount);
            }
            catch (DbEntityValidationException dbEx)
            {
                return Result<int>.Failure($"Entity validation error: {dbEx.Message}");
            }
            catch (SqlException sqlEx)
            {
                return Result<int>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public Result<bool> AlreadyExistentAccount(string email)
        {
            try
            {
                bool exists = _context.Value.Account.Any(a => a.mail == email);
                return Result<bool>.Success(exists);
            }
            catch (SqlException sqlEx)
            {
                return Result<bool>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public Result<string> ChangePassword(string email, string newHashedPassword)
        {
            using (var transaction = _context.Value.Database.BeginTransaction())
            {
                try
                {
                    var existenceCheckResult = AlreadyExistentAccount(email);
                    if (!existenceCheckResult.IsSuccess)
                    {
                        return Result<string>.Failure(existenceCheckResult.Error);
                    }

                    if (!existenceCheckResult.Value)
                    {
                        return Result<string>.Failure("Account does not exist");
                    }

                    var account = _context.Value.Account.FirstOrDefault(a => a.mail == email);
                    if (account == null)
                    {
                        return Result<string>.Failure("Account not found");
                    }

                    account.password = newHashedPassword;
                    _context.Value.SaveChanges();

                    transaction.Commit();

                    return Result<string>.Success("Password changed successfully");
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

    }
}
