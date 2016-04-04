using System;
using System.Linq;
using Xunit;
using SabotageSms.GameControl;
using Microsoft.Data.Entity;
using SabotageSms.Models.DbModels;
using SabotageSms.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Entity.Infrastructure;
using Xunit.Abstractions;
using SabotageSms.GameControl.States;

namespace SabotageSms.Tests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dnx.html
    public class GameTest
    {
        private ITestOutputHelper _output;
        private ServiceCollection _serviceCollection;
        private DbContextOptions<ApplicationDbContext> _contextOptions;
        private ApplicationDbContext _db;
        
        private static readonly string[,] _playerInfo = new string[,]
        {
            {"TaylorSwift", "18008675309"},
            {"MisterButt",  "18008675310"},
            {"DoctorButt",  "18008675311"},
            {"SenorButt",   "18008675312"},
            {"CaptainButt", "18008675313"},
            {"ProfButt",    "18008675314"},
            {"MrsButt",     "18008675315"},
            {"ImaButt",     "18008675316"},
            {"ButtMan",     "18008675317"},
            {"Butt",        "18008675318"},
        };
        
        public GameTest(ITestOutputHelper output)
        {
            _output = output;
            
            // Create a service collection that we can create service providers from
            // A service collection defines the services that will be available in service 
            // provider instances (think of it as ServiceProviderBuilder)
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddEntityFramework().AddInMemoryDatabase();

            // Create options to tell the context to use the InMemory database
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _contextOptions = optionsBuilder.Options;
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            _db = new ApplicationDbContext(serviceProvider, _contextOptions);
        }
        
        private void Sms(string fromNumber, string body)
        {
            var gameDataProvider = new EfGameDataProvider(_db);
            var parsingProvider = new ParsingProvider();
            var smsProvider = new MockSmsProvider(_output);
            var controller = new MockSmsController(gameDataProvider, parsingProvider, smsProvider);
            controller.Sms(fromNumber, body);
        }
        
        private void NamePlayers(int playerCount = 5)
        {
            for (var i = 0; i < playerCount; i++)
            {
                Sms(_playerInfo[i, 1], "Name " + _playerInfo[i, 0]);
            }
            
            // Check that players have been created and named properly
            for (var i = 0; i < playerCount; i++)
            {
                var player = _db.Players.SingleOrDefault(p => p.PhoneNumber == _playerInfo[i, 1] && p.Name == _playerInfo[i, 0]);
                Assert.NotNull(player);
            }
        }
        
        [Theory]
        [InlineData(5)]
        public void RunGame(int numPlayers = GameManager.MinPlayers)
        {
            var rand = new Random();
            
            // Join up players
            NamePlayers(numPlayers);
            
            // Create new game
            Sms(_playerInfo[0, 1], "New");
            
            // Verify new game
            var player = _db.Players
                .Include(p => p.CurrentGame)
                .SingleOrDefault(p => p.PhoneNumber == _playerInfo[0, 1]);
            Assert.NotNull(player);
            Assert.NotNull(player.CurrentGame);
            
            // Join remaining players
            for (var i = 1; i < numPlayers; i++)
            {
                Sms(_playerInfo[i, 1], "Join " + player.CurrentGame.JoinCode);
            }
            
            // Verify all players are in the game
            var game = _db.Games
                .Include(g => g.GamePlayers)
                .ThenInclude(gp => gp.Player)
                .SingleOrDefault(g => g.GameId == player.CurrentGame.GameId);
            Assert.NotNull(game);
            var gameId = game.GameId;
            for (var i = 0; i < numPlayers; i++)
            {
                Assert.NotNull(
                    game.GamePlayers
                        .SingleOrDefault(p => p.Player.PhoneNumber == _playerInfo[i, 1]
                            && p.Player.Name == _playerInfo[i, 0])
                );
            }
            
            // Start the game
            Sms(_playerInfo[0, 1], "Start");
            
            // Verify the game has moved to the roster state
            game = _db.Games
                .Hydrate()
                .SingleOrDefault(g => g.GameId == gameId);
            Assert.Equal(game.CurrentState, typeof(RosterState).Name);
            
            // Run each round
            while (true)
            {
                var leader = game.GamePlayers
                    .OrderBy(gp => gp.TurnOrder)
                    .ToList()[game.LeaderCount % game.GamePlayers.Count];
                var missionPlayerCount = GameManager
                    .MissionPlayerNumber[game.Rounds.Count - 1, game.GamePlayers.Count - GameManager.MinPlayers];
                    
                // Select players
                var selectedNames = new string[missionPlayerCount];
                var selectedNumbers = new string[missionPlayerCount];
                for (var i = 0; i < selectedNames.Length; i++)
                {
                    selectedNames[i] = _playerInfo[i, 0];
                    selectedNumbers[i] = _playerInfo[i, 1];
                }
                Sms(leader.Player.PhoneNumber, "Select " + string.Join(", ", selectedNames));
                
                // Confirm selection
                Sms(leader.Player.PhoneNumber, "Confirm");
                
                // Randomly approve/reject
                // TODO: This makes this test nondeterministic... rethink
                for (var i = 0; i < numPlayers; i++)
                {
                    if (rand.NextDouble() > 0.5)
                    {
                        Sms(_playerInfo[i, 1], "Approve");
                    }
                    else
                    {
                        Sms(_playerInfo[i, 1], "Reject");
                    }
                }
                
                game = _db.Games
                    .Hydrate()
                    .SingleOrDefault(g => g.GameId == gameId);
                if (game.CurrentState != typeof(MissionState).Name)
                {
                    continue;
                }
                
                // Randomly pass/fail
                // TODO: This makes this test nondeterministic... rethink
                if (rand.NextDouble() > 0.5)
                {
                    var numFails = GameManager.MissionRequiredFailCount[game.Rounds.Count - 1, game.GamePlayers.Count - GameManager.MinPlayers];
                    for (var i = 0; i < numFails; i++)
                    {
                        Sms(selectedNumbers[i], "Fail");
                    }
                    for (var i = numFails; i < selectedNumbers.Length; i++)
                    {
                        Sms(selectedNumbers[i], "Pass");
                    }
                    game = _db.Games
                        .Hydrate()
                        .SingleOrDefault(g => g.GameId == gameId);
                    var lastRound = _db.Rounds
                        .Where(r => r.GameId == game.GameId)
                        .Where(r => r.ApprovingPlayers.Count + r.RejectingPlayers.Count == game.GamePlayers.Count)
                        .OrderByDescending(r => r.RoundNumber)
                        .FirstOrDefault();
                    Assert.True(lastRound.BadWins);
                }
                else
                {
                    for (var i = 0; i < selectedNumbers.Length; i++)
                    {
                        Sms(selectedNumbers[i], "Pass");
                    }
                    game = _db.Games
                        .Hydrate()
                        .SingleOrDefault(g => g.GameId == gameId);
                    var lastRound = _db.Rounds
                        .Where(r => r.GameId == game.GameId)
                        .Where(r => r.ApprovingPlayers.Count + r.RejectingPlayers.Count == game.GamePlayers.Count)
                        .OrderByDescending(r => r.RoundNumber)
                        .FirstOrDefault();
                    Assert.False(lastRound.BadWins);
                }
                
                game = _db.Games
                    .Hydrate()
                    .SingleOrDefault(g => g.GameId == gameId);
                
                if (game.CurrentState != typeof(RosterState).Name)
                {
                    break;
                }
            }
            
            Assert.Equal(game.CurrentState, typeof(GameOverState).Name);
        }
    }
}
