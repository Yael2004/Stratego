using log4net;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(PlayerRepository));

        public virtual Result<Player> GetOtherPlayerById(int playerId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var player = context.Player.FirstOrDefault(p => p.Id == playerId);

                    if (player == null)
                    {
                        return Result<Player>.Failure("Player not found");
                    }

                    return Result<Player>.Success(player);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<Player>.Failure($"{Messages.DataBaseError}: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<Player>.Failure($"{Messages.UnexpectedError}: {ex.Message}");
            }
        }

        public virtual Result<bool> IsFriend(int playerId, int otherPlayerId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var isFriend = context.Friend.Any(f =>
                        (f.PlayerId == playerId && f.FriendId == otherPlayerId && f.Status == "friend") ||
                        (f.PlayerId == otherPlayerId && f.FriendId == playerId && f.Status == "friend"));

                    return Result<bool>.Success(isFriend);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<bool>.Failure($"{Messages.DataBaseError}: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<bool>.Failure($"{Messages.UnexpectedError}: {ex.Message}");
            }
        }

        public virtual Result<IEnumerable<Player>> GetPlayerFriendsList(int playerId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var result = context.Friend
                    .Where(f =>
                        (f.PlayerId == playerId || f.FriendId == playerId) &&
                        f.Status == "accepted")
                    .Join(
                        context.Player,
                        friend => friend.PlayerId == playerId ? friend.FriendId : friend.PlayerId,
                        player => player.Id,
                        (friend, player) => player
                    )
                    .Distinct()
                    .ToList();

                    if (result.Count == 0)
                    {
                        return Result<IEnumerable<Player>>.Failure("No friends found for the given player ID");
                    }
                    return Result<IEnumerable<Player>>.Success(result);
                }
            } 
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<IEnumerable<Player>>.Failure($"{Messages.DataBaseError} : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<IEnumerable<Player>>.Failure($"{Messages.UnexpectedError}: {ex.Message}");
            }
        }

        public virtual Result<Player> UpdatePlayer(Player updatedPlayer, string labelPath, string picturePath)
        {
            using (var context = new StrategoEntities())
            using (var transaction = context.Database.BeginTransaction())
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

                    var playerInDb = context.Player
                        .FirstOrDefault(p => p.Id == updatedPlayer.Id);

                    if (playerInDb == null)
                    {
                        return Result<Player>.Failure("Player not found.");
                    }

                    playerInDb.Name = updatedPlayer.Name;
                    playerInDb.PictureId = pictureIdResult.Value;
                    playerInDb.IdLabel = labelIdResult.Value;

                    context.SaveChanges();

                    transaction.Commit();

                    return Result<Player>.Success(playerInDb);
                }
                catch (DbEntityValidationException dbEx)
                {
                    log.Error(Messages.EntityValidationError, dbEx);
                    transaction.Rollback();
                    return Result<Player>.Failure($"{Messages.EntityValidationError}: {dbEx.Message}");
                }
                catch (SqlException sqlEx)
                {
                    log.Error(Messages.DataBaseError, sqlEx);
                    transaction.Rollback();
                    return Result<Player>.Failure($"{Messages.DataBaseError} : {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    log.Error(Messages.UnexpectedError, ex);
                    transaction.Rollback();
                    return Result<Player>.Failure($"{Messages.UnexpectedError} : {ex.Message}");
                }
            }
        }


        public virtual Result<Player> GetPlayerByAccountId(int accountId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var player = context.Player.FirstOrDefault(p => p.AccountId == accountId);

                    if (player == null)
                    {
                        return Result<Player>.Failure("Player not found for the given account ID");
                    }

                    return Result<Player>.Success(player);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<Player>.Failure($"{Messages.DataBaseError} : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<Player>.Failure($"{Messages.UnexpectedError} : {ex.Message}");
            }
        }

        public Result<int> GetLabelId(string labelPath)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var label = context.Label
                    .FirstOrDefault(l => l.Path == labelPath);

                    if (label == null)
                    {
                        return Result<int>.Failure("Label path not found.");
                    }

                    return Result<int>.Success(label.IdLabel);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<int>.Failure($"{Messages.DataBaseError}  : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<int>.Failure($"{Messages.UnexpectedError} : {ex.Message}");
            }
        }

        public Result<int> GetPictureId(string picturePath)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var picture = context.Pictures
                        .FirstOrDefault(p => p.path == picturePath);

                    if (picture == null)
                    {
                        return Result<int>.Failure("Picture path not found.");
                    }

                    return Result<int>.Success(picture.IdPicture);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<int>.Failure($"{Messages.DataBaseError}  : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<int>.Failure($"{Messages.UnexpectedError} : {ex.Message}");
            }
        }

        public virtual Result<string> GetPicturePathById(int pictureId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var picture = context.Pictures
                    .FirstOrDefault(p => p.IdPicture == pictureId);

                    if (picture == null)
                    {
                        return Result<string>.Failure("Picture not found.");
                    }

                    return Result<string>.Success(picture.path);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<string>.Failure($"{Messages.DataBaseError}  : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<string>.Failure($"{Messages.UnexpectedError} : {ex.Message}");
            }
        }

        public virtual Result<string> GetLabelPathById(int labelId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var label = context.Label
                    .FirstOrDefault(l => l.IdLabel == labelId);

                    if (label == null)
                    {
                        return Result<string>.Failure("Label not found.");
                    }

                    return Result<string>.Success(label.Path);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<string>.Failure($"{Messages.DataBaseError}  : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<string>.Failure($"{Messages.UnexpectedError} : {ex.Message}");
            }
        }

        public virtual Result<string> GetMailByPlayerId(int playerId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var email = context.Player
                    .Where(p => p.Id == playerId)
                    .Select(p => p.Account.mail)
                    .FirstOrDefault();

                    if (email == null)
                    {
                        return Result<string>.Failure("Player or account not found.");
                    }

                    return Result<string>.Success(email);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<string>.Failure($"{Messages.DataBaseError}  : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<string>.Failure($"{Messages.UnexpectedError}  : {ex.Message}");
            }
        }

        public virtual Result<IEnumerable<Player>> GetTopPlayersByWins()
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var result = context.Games
                    .GroupBy(g => g.AccountId)
                    .Select(g => new
                    {
                        AccountId = g.Key,
                        WonGames = g.Sum(x => x.WonGames)
                    })
                    .OrderByDescending(g => g.WonGames)
                    .Take(10)
                    .Join(
                        context.Player,
                        games => games.AccountId,
                        player => player.AccountId,
                        (games, player) => player
                    )
                    .ToList();

                    if (result.Count == 0)
                    {
                        return Result<IEnumerable<Player>>.Failure("No players found with games won.");
                    }
                    return Result<IEnumerable<Player>>.Success(result);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<IEnumerable<Player>>.Failure($"{Messages.DataBaseError}: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<IEnumerable<Player>>.Failure($"{Messages.UnexpectedError}: {ex.Message}");
            }
        }

        public virtual Result<string> ReportPlayer(int reporterId, int reportedId, string reason)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    if (string.IsNullOrWhiteSpace(reason))
                    {
                        return Result<string>.Failure("Reason for report cannot be empty.");
                    }

                    var report = new Report
                    {
                        IdReporter = reporterId,
                        IdReported = reportedId,
                        Reason = reason,
                        Date = DateTime.Now
                    };

                    context.Report.Add(report);
                    context.SaveChanges();

                    return Result<string>.Success("Player reported successfully.");
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<string>.Failure($"{Messages.DataBaseError}   : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<string>.Failure($"{Messages.UnexpectedError}  : {ex.Message}");
            }
        }

        public virtual Result<int> GetReportCount(int playerId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var reportCount = context.Report
                        .Count(r => r.IdReported == playerId);

                    return Result<int>.Success(reportCount);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<int>.Failure($"{Messages.DataBaseError}: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<int>.Failure($"{Messages.UnexpectedError}: {ex.Message}");
            }
        }

    }
}
