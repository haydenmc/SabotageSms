namespace SabotageSms.GameControl
{
    /// <summary>
    /// Acts as a container for game strings
    /// TODO: Enable localization, store in a resource file somewhere
    /// </summary>
    public static class GameStrings
    {
        public const string UnknownCommand             = "Sorry, I didn't understand you. Please try another command.";
        public const string YouNeedAName               = "Hello! Before you begin, you need to set a name. Please reply with 'name YOURNAME'.";
        public const string NameRequirements           = "Your name cannot be blank, and can not have any special characters.";
        public const string DuplicateName              = "Someone else is in your game is already using this name.";
        public const string NameSet                    = "Your name has been set to '{0}'. 'New' or 'Join CODEHERE'.";
        public const string PlayerHasChangedName       = "'{0}' has changed their name to '{1}'.";
        public const string NewGameCreated             = "New game created! Tell others to text 'Join {0}' to this number.";
        public const string CouldNotFindGame           = "We couldn't find that game.";
        public const string CannotJoinGameInProgress   = "You can't join a game in progress.";
        public const string GameIsFull                 = "This game is full.";
        public const string YouHaveJoined              = "You have joined the game! There are {0} players currently in this game.";
        public const string NewPlayerJoined            = "{0} has joined the game! There are {1} players currently in this game.";
        public const string YouAreBad                  = "TOP SECRET: You are a saboteur!";
        public const string YouAreGood                 = "TOP SECRET: You are good!";
        public const string NeedMorePlayers            = "You cannot start the game yet. You need between {0} and {1} players.";
        public const string MissionStart               = "MISSION START: {0}.";
        public const string MissionStartYou            = "MISSION START: You, {0}. 'Pass' or 'Fail'.";
        public const string YouAreNotOnThisMission     = "You were not selected for this mission.";
        public const string MissionSabotaged           = "MISSION SABOTAGED: {0} pass, {1} fail.";
        public const string MissionSucceeded           = "MISSION SUCCEEDED: {0} pass, {1} fail.";
        public const string SaboteursWin               = "GAME OVER: SABOTEURS WIN.\nSaboteurs: {0}";
        public const string SaboteursLose              = "GAME OVER: SABOTEURS LOSE.\nSaboteurs: {0}";
        public const string ResponseRecordedWaiting    = "Response recorded. Still waiting on {0} players.";
        public const string PlayersSelectedForApproval = "{0} have been selected. Vote 'approve' or 'reject'.";
        public const string ApproveRejectList          = "Approve: {0}\nReject: {1}";
        public const string MissionApproved            = "MISSION APPROVED.\n{0}";
        public const string MissionRejected            = "MISSION REJECTED. {0} rejections remain.\n{1}";
        public const string OnlyLeaderCanConfirmRoster = "Only {0} can confirm the mission roster.";
        public const string FailCountWarning           = "*{0} fails required to sabotage this mission.";
        public const string NewRoundAnnounce           = "ROUND {0}: {1} leader. {2} players required.{3}";
        public const string NewRoundAnnounceForLeader  = "ROUND {0}: You are the leader. 'Select' {1} players.{2}";
        public const string OnlyLeaderCanSelectRoster  = "Only {0} can choose players to participate in this mission.";
        public const string MustSelectNumberOfPlayers  = "You need to select {0} players for this mission.";
        public const string CouldNotFindPlayerByName   = "We couldn't find a player named '{0}'.";
        public const string RosterSelectedForLeader    = "{0} have been selected, pending your 'confirm'.";
        public const string RosterSelected             = "{0} have been selected, pending {1}'s confirmation.";
    }
}