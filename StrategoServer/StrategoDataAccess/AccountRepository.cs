﻿using System;
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

        public async Task<Result<string>> CreateAccountAsync(string email, string hashedPassword)
        {
            if (await AlreadyExistentAccountAsync(email))
            {
                return Result<string>.Failure("Account already exists");
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
                return Result<string>.Success("Account created successfully");
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

        public async Task<Result<string>> ValidateCredentialsAsync(string email, string hashedPassword)
        {
            try
            {
                var account = await _context.Account.FirstOrDefaultAsync(a => a.mail == email && a.password == hashedPassword);
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
