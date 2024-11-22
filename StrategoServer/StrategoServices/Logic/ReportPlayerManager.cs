using StrategoDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoServices.Logic
{
    public class ReportPlayerManager
    {
        private readonly Lazy<PlayerRepository> _playerRepository;

        public ReportPlayerManager(Lazy<PlayerRepository> playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public Result<string> ReportPlayer(int reporterId, int reportedId, string reason)
        {
            return _playerRepository.Value.ReportPlayer(reporterId, reportedId, reason);
        }
    }
}
