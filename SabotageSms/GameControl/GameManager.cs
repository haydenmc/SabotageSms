using System;
using System.Collections.Generic;
using System.Linq;
using SabotageSms.GameControl.States;
using SabotageSms.Models;
using SabotageSms.Providers;

namespace SabotageSms.GameControl
{
    /// <summary>
    /// GameManager acts as a broker between commands and gamestates
    /// </summary>
    public class GameManager
    {
        /// <summary>
        /// The maximum string length a player's name can be before it is truncated
        /// </summary>
        public const int MaxNameLength = 12;
        
        /// <summary>
        /// The minimum number of players needed to start a game
        /// </summary>
        public const int MinPlayers = 5;
        
        /// <summary>
        /// The maximum number of players allowed in a game 
        /// </summary>
        /// <returns></returns>
        public static int MaxPlayers
        {
            get
            {
                // Calculated by looking at the MissionPlayerNumber table
                return MinPlayers + MissionPlayerNumber.GetLength(1);
            }
        }
        
        /// <summary>
        /// Maximum number of roster rejections before the saboteurs win
        /// </summary>
        public const int MaxRejectionCount = 5;
        
        /// <summary>
        /// Number of mission wins needed for victory
        /// </summary>
        public const int WinCount = 3;
        
        /// <summary>
        /// Defines players per mission by player count and round number.
        /// First dimension is round number, second dimension is player count.
        /// </summary>
        public static readonly int[,] MissionPlayerNumber = new int[,] 
		{
			{2, 2, 2, 3, 3, 3}, // Round 1
			{3, 3, 3, 4, 4, 4}, // Round 2
			{2, 4, 3, 4, 4, 4}, // Round 3
			{3, 3, 4, 5, 5, 5}, // Round 4
			{3, 4, 4, 5, 5, 5}  // Round 5
		};
        
        /// <summary>
        /// Defines the number of 'fail' votes required to fail a mission.
        /// First dimension is round number, second dimension is player count.
        /// </summary>
        public static readonly int[,] MissionRequiredFailCount = new int[,] 
		{
			{1, 1, 1, 1, 1, 1}, // Round 1
			{1, 1, 1, 1, 1, 1}, // Round 2
			{1, 1, 1, 1, 1, 1}, // Round 3
			{1, 1, 2, 2, 2, 2}, // Round 4
			{1, 1, 1, 1, 1, 1}  // Round 5
		};
        
        /// <summary>
        /// The current state of the game
        /// </summary>
        private AbstractState _currentState { get; set; }
        
        /// <summary>
        /// The game model this manager is tracking
        /// </summary>
        private Game _game { get; set; }
        
        /// <summary>
        /// The SMS provider used to message players
        /// </summary>
        private ISmsProvider _smsProvider { get; set; }
        
        /// <summary>
        /// The game data provider used to update the database
        /// </summary>
        private IGameDataProvider _gameDataProvider { get; set; }
        
        public GameManager(Game game, IGameDataProvider gameDataProvider, ISmsProvider smsProvider)
        {
            _game = game;
            _gameDataProvider = gameDataProvider;
            _smsProvider = smsProvider;

            // Instantiate proper game state based on string from database
            _currentState = new NoGameState(_gameDataProvider, _smsProvider, _game);
            if (_game != null)
            {
                var stateName = _game.CurrentState;
                var type = Type.GetType("SabotageSms.GameControl.States." + stateName);
                if (type != null)
                {
                    // Ensure params aligns with AbstractState constructor
                    _currentState = (AbstractState) Activator.CreateInstance(type, _gameDataProvider, _smsProvider, _game);
                }
            }
        }
        
        /// <summary>
        /// Runs a command from a player against the current game state
        /// </summary>
        /// <param name="fromPlayer">The player executing this command</param>
        /// <param name="command">The command to execute</param>
        /// <param name="parameters">Parameters for the command</param>
        public void ExecuteCommand(Player fromPlayer, Command command, object parameters = null)
        {
            // Check if this player has a name, ask them to set one if they don't.
            if (command != Command.Name && (fromPlayer.Name == null || fromPlayer.Name.Length <= 0))
            {
                _smsProvider.SendSms(fromPlayer.PhoneNumber, GameStrings.YouNeedAName);
                return;
            }
            var resultState = _currentState.ProcessCommand(fromPlayer, command, parameters);
            if (resultState == null)
            {
                resultState = new NoGameState(_gameDataProvider, _smsProvider, _game).ProcessCommand(fromPlayer, command, parameters);
                if (resultState == null)
                {
                    _smsProvider.SendSms(fromPlayer.PhoneNumber, GameStrings.UnknownCommand);
                    return;
                }
            }
            if (_game != null)
            {
                _gameDataProvider.SetGameState(_game.GameId, resultState.GetType().Name);
            }
        }
    }
}