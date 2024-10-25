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

        public async Task<Result<string>> CreateAccountAsync(string email, string hashedPassword, string playerName)
        {
            var existenceCheckResult = await AlreadyExistentAccountAsync(email);

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
                    await _context.Value.SaveChangesAsync();

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
                    await _context.Value.SaveChangesAsync();

                    transaction.Commit();

                    return Result<string>.Success("Account and player created successfully");
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
        }

        public async Task<Result<int>> ValidateCredentialsAsync(string email, string hashedPassword)
        {
            try
            {
                var account = await _context.Value.Account.FirstOrDefaultAsync(a => a.mail == email && a.password == hashedPassword);

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

        public async Task<Result<bool>> AlreadyExistentAccountAsync(string email)
        {
            try
            {
                bool exists = await _context.Value.Account.AnyAsync(a => a.mail == email);
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

    }
}
