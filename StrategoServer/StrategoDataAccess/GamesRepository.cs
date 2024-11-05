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

        public GamesRepository(Lazy<StrategoEntities> context)
        {
            _context = context;
        }

        public Result<Games> GetGameStatisticsByAccountId(int accountId)
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
                return Result<Games>.Failure($"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return Result<Games>.Failure($"Unexpected error: {ex.Message}");
            }
        }

    }
}
