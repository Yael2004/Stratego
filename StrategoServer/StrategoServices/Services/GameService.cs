using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using StrategoServices.Services.Interfaces;
using StrategoServices.Services.Interfaces.Callbacks;
using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GameService : IGameService
    {
        private readonly ConcurrentDictionary<int, GameSession> _activeGames = new ConcurrentDictionary<int, GameSession>();

        public async Task<int> CreateGameAsync(int player1Id, int player2Id = 0)
        {
            OperationResult result;
            int gameId = 0;

            IGameServiceCallback player1Callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();

            try
            {
                do
                {
                    gameId = new Random().Next(1, 100000);
                } while (_activeGames.ContainsKey(gameId));

                var gameSession = new GameSession(player1Id, player2Id);

                if (!_activeGames.TryAdd(gameId, gameSession))
                {
                    result = new OperationResult(false, "Failed to create game.");
                }
                else
                {
                    gameSession.SetPlayer1(player1Id, player1Callback);

                    var communicationObject = (ICommunicationObject)player1Callback;
                    communicationObject.Faulted += (s, e) => HandlePlayerDisconnection(gameId, player1Id);
                    communicationObject.Closed += (s, e) => HandlePlayerDisconnection(gameId, player1Id);

                    result = new OperationResult(true, "Game created.");
                }
            }
            catch (Exception ex)
            {
                result = new OperationResult(false, $"An unexpected error occurred: {ex.Message}");
                gameId = 0;
            }

            await Task.Run(() => player1Callback.OnGameStarted(result, gameId));
            return gameId;
        }


        public async Task JoinGameAsync(int gameId, int playerId)
        {
            OperationResult result;

            try
            {
                if (!_activeGames.TryGetValue(gameId, out var gameSession))
                {
                    result = new OperationResult(false, "Game session not found.");
                    await NotifyClient(gameId, playerId, result);
                    return;
                }

                if (gameSession.GetOpponentId(playerId) == playerId || gameSession.GetCallbackForPlayer(playerId) != null)
                {
                    result = new OperationResult(false, "Player is already in the game.");
                    await NotifyClient(gameId, playerId, result);
                    return;
                }

                var player2Callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
                gameSession.SetPlayer2(playerId, player2Callback);

                var communicationObject = (ICommunicationObject)player2Callback;
                communicationObject.Faulted += (s, e) => HandlePlayerDisconnection(gameId, playerId);
                communicationObject.Closed += (s, e) => HandlePlayerDisconnection(gameId, playerId);

                result = new OperationResult(true, "You joined the game.");
                await Task.WhenAll(
                    Task.Run(() => gameSession.GetCallbackForPlayer(gameSession.GetOpponentId(playerId))?.OnGameStarted(new OperationResult(true, "Opponent joined the game."), gameId)),
                    Task.Run(() => player2Callback.OnGameStarted(result, gameId))
                );
            }
            catch (Exception ex)
            {
                result = new OperationResult(false, $"An unexpected error occurred: {ex.Message}");
                await NotifyClient(gameId, playerId, result);
            }
        }

        private async Task NotifyClient(int gameId, int playerId, OperationResult result)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
            await Task.Run(() => callback.OnGameStarted(result, gameId));
        }

        public async Task SendPositionAsync(int gameId, int playerId, PositionDTO position)
        {
            OperationResult result;

            try
            {
                if (!_activeGames.TryGetValue(gameId, out var gameSession))
                {
                    result = new OperationResult(false, "Game session not found.");
                    await NotifyClientPosition(playerId, result, position);
                    return;
                }

                if (!gameSession.IsTurn(playerId))
                {
                    result = new OperationResult(false, "Not your turn.");
                    await NotifyClientPosition(playerId, result, position);
                    return;
                }

                var opponentCallback = gameSession.GetCallbackForPlayer(gameSession.GetOpponentId(playerId));
                await Task.Run(() => opponentCallback?.OnReceiveOpponentPosition(new OperationResult(true, "Position received."), position));

                gameSession.ToggleTurn();
                result = new OperationResult(true, "Position sent successfully.");
            }
            catch (Exception ex)
            {
                result = new OperationResult(false, $"An unexpected error occurred: {ex.Message}");
            }

            await NotifyClientPosition(playerId, result, position);
        }

        private async Task NotifyClientPosition(int playerId, OperationResult result, PositionDTO position)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
            await Task.Run(() => callback.OnReceiveOpponentPosition(result, position));
        }

        public async Task EndGameAsync(int gameId, int playerId)
        {
            OperationResult result;

            try
            {
                if (!_activeGames.TryGetValue(gameId, out var gameSession))
                {
                    result = new OperationResult(false, "Game session not found.");
                    await NotifyClientEndGame(playerId, result, "No advice.");
                    return;
                }

                var opponentId = gameSession.GetOpponentId(playerId);
                var playerCallback = gameSession.GetCallbackForPlayer(playerId);
                var opponentCallback = gameSession.GetCallbackForPlayer(opponentId);

                result = new OperationResult(true, "Game ended.");
                var advice = playerId == gameSession.Player1Id ? "You won!" : "Better luck next time!";

                await Task.WhenAll(
                    Task.Run(() => playerCallback?.OnGameEnded(result, advice)),
                    Task.Run(() => opponentCallback?.OnGameEnded(result, "You lost!"))
                );

                _activeGames.TryRemove(gameId, out _);
            }
            catch (Exception ex)
            {
                result = new OperationResult(false, $"An unexpected error occurred: {ex.Message}");
                await NotifyClientEndGame(playerId, result, "Error occurred.");
            }
        }

        private async Task NotifyClientEndGame(int playerId, OperationResult result, string advice)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
            await Task.Run(() => callback.OnGameEnded(result, advice));
        }

        public async Task AbandonGameAsync(int gameId, int playerId)
        {
            OperationResult result;

            try
            {
                if (!_activeGames.TryGetValue(gameId, out var gameSession))
                {
                    result = new OperationResult(false, "Game session not found.");
                    await NotifyClientAbandon(playerId, result);
                    return;
                }

                var opponentCallback = gameSession.GetCallbackForPlayer(gameSession.GetOpponentId(playerId));
                await Task.Run(() => opponentCallback?.OnOpponentAbandonedGame());

                _activeGames.TryRemove(gameId, out _);
                result = new OperationResult(true, "Game abandoned successfully.");
            }
            catch (Exception ex)
            {
                result = new OperationResult(false, $"An unexpected error occurred: {ex.Message}");
            }

            await NotifyClientAbandon(playerId, result);
        }

        private async Task NotifyClientAbandon(int playerId, OperationResult result)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
            await Task.Run(() => callback.OnGameEnded(result, "Game abandoned."));
        }

        private void HandlePlayerDisconnection(int gameId, int playerId)
        {
            if (_activeGames.TryGetValue(gameId, out var gameSession))
            {
                if (gameSession.GetCallbackForPlayer(playerId) != null)
                {
                    if (playerId == gameSession.Player1Id)
                    {
                        gameSession.SetPlayer1(0, null);
                    }
                    else
                    {
                        gameSession.SetPlayer2(0, null);
                    }
                }

                if (gameSession.Player1Id == 0 && gameSession.Player2Id == 0)
                {
                    _activeGames.TryRemove(gameId, out _);
                }
            }
        }

    }
}
