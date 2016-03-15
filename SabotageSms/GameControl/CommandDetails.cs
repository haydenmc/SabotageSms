namespace SabotageSms.GameControl
{
    public enum CommandType
    {
        Unknown = 0,
        New = 1,
        Join = 2,
        StartGame = 3,
        SelectRoster = 4,
        RosterCountVote = 5,
        RejectRoster = 6,
        ApproveRoster = 7,
        MissionCountSuccessFail = 8,
        NewMission = 9,
        GameEnd = 10
    }
    
    public class CommandDetails
    {
        public CommandType CommandType { get; set; }
        public string GameJoinCode { get; set; }
    }
}