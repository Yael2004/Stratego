using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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

        public virtual Result<Player> GetOtherPlayerById(int playerId)
        {
            try
            {
                var player = _context.Value.Player.FirstOrDefault(p => p.Id == playerId);

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

        public virtual Result<bool> IsFriend(int playerId, int otherPlayerId)
        {
            try
            {
                var isFriend = _context.Value.Friend.Any(f =>
                    (f.PlayerId == playerId && f.FriendId == otherPlayerId && f.Status == "friend") ||
                    (f.PlayerId == otherPlayerId && f.FriendId == playerId && f.Status == "friend"));

                return Result<bool>.Success(isFriend);
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

        public virtual Result<IEnumerable<Player>> GetPlayerFriendsList(int playerId)
        {
            try
            {
                var result = _context.Value.Friend
                    .Where(f => f.PlayerId == playerId && f.Status == "accepted")
                    .Join
                    (
                        _context.Value.Player,
                        friend => friend.FriendId,
                        player => player.Id,
                        (friend, player) => player
                    )
                    .ToList();

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

        public virtual Result<Player> UpdatePlayer(Player updatedPlayer, string labelPath, string picturePath)
        {
            using (var transaction = _context.Value.Database.BeginTransaction())
            {
                try
                {
                    var pictureIdResult = GetPictureId(picturePath);
                    if (!pictureIdResult.IsSuccess)
                    {
                        return Result<Player>.Failure(pictureIdResult.Error);
                    }

                    var labelIdResult = GetLabelId(labelPath);
                    if (!labelIdResult.IsSuccess)
                    {
                        return Result<Player>.Failure(labelIdResult.Error);
                    }

                    var playerInDb = _context.Value.Player
                        .FirstOrDefault(p => p.Id == updatedPlayer.Id);

                    if (playerInDb == null)
                    {
                        return Result<Player>.Failure("Player not found.");
                    }

                    playerInDb.Name = updatedPlayer.Name;
                    playerInDb.PictureId = pictureIdResult.Value;
                    playerInDb.IdLabel = labelIdResult.Value;

                    _context.Value.SaveChanges();

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


        public virtual Result<Player> GetPlayerByAccountId(int accountId)
        {
            try
            {
                var player = _context.Value.Player.FirstOrDefault(p => p.AccountId == accountId);

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

        public Result<int> GetLabelId(string labelPath)
        {
            try
            {
                var label = _context.Value.Label
                    .FirstOrDefault(l => l.Path == labelPath);

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

        public Result<int> GetPictureId(string picturePath)
        {
            try
            {
                var picture = _context.Value.Pictures
                    .FirstOrDefault(p => p.path == picturePath);

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

        public virtual Result<string> GetPicturePathById(int pictureId)
        {
            try
            {
                var picture = _context.Value.Pictures
                    .FirstOrDefault(p => p.IdPicture == pictureId);

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

        public virtual Result<string> GetLabelPathById(int labelId)
        {
            try
            {
                var label = _context.Value.Label
                    .FirstOrDefault(l => l.IdLabel == labelId);

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

        public virtual Result<string> GetMailByPlayerId(int playerId)
        {
            try
            {
                var email = _context.Value.Player
                    .Where(p => p.Id == playerId)
                    .Select(p => p.Account.mail) 
                    .FirstOrDefault();

                if (email == null)
                {
                    return Result<string>.Failure("Player or account not found.");
                }

                return Result<string>.Success(email);
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
}
