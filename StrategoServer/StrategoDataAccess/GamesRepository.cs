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
        private readonly Lazy<StrategoEntities> _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(GamesRepository));

        public GamesRepository(Lazy<StrategoEntities> context)
        {
            _context = context;
        }

        public virtual Result<Games> GetGameStatisticsByAccountId(int accountId)
        {
            try
            {
                var games = _context.Value.Games
                    .Where(g => g.AccountId == accountId)
                    .FirstOrDefault();

                if (games == null)
                {
                    return Result<Games>.Failure("Not available");
                }

                return Result<Games>.Success(games);
            }
            catch (SqlException sqlEx)
            {
                log.Error("Database error", sqlEx);
                return Result<Games>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error", ex);
                return Result<Games>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public virtual Result<string> IncrementWonGames(int accountId)
        {
            try
            {
                var game = _context.Value.Games.FirstOrDefault(g => g.AccountId == accountId);

                if (game == null)
                {
                    return Result<string>.Failure("Game record not found for the specified AccountId.");
                }

                game.WonGames += 1;
                _context.Value.SaveChanges();

                return Result<string>.Success("Won saved succesfully");
            }
            catch (SqlException sqlEx)
            {
                log.Error("Database error", sqlEx);
                return Result<string>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error", ex);
                return Result<string>.Failure($"Unexpected error: {ex.Message}");
            }
        }

        public virtual Result<string> IncrementDeafeatGames(int accountId)
        {
            try
            {
                var game = _context.Value.Games.FirstOrDefault(g => g.AccountId == accountId);

                if (game == null)
                {
                    return Result<string>.Failure("Game record not found for the specified AccountId.");
                }

                game.DeafeatGames += 1;
                _context.Value.SaveChanges();

                return Result<string>.Success("Defeat saved succesfully");
            }
            catch (SqlException sqlEx)
            {
                log.Error("Database error", sqlEx);
                return Result<string>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error", ex);
                return Result<string>.Failure($"Unexpected error: {ex.Message}");
            }
        }

    }
}
