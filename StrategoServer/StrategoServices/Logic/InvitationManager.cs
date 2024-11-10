using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoServices.Logic
{
    public class InvitationManager
    {
        private readonly Lazy<PlayerRepository> _playerRepository;

        public InvitationManager(Lazy<PlayerRepository> playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public Result<string> GetPlayerMail(int playerId)
        {
            return _playerRepository.Value.GetMailByPlayerId(playerId);
        }
    }
}
