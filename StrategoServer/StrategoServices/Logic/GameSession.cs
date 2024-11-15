using StrategoServices.Data.DTO;
using StrategoServices.Services.Interfaces.Callbacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServices.Logic
{
    public class GameSession
    {
        private readonly int _player1Id;
        private readonly int _player2Id;
        private IGameServiceCallback _player1Callback;
        private IGameServiceCallback _player2Callback;
        private int _currentTurnPlayerId;

        public int GameId { get; }

        public GameSession(int player1Id, int player2Id)
        {
            _player1Id = player1Id;
            _player2Id = player2Id;
            _currentTurnPlayerId = player1Id;
            GameId = new Random().Next(1, 100000);
        }

        public void SetCallbacks(IGameServiceCallback player1Callback, IGameServiceCallback player2Callback)
        {
            _player1Callback = player1Callback;
            _player2Callback = player2Callback;
        }

        public IGameServiceCallback GetCallbackForPlayer(int playerId)
        {
            return playerId == _player1Id ? _player1Callback : _player2Callback;
        }

        public bool IsTurn(int playerId)
        {
            return _currentTurnPlayerId == playerId;
        }

        public void ToggleTurn()
        {
            _currentTurnPlayerId = _currentTurnPlayerId == _player1Id ? _player2Id : _player1Id;
        }

        public int GetOpponentId(int playerId)
        {
            return playerId == _player1Id ? _player2Id : _player1Id;
        }

    }
}
