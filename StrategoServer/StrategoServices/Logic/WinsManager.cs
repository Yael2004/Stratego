using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoServices.Logic
{
    public class WinsManager
    {
        private readonly Lazy<GamesRepository> _gamesRepository;

        public WinsManager(Lazy<GamesRepository> gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }

        public Result<string> IncrementWins(int accountId)
        {
            return _gamesRepository.Value.IncrementWonGames(accountId);
        }

        public Result<string> IncrementDefeats(int accountId)
        {
            return _gamesRepository.Value.IncrementDeafeatGames(accountId);
        }
    }
}
