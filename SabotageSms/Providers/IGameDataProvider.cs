using SabotageSms.Models;

namespace SabotageSms.Providers
{
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
        Player GetGamePlayerByName(long gameId, string playerName);
        Game AddRound(long gameId);
        Game SetRoundSelectedPlayers(long roundId, long[] playerIds);
        Game SetRoundPlayerAsApproving(long roundId, long playerId, bool isApproving);
        Game ClearRoundApprovals(long roundId);
        Game AdvanceGameLeader(long gameId);
        Game SetRoundRejectedCount(long roundId, int rejectedCount);
        Game SetRoundPlayerPassFail(long roundId, long playerId, bool isPass);
        Game SetRoundBadWins(long roundId, bool badWins);
    }
}