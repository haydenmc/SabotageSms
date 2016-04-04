using SabotageSms.Models;

namespace SabotageSms.Providers
{
    /// <summary>
    /// Interface for fetching and updating game data to a database/storage
    /// </summary>
    public interface IGameDataProvider
    {
        Game GetGameById(long gameId);
        Game GetGameByJoinCode(string joinCode);
        Player GetOrCreatePlayerByPhoneNumber(string phoneNumber);
        Player SetPlayerName(long playerId, string playerName);
        Game GetPlayerCurrentGame(long playerId);
        Game CreateNewGame(long playerId);
        Game JoinPlayerToGame(long playerId, long gameId);
        Game SetPlayersGoodBad(long gameId, bool isBad, params long[] playerIds);
        Game ScrambleTurnOrder(long gameId);
        Game SetGameState(long gameId, string newState);
        Game AddRound(long gameId);
        Round SetRoundSelectedPlayers(long roundId, long[] playerIds);
        Round SetRoundPlayerAsApproving(long roundId, long playerId, bool isApproving);
        Round ClearRoundApprovals(long roundId);
        Game AdvanceGameLeader(long gameId);
        Round SetRoundRejectedCount(long roundId, int rejectedCount);
        Round SetRoundPlayerPassFail(long roundId, long playerId, bool isPass);
        Round SetRoundBadWins(long roundId, bool badWins);
    }
}