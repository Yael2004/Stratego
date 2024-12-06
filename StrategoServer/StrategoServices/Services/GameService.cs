using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading.Tasks;
using log4net;
using StrategoDataAccess;
using StrategoServices.Data;
using StrategoServices.Data.DTO;
using StrategoServices.Logic;
using StrategoServices.Services.Interfaces;
using StrategoServices.Services.Interfaces.Callbacks;
using Utilities;

namespace StrategoServices.Services
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GameService : IGameService, ICreateGameService
    {
        private readonly ConcurrentDictionary<int, GameSession> _activeGames = new ConcurrentDictionary<int, GameSession>();
        private readonly Lazy<WinsManager> _winsManager;
        private readonly ILog log = LogManager.GetLogger(typeof(GameService));

        public GameService(Lazy<WinsManager> winsManager)
        {
            _winsManager = winsManager;
        }

        /// <summary>
        /// Creates a new game session and adds it to the active games list
        /// </summary>
        /// <returns>Game id and operation result encapsulated</returns>
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

        /// <summary>
        /// Associate a player with a game session and sets the callback for the player
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="playerId"></param>
        /// <returns>Task</returns>
        public async Task JoinGameSessionAsync(int gameId, int playerId)
        {
            var playerCallback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();

            var result = GetGameSession(gameId, out var gameSession);
            if (!result.IsSuccess)
            {
                await NotifyCallbackAsync(() => playerCallback.OnGameStarted(0, new GameStartedResponse(false, result)));
                return;
            }

            var joinResult = await AddPlayerToSessionAsync(gameSession, playerId, playerCallback);
        }

        /// <summary>
        /// Adds a player to a game session for starts the game if both players are ready
        /// </summary>
        /// <param name="gameSession"></param>
        /// <param name="playerId"></param>
        /// <param name="playerCallback"></param>
        /// <returns>Task with operation result</returns>
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
                        NotifyCallbackAsync(() => player1Callback.OnGameStarted(gameSession.GameId, new GameStartedResponse(true, readyResult))),
                        NotifyCallbackAsync(() => playerCallback.OnGameStarted(gameSession.GameId, new GameStartedResponse(false, readyResult)))
                    );
                    return readyResult;
                }

                return new OperationResult(true, "Joined the game as Player 2.");
            }

            return new OperationResult(false, "Game session is already full.");
        }

        /// <summary>
        /// Sends the position of a player piece to the opponent for handling
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="playerId"></param>
        /// <param name="position"></param>
        /// <returns>Task</returns>
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

        /// <summary>
        /// Sends the final position and stats for ending the game and sets the win or defeat for the players
        /// </summary>
        /// <param name="finalStats"></param>
        /// <returns>Task</returns>
        public async Task EndGameAsync(FinalStatsDTO finalStats)
        {
            var result = GetGameSession(finalStats.GameId, out var gameSession);
            if (!result.IsSuccess)
            {
                var callback = OperationContext.Current.GetCallbackChannel<IGameServiceCallback>();
                await NotifyCallbackAsync(() => callback.OnGameEnded("Error", result));
                return;
            }

            var playerCallback = gameSession.GetCallbackForPlayer(finalStats.PlayerId);
            OperationResult statsUpdateResult;

            try
            {
                if (finalStats.HasWon)
                {
                    var resultIncrementWins = _winsManager.Value.IncrementWins(finalStats.AccountId);
                    statsUpdateResult = new OperationResult(resultIncrementWins.IsSuccess, resultIncrementWins.Error);
                }
                else
                {
                    var resultIncrementWins = _winsManager.Value.IncrementDefeats(finalStats.AccountId);
                    statsUpdateResult = new OperationResult(resultIncrementWins.IsSuccess, resultIncrementWins.Error);
                }

                if (!statsUpdateResult.IsSuccess)
                {
                    await NotifyCallbackAsync(() => playerCallback.OnGameEnded("Error updating stats", statsUpdateResult));
                    return;
                }
                else
                {
                    var message = finalStats.HasWon ? "You won!" : "You lost!";
                    await NotifyCallbackAsync(() => playerCallback.OnGameEnded(message, statsUpdateResult));
                }
            }
            catch (Exception ex)
            {
                log.Error(Messages.UnexpectedError, ex);
                statsUpdateResult = new OperationResult(false, $"Messages.UnexpectedError: {ex.Message}");
                await NotifyCallbackAsync(() => playerCallback.OnGameEnded("Error", statsUpdateResult));
            }

        }

        /// <summary>
        /// Handle the abandonment of a game by a player and notifies the opponent
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="playerId"></param>
        /// <returns>Task</returns>
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

        /// <summary>
        /// Get the game session for a given game id
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="gameSession"></param>
        /// <returns>Operation result</returns>
        private OperationResult GetGameSession(int gameId, out GameSession gameSession)
        {
            if (_activeGames.TryGetValue(gameId, out gameSession))
            {
                return new OperationResult(true, "Game session found.");
            }

            return new OperationResult(false, "Game session not found.");
        }

        /// <summary>
        /// Helper method to notify a callback action asynchronously
        /// </summary>
        /// <param name="callbackAction"></param>
        /// <returns>Task</returns>
        private async Task NotifyCallbackAsync(Action callbackAction)
        {
            try
            {
                await Task.Run(callbackAction);
            }
            catch (TimeoutException tex)
            {
                log.Error($"Messages.TimeoutError: ", tex);
            }
            catch (CommunicationException cex)
            {
                log.Fatal($"Messages.CommunicationError:", cex);
            }
            catch (Exception ex)
            {
                log.Fatal($"Messages.UnexpectedError: ", ex);
            }
        }

        /// <summary>
        /// Sends the movement instructions to the opponent for handling
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="instruction"></param>
        /// <returns>Task</returns>
        public async Task SendMovementInstructionsAsync(int gameId, MovementInstructionDTO instruction)
        {
            var result = GetGameSession(gameId, out var gameSession);

            if (!result.IsSuccess)
            {
                Console.WriteLine($"Game session not found for GameId: {gameId}");
                return;
            }

            var opponentId = gameSession.GetOpponentId(instruction.DefenderId);

            var attackerCallback = gameSession.GetCallbackForPlayer(opponentId);
            if (attackerCallback == null)
            {
                Console.WriteLine($"Attacker callback not found for OpponentId: {opponentId}");
                return;
            }

            var response = new MovementInstructionResponse
            {
                MovementInstructionDTO = instruction,
                OperationResult = new OperationResult(true, "Movement processed successfully")
            };

            try
            {
                await NotifyCallbackAsync(() => attackerCallback.OnReceiveMovementInstructions(response));
            }
            catch (TimeoutException tex)
            {
                log.Error($"Timeout error while sending movement instructions to OpponentId: {opponentId}. Exception: {tex.Message}");
            }
            catch (CommunicationException cex)
            {
                log.Fatal($"Communication error while sending movement instructions to OpponentId: {opponentId}. Exception: {cex.Message}");

            }
            catch (Exception ex)
            {
                log.Fatal($"Error while sending movement instructions to OpponentId: {opponentId}. Exception: {ex.Message}");
            }
        }

    }
}
