using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace StrategoDataAccess
{
    public class GamesRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GamesRepository));

        public virtual Result<Games> GetGameStatisticsByAccountId(int accountId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var games = context.Games
                    .Where(g => g.AccountId == accountId)
                    .FirstOrDefault();

                    if (games == null)
                    {
                        return Result<Games>.Failure("Not available");
                    }

                    return Result<Games>.Success(games);
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<Games>.DataBaseError($"{Messages.DataBaseError} : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<Games>.DataBaseError($"{Messages.UnexpectedError}  : {ex.Message}");
            }
        }

        public virtual Result<string> IncrementWonGames(int accountId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var game = context.Games.FirstOrDefault(g => g.AccountId == accountId);

                    if (game == null)
                    {
                        return Result<string>.Failure("Game record not found for the specified AccountId.");
                    }

                    game.WonGames += 1;
                    context.SaveChanges();

                    return Result<string>.Success("Won saved succesfully");
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<string>.DataBaseError($"{Messages.DataBaseError} : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<string>.DataBaseError($"{Messages.UnexpectedError} : {ex.Message}");
            }
        }

        public virtual Result<string> IncrementDeafeatGames(int accountId)
        {
            try
            {
                using (var context = new StrategoEntities())
                {
                    var game = context.Games.FirstOrDefault(g => g.AccountId == accountId);

                    if (game == null)
                    {
                        return Result<string>.Failure("Game record not found for the specified AccountId.");
                    }

                    game.DeafeatGames += 1;
                    context.SaveChanges();

                    return Result<string>.Success("Defeat saved succesfully");
                }
            }
            catch (SqlException sqlEx)
            {
                log.Error(Messages.DataBaseError, sqlEx);
                return Result<string>.DataBaseError($"{Messages.DataBaseError} : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                return Result<string>.DataBaseError($"{Messages.UnexpectedError} : {ex.Message}");
            }
        }

    }
}
