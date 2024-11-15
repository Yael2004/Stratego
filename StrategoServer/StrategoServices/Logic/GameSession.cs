using StrategoServices.Services.Interfaces.Callbacks;

namespace StrategoServices.Logic
{
    public class GameSession
    {
        private int _player1Id;
        private int _player2Id;
        private IGameServiceCallback _player1Callback;
        private IGameServiceCallback _player2Callback;
        private int _currentTurnPlayerId;

        public int GameId { get; } 

        public int Player1Id => _player1Id;
        public int Player2Id => _player2Id;

        public GameSession(int gameId, int player1Id, int player2Id = 0)
        {
            GameId = gameId;
            _player1Id = player1Id;
            _player2Id = player2Id;
            _currentTurnPlayerId = player1Id;
        }

        public void SetCallbacks(IGameServiceCallback player1Callback, IGameServiceCallback player2Callback)
        {
            _player1Callback = player1Callback;
            _player2Callback = player2Callback;
        }

        public void SetPlayer1(int player1Id, IGameServiceCallback player1Callback)
        {
            _player1Id = player1Id;
            _player1Callback = player1Callback;
        }

        public void SetPlayer2(int player2Id, IGameServiceCallback player2Callback)
        {
            _player2Id = player2Id;
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
