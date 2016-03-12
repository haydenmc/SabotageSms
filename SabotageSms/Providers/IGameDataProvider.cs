using SabotageSms.Models;

namespace SabotageSms.Providers
{
    public interface IGameDataProvider
    {
        Player GetPlayerByPhoneNumber(string phoneNumber);
        Game GetPlayerCurrentGame(long playerId);
        Game CreateNewGame(long playerId);
    }
}