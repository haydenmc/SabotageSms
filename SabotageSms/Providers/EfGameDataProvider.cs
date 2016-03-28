using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Entity;
using SabotageSms.GameControl.States;
using SabotageSms.Models;
using SabotageSms.Models.DbModels;

namespace SabotageSms.Providers
{
    public class EfGameDataProvider : IGameDataProvider
    {
        private static readonly int _joinCodeLength = 5;
        
        private ApplicationDbContext _db;
        
        public EfGameDataProvider(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public Game GetGameById(long gameId)
        {
            return _db.Games.SingleOrDefault(g => g.GameId == gameId)?.ToGame();
        }
        
        public Game GetGameByJoinCode(string joinCode)
        {
            return _db.Games.SingleOrDefault(g => g.JoinCode == joinCode)?.ToGame();
        }
        
        public Player GetOrCreatePlayerByPhoneNumber(string phoneNumber)
        {
            var player = _db.Players.SingleOrDefault(p => p.PhoneNumber == phoneNumber);
            if (player == null) {
                player = new DbPlayer()
                {
                    PhoneNumber = phoneNumber
                };
            }
            _db.Players.Add(player);
            _db.SaveChanges();
            return player.ToPlayer();
        }
        
        public Player SetPlayerName(long playerId, string name)
        {
            var player = _db.Players.SingleOrDefault(p => p.PlayerId == playerId);
            if (player != null)
            {
                player.Name = name;
                _db.SaveChanges();
            }
            return player?.ToPlayer();
        }

        public Game GetPlayerCurrentGame(long playerId)
        {
            var game = _db.Players
                .Include(p => p.CurrentGame)
                .Include(p => p.CurrentGame.GamePlayers)
                .SingleOrDefault(p => p.PlayerId == playerId)
                .CurrentGame;
            return game?.ToGame();
        }
        
        public Game CreateNewGame(long playerId)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var joinCodeStringBuilder = new StringBuilder();
            for (var i = 0; i < _joinCodeLength; i++) {
                joinCodeStringBuilder.Append((char)random.Next('A','Z'));
            }
            var newGame = new DbGame() {
                JoinCode = joinCodeStringBuilder.ToString(),
                CurrentState = typeof(LobbyState).Name,
                GamePlayers = new List<DbGamePlayer>()
                {
                    new DbGamePlayer() { PlayerId = playerId }
                }
            };
            _db.Players.SingleOrDefault(p => p.PlayerId == playerId).CurrentGame = newGame;
            _db.Games.Add(newGame);
            _db.SaveChanges();
            return newGame.ToGame();
        }
        
        public Game JoinPlayerToGame(long playerId, long gameId)
        {
            var player = _db.Players.SingleOrDefault(p => p.PlayerId == playerId);
            if (player == null) 
            {
                return null;
            }
            var game = _db.Games.SingleOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                return null;
            }
            player.CurrentGame = game;
            game.GamePlayers.Add(new DbGamePlayer() { Player = player });
            _db.SaveChanges();
            return game.ToGame();
        }
        
        public Game SetPlayersGoodBad(long gameId, bool isBad, params long[] playerIds)
        {
            var game = _db.Games.SingleOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                return null;
            }
            var gamePlayers = game.GamePlayers.Where(g => playerIds.Contains(g.PlayerId));
            foreach (var gamePlayer in gamePlayers)
            {
                gamePlayer.IsBad = isBad;
            }
            _db.SaveChanges();
            return game.ToGame();
        }
        
        public Game ScrambleTurnOrder(long gameId)
        {
            var game = _db.Games.SingleOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                return null;
            }
            var playerOrder = game.GamePlayers.OrderBy(o => Guid.NewGuid()).ToList();
            for (var i = 0; i < playerOrder.Count; i++)
            {
                playerOrder[i].TurnOrder = i;
            }
            _db.SaveChanges();
            return game.ToGame();
        }
        
        public Game SetGameState(long gameId, string newState)
        {
            var game = _db.Games.SingleOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                return null;
            }
            game.CurrentState = newState;
            _db.SaveChanges();
            return game.ToGame();
        }
    }
}