using System;
using System.Collections.Generic;

namespace SabotageSms.Providers {
    public class GameManager {
        // We may want to have two separate tables... States and Commands.
        private enum GameState {
            Lobby = 0,
            NewPlayer = 1,
            GameStart = 2,
            MissionRoster = 3,
            MissionApproveReject = 4,
            TallyApproveReject = 5,
            MissionSuccessFail = 6,
            TallySuccessFail = 7,
            MissionResult = 8,
            GameEnd = 9
        }
        
        private delegate void StateTransition();
        
        private Dictionary<Tuple<GameState, GameState>, StateTransition> Transitions;
    }
}