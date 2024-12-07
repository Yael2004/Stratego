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
                    log.Error(Messages.EntityValidationError, dbEx);
                    return Result<string>.DataBaseError($"{Messages.EntityValidationError}: {dbEx.Message}");
                }
                catch (SqlException sqlEx)
                {
                    transaction.Rollback();
                    log.Error(Messages.DataBaseError, sqlEx);
                    return Result<string>.DataBaseError($"{Messages.DataBaseError}: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error(Messages.UnexpectedError, ex);
                    return Result<string>.Failure($"{Messages.UnexpectedError}: {ex.Message}");
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
                log.Error(Messages.EntityValidationError, dbEx);
                return Result<int>.DataBaseError($"{Messages.EntityValidationError}: {dbEx.Message}");
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<int>.DataBaseError($"{Messages.DataBaseError}: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<int>.Failure($"{Messages.UnexpectedError}: {ex.Message}");
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
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<bool>.DataBaseError($"{Messages.DataBaseError}: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<bool>.Failure($"{Messages.UnexpectedError}: {ex.Message}");
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
                    log.Error(Messages.EntityValidationError, dbEx);
                    return Result<string>.DataBaseError($"{Messages.EntityValidationError} : {dbEx.Message}");
                }
                catch (SqlException sqlEx)
                {
                    transaction.Rollback();
                    log.Error(Messages.DataBaseError, sqlEx);
                    return Result<string>.DataBaseError($"{Messages.DataBaseError}   : {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    log.Error(Messages.UnexpectedError, ex);
                    return Result<string>.Failure($"{Messages.UnexpectedError}   : {ex.Message}");
                }
            }
        }
    }
}
