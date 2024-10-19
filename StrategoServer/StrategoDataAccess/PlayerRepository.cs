using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoDataAccess
{
    public class PlayerRepository
    {
        private readonly StrategoEntities _context;

        public PlayerRepository(StrategoEntities context)
        {
            _context = context;
        }

        public async Task<Result<Player>> GetPlayerByIdAsync(int playerId)
        {
            try
            {
                var player = await _context.Player.FirstOrDefaultAsync(p => p.Id == playerId);

                if (player == null)
                {
                    return Result<Player>.Failure("Player not found");
                }

                return Result<Player>.Success(player);
            }
            catch (SqlException sqlEx)
            {
                return Result<Player>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<Player>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            return await _context.Player.ToListAsync();
        }

        public async Task<bool> UpdatePlayerAsync(int playerId, string newName)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var player = await _context.Player.FirstOrDefaultAsync(p => p.Id == playerId);

                    if (player == null)
                    {
                        return false;  
                    }

                    player.Name = newName;

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return true;
                }
                catch (DbEntityValidationException dbEx)
                {
                    transaction.Rollback();  
                    Console.WriteLine($"Error de validación de entidad: {dbEx.Message}");
                    return false;
                }
                catch (DbUpdateException dbEx)
                {
                    transaction.Rollback();  
                    Console.WriteLine($"Error al actualizar la base de datos: {dbEx.Message}");
                    return false;
                }
                catch (SqlException sqlEx)
                {
                    transaction.Rollback();  
                    Console.WriteLine($"Error en la base de datos SQL: {sqlEx.Message}");
                    return false;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();  
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<Result<Player>> GetPlayerByAccountIdAsync(int accountId)
        {
            try
            {
                var player = await _context.Player.FirstOrDefaultAsync(p => p.AccountId == accountId);

                if (player == null)
                {
                    return Result<Player>.Failure("Player not found for the given account ID");
                }

                return Result<Player>.Success(player);
            }
            catch (SqlException sqlEx)
            {
                return Result<Player>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<Player>.Failure($"Unexpected error: {ex.Message}");
            }
        }

    }
}
