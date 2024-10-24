﻿using StrategoDataAccess;
using StrategoServices.Data.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace StrategoServices.Logic
{
    public class ProfilesManager
    {
        private readonly GamesRepository _gamesRepository;

        public ProfilesManager(GamesRepository profilesRepository)
        {
            _gamesRepository = profilesRepository;
        }

        public async Task<Result<PlayerStatisticsDTO>> GetPlayerGameStatisticsAsync(int accountId)
        {
            var result = await _gamesRepository.GetGameStatisticsByAccountIdAsync(accountId);

            if (!result.IsSuccess)
            {
                return Result<PlayerStatisticsDTO>.Failure(result.Error);
            }

            var gameStatsDto = new PlayerStatisticsDTO
            {
                WonGames = result.Value.WonGames,
                LostGames = result.Value.DeafeatGames,
            };

            return Result<PlayerStatisticsDTO>.Success(gameStatsDto);
        }

    }
}
