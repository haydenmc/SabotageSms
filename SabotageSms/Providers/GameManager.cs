using System;
using System.Collections.Generic;
using SabotageSms.Models;

namespace SabotageSms.Providers {
    public class GameManager {
        private delegate string CommandExecution(Player player, Game game, CommandDetails command);
        private Dictionary<Tuple<GameState, CommandType>, CommandExecution> StateTransitions = 
            new Dictionary<Tuple<GameState, CommandType>, CommandExecution>() {
                
            };
        
        public void Command(CommandType type) {
            
        }
    }
}