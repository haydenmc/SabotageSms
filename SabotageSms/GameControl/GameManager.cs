using System;
using System.Collections.Generic;
using System.Linq;
using SabotageSms.GameControl.States;
using SabotageSms.Models;
using SabotageSms.Providers;

namespace SabotageSms.GameControl
{
    public class GameManager
    {
        public const int MinPlayers = 5;
        
        public const int MaxPlayers = 10;
        
        public static readonly int[,] MissionPlayerNumber = new int[,] 
		{
			{2, 2, 2, 3, 3, 3}, 
			{3, 3, 3, 4, 4, 4}, 
			{2, 4, 3, 4, 4, 4}, 
			{3, 3, 4, 5, 5, 5}, 
			{3, 4, 4, 5, 5, 5}
		};
        
        private AbstractState _currentState { get; set; }
        
        private Game _game { get; set; }
        
        private ISmsProvider _smsProvider { get; set; }
        
        private IGameDataProvider _gameDataProvider { get; set; }
        
        public GameManager(Game game, IGameDataProvider gameDataProvider, ISmsProvider smsProvider)
        {
            _game = game;
            _gameDataProvider = gameDataProvider;
            _smsProvider = smsProvider;
            // TODO: Populate correct state here
            _currentState = new NoGameState(_gameDataProvider, _smsProvider, _game);
        }
        
        public void ExecuteCommand(Player fromPlayer, Command command, object parameters)
        {
            var resultState = _currentState.ProcessCommand(fromPlayer, command, parameters);
            // TODO: Persist new result state in DB
        }
    }
}