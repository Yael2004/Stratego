using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;
using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using StrategoServices.Services.Interfaces.Callbacks;
using StrategoServices.Services.Interfaces;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GameService : IGameService
    {
        private readonly ConcurrentDictionary<int, GameSession> _activeGames = new ConcurrentDictionary<int, GameSession>();

        public async Task<OperationResult> StartGameAsync(int player1Id, int player2Id)
        {
            var gameSession = new GameSession(player1Id, player2Id);
            if (!_activeGames.TryAdd(gameSession.GameId, gameSession))
            {
                return new OperationResult(false, "Failed to start the game.");
            }

            var player1Callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
            var player2Callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();

            gameSession.SetCallbacks(player1Callback, player2Callback);

            await Task.WhenAll(
                Task.Run(() => player1Callback.OnGameStarted(gameSession.GameId)),
                Task.Run(() => player2Callback.OnGameStarted(gameSession.GameId))
            );

            return new OperationResult(true, "Game started successfully.");
        }

        public async Task<OperationResult> SendPositionAsync(int gameId, int playerId, PositionDTO position)
        {
            if (!_activeGames.TryGetValue(gameId, out var gameSession))
                return new OperationResult(false, "Game session not found.");

            if (!gameSession.IsTurn(playerId))
                return new OperationResult(false, "Not your turn.");

            var opponentCallback = gameSession.GetCallbackForPlayer(gameSession.GetOpponentId(playerId));
            await Task.Run(() => opponentCallback.OnReceiveOpponentPosition(position));

            gameSession.ToggleTurn();
            return new OperationResult(true, "Position sent successfully.");
        }

        public async Task<OperationResult> EndGameAsync(int gameId, int ganadorId)
        {
            if (!_activeGames.TryGetValue(gameId, out var gameSession))
                return new OperationResult(false, "Game session not found.");

            var ganadorCallback = gameSession.GetCallbackForPlayer(ganadorId);
            var perdedorId = gameSession.GetOpponentId(ganadorId);
            var perdedorCallback = gameSession.GetCallbackForPlayer(perdedorId);

            await Task.WhenAll(
                Task.Run(() => ganadorCallback.OnGameEnded("You won!")),
                Task.Run(() => perdedorCallback.OnGameEnded("You lost!"))
            );

            EndGame(gameId);
            return new OperationResult(true, "Game ended successfully.");
        }

        public Task<OperationResult> AbandonGameAsync(int gameId, int playerId)
        {
            if (_activeGames.TryGetValue(gameId, out var gameSession))
            {
                var opponentCallback = gameSession.GetCallbackForPlayer(gameSession.GetOpponentId(playerId));
                opponentCallback.OnOpponentAbandonedGame();

                EndGame(gameId);

                return Task.FromResult(new OperationResult(true, "Player abandoned the game."));
            }
            return Task.FromResult(new OperationResult(false, "Game session not found."));
        }

        private void EndGame(int gameId)
        {
            _activeGames.TryRemove(gameId, out _);
        }

    }
}