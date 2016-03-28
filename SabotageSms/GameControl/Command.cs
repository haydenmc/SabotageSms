namespace SabotageSms.GameControl
{
    public enum Command
    {
        Unknown = 0,
        New = 1,
        Join = 2,
        StartGame = 3,
        SelectRoster = 4,
        ConfirmRoster = 5,
        RosterCountVote = 6,
        RejectRoster = 7,
        ApproveRoster = 8,
        MissionCountSuccessFail = 9,
        NewMission = 10,
        GameEnd = 11
    }
}