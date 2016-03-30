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
            return _db.Games
                .Include(g => g.GamePlayers)
                .ThenInclude(gp => gp.Player)
                .SingleOrDefault(g => g.GameId == gameId)?.ToGame();
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
                _db.Players.Add(player);
            }
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
                .ThenInclude(cg => cg.GamePlayers)
                .ThenInclude(cgp => cgp.Player)
                .SingleOrDefault(p => p.PlayerId == playerId)
                .CurrentGame;
            return game?.ToGame();
        }
        
        public Game CreateNewGame(long playerId)
        {
            var player = _db.Players.SingleOrDefault(p => p.PlayerId == playerId);
            if (player == null) 
            {
                return null;
            }
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
                    new DbGamePlayer()
                    {
                        Player = player,
                        IsBad = false
                    }
                },
                Rounds = new List<DbRound>()
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
            game.GamePlayers.Add(
                new DbGamePlayer()
                {
                    Player = player,
                    IsBad = false
                }
            );
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
        
        public Player GetGamePlayerByName(long gameId, string playerName)
        {
            var gamePlayer = _db.GamePlayers
                .SingleOrDefault(g =>
                    g.Player.Name.ToUpper() == playerName.ToUpper()
                    && g.GameId == gameId
                );
            return gamePlayer?.Player.ToPlayer();
        }
        
        public Game AddRound(long gameId)
        {
            var game = _db.Games.SingleOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                return null;
            }
            game.Rounds.Add(new DbRound()
            {
                RoundNumber = game.Rounds.Count + 1,
                SelectedPlayers = new List<DbPlayer>(),
                ApprovingPlayers = new List<DbPlayer>(),
                RejectingPlayers = new List<DbPlayer>(),
                PassingPlayers = new List<DbPlayer>(),
                FailingPlayers = new List<DbPlayer>(),
            });
            _db.SaveChanges();
            return game.ToGame();
        }
        
        public Game SetRoundSelectedPlayers(long roundId, long[] playerIds)
        {
            var round = _db.Rounds
                .Include(r => r.SelectedPlayers)
                .Include(r => r.Game)
                .SingleOrDefault(g => g.RoundId == roundId);
            if (round == null)
            {
                return null;
            }
            round.SelectedPlayers.Clear();
            for (var i = 0; i < playerIds.Length; i++)
            {
                round.SelectedPlayers.Add(_db.Players.SingleOrDefault(p => p.PlayerId == playerIds[i]));
            }
            _db.SaveChanges();
            return round.Game.ToGame();
        }
        
        public Game SetRoundPlayerAsApproving(long roundId, long playerId, bool isApproving)
        {
            var round = _db.Rounds
                .Include(r => r.Game)
                .Include(r => r.Game.GamePlayers)
                .ThenInclude(gp => gp.Player)
                .SingleOrDefault(r => r.RoundId == roundId);
            if (round == null)
            {
                return null;
            }
            var player = round.Game.GamePlayers.SingleOrDefault(p => p.PlayerId == playerId)?.Player;
            if (player == null)
            {
                return null;
            }
            if (isApproving)
            {
                // Remove from rejecting
                if (round.RejectingPlayers.SingleOrDefault(p => p.PlayerId == playerId) != null)
                {
                    round.RejectingPlayers.Remove(player);
                }
                // Add to approving
                var approvalRecord = round.ApprovingPlayers.SingleOrDefault(p => p.PlayerId == playerId);
                if (approvalRecord == null)
                {
                    round.ApprovingPlayers.Add(player);
                }
            }
            else
            {
                // Remove from approving
                if (round.ApprovingPlayers.SingleOrDefault(p => p.PlayerId == playerId) != null)
                {
                    round.ApprovingPlayers.Remove(player);
                }
                // Add to rejecting
                var rejectionRecord = round.RejectingPlayers.SingleOrDefault(p => p.PlayerId == playerId);
                if (rejectionRecord == null)
                {
                    round.RejectingPlayers.Add(player);
                }
            }
            _db.SaveChanges();
            return round.Game.ToGame();
        }
        
        public Game ClearRoundApprovals(long roundId)
        {
            var round = _db.Rounds
                .Include(r => r.Game)
                .SingleOrDefault(r => r.RoundId == roundId);
            if (round == null)
            {
                return null;
            }
            round.ApprovingPlayers.Clear();
            round.RejectingPlayers.Clear();
            _db.SaveChanges();
            return round.Game.ToGame();
        }
        
        public Game AdvanceGameLeader(long gameId)
        {
            var game = _db.Games.SingleOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                return null;
            }
            game.LeaderCount++;
            _db.SaveChanges();
            return game.ToGame();
        }
        
        public Game SetRoundRejectedCount(long roundId, int rejectedCount)
        {
            var round = _db.Rounds
                .Include(r => r.Game)
                .SingleOrDefault(r => r.RoundId == roundId);
            if (round == null)
            {
                return null;
            }
            round.RejectedCount = rejectedCount;
            _db.SaveChanges();
            return round.Game.ToGame();
        }
        
        public Game SetRoundPlayerPassFail(long roundId, long playerId, bool isPass)
        {
            var round = _db.Rounds
                .Include(r => r.Game)
                .Include(r => r.Game.GamePlayers)
                .ThenInclude(gp => gp.Player)
                .SingleOrDefault(r => r.RoundId == roundId);
            if (round == null)
            {
                return null;
            }
            var player = round.Game.GamePlayers.SingleOrDefault(p => p.PlayerId == playerId)?.Player;
            if (player == null)
            {
                return null;
            }
            if (isPass)
            {
                // Remove from fail
                if (round.FailingPlayers.SingleOrDefault(p => p.PlayerId == playerId) != null)
                {
                    round.FailingPlayers.Remove(player);
                }
                // Add to pass
                var passRecord = round.PassingPlayers.SingleOrDefault(p => p.PlayerId == playerId);
                if (passRecord == null)
                {
                    round.PassingPlayers.Add(player);
                }
            }
            else
            {
                // Remove from pass
                if (round.PassingPlayers.SingleOrDefault(p => p.PlayerId == playerId) != null)
                {
                    round.PassingPlayers.Remove(player);
                }
                // Add to fail
                var failRecord = round.FailingPlayers.SingleOrDefault(p => p.PlayerId == playerId);
                if (failRecord == null)
                {
                    round.FailingPlayers.Add(player);
                }
            }
            _db.SaveChanges();
            return round.Game.ToGame();
        }
        
        public Game SetRoundBadWins(long roundId, bool badWins)
        {
            var round = _db.Rounds
                .SingleOrDefault(r => r.RoundId == roundId);
            if (round == null)
            {
                return null;
            }
            round.BadWins = badWins;
            _db.SaveChanges();
            return round.Game.ToGame();
        }
    }
}