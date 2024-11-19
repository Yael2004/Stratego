using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;
using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using StrategoServices.Services.Interfaces;
using StrategoServices.Services.Interfaces.Callbacks;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GameService : IGameService, ICreateGameService
    {
        private readonly ConcurrentDictionary<int, GameSession> _activeGames = new ConcurrentDictionary<int, GameSession>();

        public GameSessionCreatedResponse CreateGameSession()
        {
            OperationResult result;
            int gameId = 0;

            try
            {
                var gameSession = new GameSession();

                if (!_activeGames.TryAdd(gameSession.GameId, gameSession))
                {
                    result = new OperationResult(false, "Failed to create game session.");
                }
                else
                {
                    gameId = gameSession.GameId;
                    result = new OperationResult(true, "Game session created successfully.");
                }
            }
            catch (Exception ex)
            {
                result = new OperationResult(false, $"An error occurred while creating the game session: {ex.Message}");
            }

            return new GameSessionCreatedResponse
            {
                GameId = gameId,
                OperationResult = result
            };
        }

        public async Task JoinGameSessionAsync(int gameId, int playerId)
        {
            var playerCallback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();

            var result = GetGameSession(gameId, out var gameSession);
            if (!result.IsSuccess)
            {
                await NotifyCallbackAsync(() => playerCallback.OnGameStarted(0, result));
                return;
            }

            var joinResult = await AddPlayerToSessionAsync(gameSession, playerId, playerCallback);
            await NotifyCallbackAsync(() => playerCallback.OnGameStarted(gameId, joinResult));
        }

        private async Task<OperationResult> AddPlayerToSessionAsync(GameSession gameSession, int playerId, IGameServiceCallback playerCallback)
        {
            if (gameSession.Player1Id == 0)
            {
                gameSession.SetPlayer1(playerId, playerCallback);
                return new OperationResult(true, "Joined the game as Player 1.");
            }

            if (gameSession.Player2Id == 0)
            {
                gameSession.SetPlayer2(playerId, playerCallback);

                if (gameSession.IsGameReady())
                {
                    var player1Callback = gameSession.GetCallbackForPlayer(gameSession.Player1Id);
                    var readyResult = new OperationResult(true, "Game is ready.");
                    await Task.WhenAll(
                        NotifyCallbackAsync(() => player1Callback.OnGameStarted(gameSession.GameId, readyResult)),
                        NotifyCallbackAsync(() => playerCallback.OnGameStarted(gameSession.GameId, readyResult))
                    );
                    return readyResult;
                }

                return new OperationResult(true, "Joined the game as Player 2.");
            }

            return new OperationResult(false, "Game session is already full.");
        }

        public async Task SendPositionAsync(int gameId, int playerId, PositionDTO position)
        {
            var playerCallback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();

            var result = GetGameSession(gameId, out var gameSession);
            if (!result.IsSuccess)
            {
                await NotifyCallbackAsync(() => playerCallback.OnReceiveOpponentPosition(position, result));
                return;
            }

            var opponentCallback = gameSession.GetCallbackForPlayer(gameSession.GetOpponentId(playerId));
            result = new OperationResult(true, "Position sent successfully.");
            await NotifyCallbackAsync(() => opponentCallback.OnReceiveOpponentPosition(position, result));
        }

        public async Task EndGameAsync(int gameId, int winnerId)
        {
            var result = GetGameSession(gameId, out var gameSession);
            if (!result.IsSuccess)
            {
                var callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
                await NotifyCallbackAsync(() => callback.OnGameEnded("Error", result));
                return;
            }

            var winnerCallback = gameSession.GetCallbackForPlayer(winnerId);
            var loserCallback = gameSession.GetCallbackForPlayer(gameSession.GetOpponentId(winnerId));
            result = new OperationResult(true, "Game ended successfully.");

            await Task.WhenAll(
                NotifyCallbackAsync(() => winnerCallback.OnGameEnded("You won!", result)),
                NotifyCallbackAsync(() => loserCallback.OnGameEnded("You lost!", result))
            );

            _activeGames.TryRemove(gameId, out _);
        }

        public async Task AbandonGameAsync(int gameId, int playerId)
        {
            var result = GetGameSession(gameId, out var gameSession);
            if (!result.IsSuccess)
            {
                var callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
                await NotifyCallbackAsync(() => callback.OnOpponentAbandonedGame(result));
                return;
            }

            var opponentCallback = gameSession.GetCallbackForPlayer(gameSession.GetOpponentId(playerId));
            result = new OperationResult(true, "Player abandoned the game.");
            await NotifyCallbackAsync(() => opponentCallback.OnOpponentAbandonedGame(result));

            _activeGames.TryRemove(gameId, out _);
        }

        private OperationResult GetGameSession(int gameId, out GameSession gameSession)
        {
            if (_activeGames.TryGetValue(gameId, out gameSession))
            {
                return new OperationResult(true, "Game session found.");
            }

            return new OperationResult(false, "Game session not found.");
        }

        private async Task NotifyCallbackAsync(Action callbackAction)
        {
            try
            {
                await Task.Run(callbackAction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Callback failed: {ex.Message}");
            }
        }

    }
}
