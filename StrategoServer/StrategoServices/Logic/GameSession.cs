﻿using StrategoServices.Data.DTO;
using StrategoServices.Services.Interfaces.Callbacks;
using System;

namespace StrategoServices.Logic
{
    public class GameSession
    {
        private int _player1Id;
        private int _player2Id;
        private IGameServiceCallback _player1Callback;
        private IGameServiceCallback _player2Callback;

        public int GameId { get; }

        public int Player1Id => _player1Id;
        public int Player2Id => _player2Id;

        public GameSession()
        {
            _player1Id = 0;
            _player2Id = 0;
            GameId = new Random().Next(1, 100000);
        }

        public void SetPlayer1(int player1Id, IGameServiceCallback player1Callback)
        {
            if (_player1Id != 0)
                throw new InvalidOperationException("Player 1 is already set.");

            _player1Id = player1Id;
            _player1Callback = player1Callback;
        }

        public void SetPlayer2(int player2Id, IGameServiceCallback player2Callback)
        {
            if (_player2Id != 0)
                throw new InvalidOperationException("Player 2 is already set.");

            _player2Id = player2Id;
            _player2Callback = player2Callback;
        }

        public IGameServiceCallback GetCallbackForPlayer(int playerId)
        {
            if (playerId == _player1Id) return _player1Callback;
            if (playerId == _player2Id) return _player2Callback;
            return null;
        }

        public int GetOpponentId(int playerId)
        {
            if (playerId == _player1Id)
            {
                return _player2Id;
            }
            if (playerId == _player2Id)
            {
                return _player1Id;
            }
            return 0;
        }

        public bool IsGameReady()
        {
            return _player1Id != 0 && _player2Id != 0;
        }
    }
}