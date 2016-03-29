using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SabotageSms.GameControl;
using Microsoft.Data.Entity;
using SabotageSms.Models.DbModels;
using SabotageSms.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Entity.Infrastructure;
using Xunit.Abstractions;
using SabotageSms.Models;

namespace SabotageSms.Tests
{
    // see example explanation on xUnit.net website:
    // https://xunit.github.io/docs/getting-started-dnx.html
    public class GameTest
    {
        private ITestOutputHelper _output;
        private ServiceCollection _serviceCollection;
        private DbContextOptions<ApplicationDbContext> _contextOptions;
        
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
        }
        
        [Fact]
        public void FivePlayerGame()
        {
            var playerNames = new string[] {
                "TaylorSwift",
                "MisterButt",
                "DoctorButt",
                "SenorButt",
                "CaptainButt"
            };
            var players = new Player[playerNames.Length];
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            using (var db = new ApplicationDbContext(serviceProvider, _contextOptions))
            {
                IGameDataProvider gameDataProvider = new EfGameDataProvider(db);
                ISmsProvider smsProvider = new MockSmsProvider(_output);
                
                for (var i = 0; i < playerNames.Length; i++)
                {
                    players[i] = gameDataProvider.GetOrCreatePlayerByPhoneNumber("1800867530" + i);
                    Assert.Equal(players[i].PhoneNumber, "1800867530" + i);
                    players[i] = gameDataProvider.SetPlayerName(players[i].PlayerId, playerNames[i]);
                }
                
                // Simulate new game command
                var gameManager = new GameManager(null, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(players[0], Command.New);
                
                // Fetch the game
                var game = gameDataProvider.GetPlayerCurrentGame(players[0].PlayerId);
                Assert.NotNull(game);
                
                // Join a second player
                gameManager = new GameManager(null, gameDataProvider, smsProvider); // Game is null as this player is not in a game
                gameManager.ExecuteCommand(players[1], Command.Join, game.JoinCode);
                
                // Fetch the game, verify players
                game = gameDataProvider.GetGameById(game.GameId);
                Assert.NotNull(game);
                Assert.True(game.Players.Where(p => p.Name == playerNames[0]).Count() > 0);
                Assert.True(game.Players.Where(p => p.Name == playerNames[1]).Count() > 0);
                
                // Join three more players so we have five
                for (var i = 2; i < players.Length; i++)
                {
                    gameManager = new GameManager(null, gameDataProvider, smsProvider); // Game is null as this player is not in a game
                    gameManager.ExecuteCommand(players[i], Command.Join, game.JoinCode);
                }
                
                // Start the game!
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(players[0], Command.StartGame);
                
                // Select mission peeps
                game = gameDataProvider.GetGameById(game.GameId);
                var leader = game.Players[game.LeaderCount % game.Players.Count];
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.SelectRoster, new string[] {
                    playerNames[0],
                    playerNames[1]
                });
                
                // Select a wrong number
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.SelectRoster, new string[] {
                    playerNames[0]
                });
                
                // Select a non-existing player
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.SelectRoster, new string[] {
                    playerNames[0],
                    "Vlad"
                });
                
                // Change it up
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.SelectRoster, new string[] {
                    playerNames[1],
                    playerNames[2]
                });
                
                // Confirm
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.ConfirmRoster);
                
                // Reject the mission 3 to 2.
                for (var i = 0; i < 2; i++)
                {
                    game = gameDataProvider.GetGameById(game.GameId);
                    gameManager = new GameManager(game, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(players[i], Command.ApproveRoster);
                }
                for (var i = 2; i < players.Length; i++)
                {
                    game = gameDataProvider.GetGameById(game.GameId);
                    gameManager = new GameManager(game, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(players[i], Command.RejectRoster);
                }
                
                // Select mission peeps
                game = gameDataProvider.GetGameById(game.GameId);
                leader = game.Players[game.LeaderCount % game.Players.Count];
                _output.WriteLine("Leader = " + leader.Name + "?");
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.SelectRoster, new string[] {
                    playerNames[0],
                    playerNames[1]
                });
                
                // Confirm
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.ConfirmRoster);
                
                // Approve the mission unanimously.
                for (var i = 0; i < players.Length; i++)
                {
                    game = gameDataProvider.GetGameById(game.GameId);
                    gameManager = new GameManager(game, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(players[i], Command.ApproveRoster);
                }
                
                // Pass the mission
                for (var i = 0; i < 2; i++)
                {
                    game = gameDataProvider.GetGameById(game.GameId);
                    gameManager = new GameManager(game, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(players[i], Command.PassMission);
                }
                
                // Select mission peeps
                game = gameDataProvider.GetGameById(game.GameId);
                leader = game.Players[game.LeaderCount % game.Players.Count];
                _output.WriteLine("Leader = " + leader.Name + "?");
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.SelectRoster, new string[] {
                    playerNames[0],
                    playerNames[1],
                    playerNames[2]
                });
                
                // Confirm
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.ConfirmRoster);
                
                // Approve the mission unanimously.
                for (var i = 0; i < players.Length; i++)
                {
                    game = gameDataProvider.GetGameById(game.GameId);
                    gameManager = new GameManager(game, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(players[i], Command.ApproveRoster);
                }
                
                // Fail the mission
                for (var i = 0; i < 2; i++)
                {
                    game = gameDataProvider.GetGameById(game.GameId);
                    gameManager = new GameManager(game, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(players[i], Command.PassMission);
                }
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(players[2], Command.FailMission);
                
                // Select mission peeps
                game = gameDataProvider.GetGameById(game.GameId);
                leader = game.Players[game.LeaderCount % game.Players.Count];
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.SelectRoster, new string[] {
                    playerNames[0],
                    playerNames[1]
                });
                
                // Confirm
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.ConfirmRoster);
                
                // Approve the mission unanimously.
                for (var i = 0; i < players.Length; i++)
                {
                    game = gameDataProvider.GetGameById(game.GameId);
                    gameManager = new GameManager(game, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(players[i], Command.ApproveRoster);
                }
                
                // Fail the mission
                for (var i = 0; i < 1; i++)
                {
                    game = gameDataProvider.GetGameById(game.GameId);
                    gameManager = new GameManager(game, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(players[i], Command.PassMission);
                }
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(players[1], Command.FailMission);
                
                // Select mission peeps
                game = gameDataProvider.GetGameById(game.GameId);
                leader = game.Players[game.LeaderCount % game.Players.Count];
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.SelectRoster, new string[] {
                    playerNames[0],
                    playerNames[1],
                    playerNames[2]
                });
                
                // Confirm
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(leader, Command.ConfirmRoster);
                
                // Approve the mission unanimously.
                for (var i = 0; i < players.Length; i++)
                {
                    game = gameDataProvider.GetGameById(game.GameId);
                    gameManager = new GameManager(game, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(players[i], Command.ApproveRoster);
                }
                
                // Fail the mission
                for (var i = 0; i < 2; i++)
                {
                    game = gameDataProvider.GetGameById(game.GameId);
                    gameManager = new GameManager(game, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(players[i], Command.PassMission);
                }
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(players[2], Command.FailMission);
                
                // Make this thing fail so I can see the output (shake fist)
                Assert.True(false);
            }
        }
    }
}
