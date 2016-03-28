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
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            using (var db = new ApplicationDbContext(serviceProvider, _contextOptions))
            {
                IGameDataProvider gameDataProvider = new EfGameDataProvider(db);
                ISmsProvider smsProvider = new MockSmsProvider(_output);
                var playerOne = gameDataProvider.GetOrCreatePlayerByPhoneNumber("18008675309");
                Assert.Equal(playerOne.PhoneNumber, "18008675309");
                playerOne = gameDataProvider.SetPlayerName(playerOne.PlayerId, "TaylorSwift");
                
                // Simulate new game command
                var gameManager = new GameManager(null, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(playerOne, Command.New);
                
                // Fetch the game
                var game = gameDataProvider.GetPlayerCurrentGame(playerOne.PlayerId);
                Assert.NotNull(game);
                
                // Join a second player
                var playerTwo = gameDataProvider.GetOrCreatePlayerByPhoneNumber("18008675310");
                Assert.Equal(playerTwo.PhoneNumber, "18008675310");
                playerTwo = gameDataProvider.SetPlayerName(playerTwo.PlayerId, "PooppyGeorge");
                gameManager = new GameManager(null, gameDataProvider, smsProvider); // Game is null as this player is not in a game
                gameManager.ExecuteCommand(playerTwo, Command.Join, game.JoinCode);
                
                // Fetch the game, verify players
                game = gameDataProvider.GetGameById(game.GameId);
                Assert.NotNull(game);
                Assert.True(game.Players.Where(p => p.PhoneNumber == "18008675309").Count() > 0);
                Assert.True(game.Players.Where(p => p.PhoneNumber == "18008675310").Count() > 0);
                
                // Join three more players so we have five
                for (var i = 0; i < 3; i++)
                {
                    var p = gameDataProvider.GetOrCreatePlayerByPhoneNumber("1800867531" + (i + 1));
                    p = gameDataProvider.SetPlayerName(p.PlayerId, "TestP" + (i + 1));
                    gameManager = new GameManager(null, gameDataProvider, smsProvider);
                    gameManager.ExecuteCommand(p, Command.Join, game.JoinCode);
                }
                
                // Start the game!
                game = gameDataProvider.GetGameById(game.GameId);
                gameManager = new GameManager(game, gameDataProvider, smsProvider);
                gameManager.ExecuteCommand(playerOne, Command.StartGame);
                Assert.True(false);
            }
        }
    }
}
