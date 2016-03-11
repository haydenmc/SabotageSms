namespace SabotageSms.Models {
    public enum CommandType {
        Unknown = 0,
        AddPlayer = 1,
        StartGame = 2,
        SelectRoster = 3,
        RosterCountVote = 4,
        RejectRoster = 5,
        ApproveRoster = 6,
        MissionCountSuccessFail = 7,
        NewMission = 8,
        GameEnd = 9
    }
    
    public class CommandDetails {
        public CommandType CommandType;
    }
}