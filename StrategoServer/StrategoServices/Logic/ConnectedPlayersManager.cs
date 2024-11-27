using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Logic
{
    public class ConnectedPlayersManager
    {
        private readonly ConcurrentDictionary<int, string> _connectedPlayers;

        public ConnectedPlayersManager()
        {
            _connectedPlayers = new ConcurrentDictionary<int, string>();
        }

        public bool AddPlayer(int playerId, string playerInfo)
        {
            return _connectedPlayers.TryAdd(playerId, playerInfo);
        }

        public bool IsPlayerConnected(int playerId)
        {
            return _connectedPlayers.ContainsKey(playerId);
        }

        public bool RemovePlayer(int playerId)
        {
            return _connectedPlayers.TryRemove(playerId, out _);
        }

        public string GetPlayerInfo(int playerId)
        {
            _connectedPlayers.TryGetValue(playerId, out var playerInfo);
            return playerInfo;
        }

        public IReadOnlyDictionary<int, string> GetAllConnectedPlayers()
        {
            return _connectedPlayers;
        }

        public IReadOnlyDictionary<int, string> GetConnectedFriends(IEnumerable<int> friendIds)
        {
            if (friendIds == null || !friendIds.Any())
            {
                return new Dictionary<int, string>();
            }

            return _connectedPlayers
                .Where(kvp => friendIds.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

    }
}
