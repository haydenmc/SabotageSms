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
    }
}