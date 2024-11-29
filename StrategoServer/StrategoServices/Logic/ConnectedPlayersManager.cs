using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

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
    }
}
