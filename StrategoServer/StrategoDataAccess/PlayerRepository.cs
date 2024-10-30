using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utilities;

namespace StrategoDataAccess
{
    public class PlayerRepository
    {
        private readonly Lazy<StrategoEntities> _context;

        public PlayerRepository(Lazy<StrategoEntities> context)
        {
            _context = context;
        }

        public async Task<Result<Player>> GetPlayerByIdAsync(int playerId)
        {
            try
            {
                var player = await _context.Value.Player.FirstOrDefaultAsync(p => p.Id == playerId);

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

        public async Task<Result<IEnumerable<Player>>> GetPlayerFriendsListAsync(int playerId)
        {
            try
            {
                var result = await _context.Value.Friend
                    .Where(f => f.PlayerId == playerId && f.Status == "accepted")
                    .Join(
                        _context.Value.Player,
                        friend => friend.FriendId,
                        player => player.Id,
                        (friend, player) => player
                    )
                    .ToListAsync();

                if (result.Count == 0)
                {
                   return Result<IEnumerable<Player>>.Failure("No friends found for the given player ID");
                }
                return Result<IEnumerable<Player>>.Success(result);
            } 
            catch (SqlException sqlEx)
            {
                return Result<IEnumerable<Player>>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Player>>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<Result<Player>> UpdatePlayerAsync(Player updatedPlayer, string labelPath, string picturePath)
        {
            using (var transaction = _context.Value.Database.BeginTransaction())
            {
                try
                {
                    var pictureIdResult = await GetPictureIdAsync(picturePath);
                    if (!pictureIdResult.IsSuccess)
                    {
                        return Result<Player>.Failure(pictureIdResult.Error);
                    }

                    var labelIdResult = await GetLabelIdAsync(labelPath);
                    if (!labelIdResult.IsSuccess)
                    {
                        return Result<Player>.Failure(labelIdResult.Error);
                    }

                    var playerInDb = await _context.Value.Player
                        .FirstOrDefaultAsync(p => p.Id == updatedPlayer.Id);

                    if (playerInDb == null)
                    {
                        return Result<Player>.Failure("Player not found.");
                    }

                    playerInDb.Name = updatedPlayer.Name;
                    playerInDb.PictureId = pictureIdResult.Value;
                    playerInDb.IdLabel = labelIdResult.Value;

                    await _context.Value.SaveChangesAsync();

                    transaction.Commit();

                    return Result<Player>.Success(playerInDb);
                }
                catch (DbEntityValidationException dbEx)
                {
                    transaction.Rollback();
                    return Result<Player>.Failure($"Entity validation error: {dbEx.Message}");
                }
                catch (DbUpdateException dbEx)
                {
                    transaction.Rollback();
                    return Result<Player>.Failure($"Database update error: {dbEx.Message}");
                }
                catch (SqlException sqlEx)
                {
                    transaction.Rollback();
                    return Result<Player>.Failure($"Database error: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Result<Player>.Failure($"Unexpected error: {ex.Message}");
                }
            }
        }


        public async Task<Result<Player>> GetPlayerByAccountIdAsync(int accountId)
        {
            try
            {
                var player = await _context.Value.Player.FirstOrDefaultAsync(p => p.AccountId == accountId);

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

        public async Task<Result<int>> GetLabelIdAsync(string labelPath)
        {
            try
            {
                var label = await _context.Value.Label
                    .FirstOrDefaultAsync(l => l.Path == labelPath);

                if (label == null)
                {
                    return Result<int>.Failure("Label path not found.");
                }

                return Result<int>.Success(label.IdLabel);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<Result<int>> GetPictureIdAsync(string picturePath)
        {
            try
            {
                var picture = await _context.Value.Pictures
                    .FirstOrDefaultAsync(p => p.path == picturePath);

                if (picture == null)
                {
                    return Result<int>.Failure("Picture path not found.");
                }

                return Result<int>.Success(picture.IdPicture);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<Result<string>> GetPicturePathByIdAsync(int pictureId)
        {
            try
            {
                var picture = await _context.Value.Pictures
                    .FirstOrDefaultAsync(p => p.IdPicture == pictureId);

                if (picture == null)
                {
                    return Result<string>.Failure("Picture not found.");
                }

                return Result<string>.Success(picture.path);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<Result<string>> GetLabelPathByIdAsync(int labelId)
        {
            try
            {
                var label = await _context.Value.Label
                    .FirstOrDefaultAsync(l => l.IdLabel == labelId);

                if (label == null)
                {
                    return Result<string>.Failure("Label not found.");
                }

                return Result<string>.Success(label.Path);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Unexpected error: {ex.Message}");
            }
        }

    }
}
