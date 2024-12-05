using log4net;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(AccountRepository));

        public virtual Result<string> CreateAccount(string email, string hashedPassword, string playerName)
        {
            var existenceCheckResult = AlreadyExistentAccount(email);

            if (!existenceCheckResult.IsSuccess)
            {
                log.Warn($"Account creation failed: {existenceCheckResult.Error}");
                return Result<string>.Failure(existenceCheckResult.Error);
            }

            if (existenceCheckResult.Value)
            {
                return Result<string>.Failure("Account already exists");
            }

            using (var context = new StrategoEntities())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var newAccount = new Account
                    {
                        mail = email,
                        password = hashedPassword
                    };

                    context.Account.Add(newAccount);
                    context.SaveChanges();

                    const int defaultPictureId = 1;
                    const int defaultLabelId = 1;

                    var newPlayer = new Player
                    {
                        Name = playerName,
                        PictureId = defaultPictureId,
                        IdLabel = defaultLabelId,
                        AccountId = newAccount.IdAccount
                    };

                    context.Player.Add(newPlayer);
                    context.SaveChanges();

                    var playerStatistics = new Games
                    {
                        WonGames = 0,
                        DeafeatGames = 0,
                        AccountId = newAccount.IdAccount
                    };

                    context.Games.Add(playerStatistics);
                    context.SaveChanges();

                    transaction.Commit();
                    return Result<string>.Success("Account and player created successfully");
                }
                catch (DbEntityValidationException dbEx)
                {
                    transaction.Rollback();
                    log.Error("Entity validation error during account creation", dbEx);
                    return Result<string>.Failure($"Entity validation error: {dbEx.Message}");
                }
                catch (SqlException sqlEx)
                {
                    transaction.Rollback();
                    log.Error("Database error during account creation", sqlEx);
                    return Result<string>.Failure($"Database error: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Unexpected error during account creation", ex);
                    return Result<string>.Failure($"Unexpected error: {ex.Message}");
                }
            }
        }

        public virtual Result<int> ValidateCredentials(string email, string hashedPassword)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var account = context.Account.FirstOrDefault(a => a.mail == email && a.password == hashedPassword);

                    if (account == null)
                    {
                        return Result<int>.Failure("Invalid credentials");
                    }

                    return Result<int>.Success(account.IdAccount);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                log.Error("Entity validation error during credential validation", dbEx);
                return Result<int>.Failure($"Entity validation error: {dbEx.Message}");
            }
            catch (SqlException sqlEx)
            {
                log.Error("Database error during credential validation", sqlEx);
                return Result<int>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error during credential validation", ex);
                return Result<int>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public virtual Result<bool> AlreadyExistentAccount(string email)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    bool exists = context.Account.Any(a => a.mail == email);
                    return Result<bool>.Success(exists);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error("Database error during account existence check", sqlEx);
                return Result<bool>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error during account existence check", ex);
                return Result<bool>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public virtual Result<string> ChangePassword(string email, string newHashedPassword)
        {
            using (var context = new StrategoEntities())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var existenceCheckResult = AlreadyExistentAccount(email);
                    if (!existenceCheckResult.IsSuccess)
                    {
                        log.Warn($"Password change failed: {existenceCheckResult.Error}");
                        return Result<string>.Failure(existenceCheckResult.Error);
                    }

                    if (!existenceCheckResult.Value)
                    {
                        return Result<string>.Failure("Account does not exist");
                    }

                    var account = context.Account.FirstOrDefault(a => a.mail == email);
                    if (account == null)
                    {
                        return Result<string>.Failure("Account not found");
                    }

                    account.password = newHashedPassword;
                    context.SaveChanges();

                    transaction.Commit();
                    return Result<string>.Success("Password changed successfully");
                }
                catch (DbEntityValidationException dbEx)
                {
                    transaction.Rollback();
                    log.Error("Entity validation error during password change", dbEx);
                    return Result<string>.Failure($"Entity validation error: {dbEx.Message}");
                }
                catch (SqlException sqlEx)
                {
                    transaction.Rollback();
                    log.Error("Database error during password change", sqlEx);
                    return Result<string>.Failure($"Database error: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error("Unexpected error during password change", ex);
                    return Result<string>.Failure($"Unexpected error: {ex.Message}");
                }
            }
        }
    }
}
