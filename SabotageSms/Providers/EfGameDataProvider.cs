using System;
using System.Linq;
using SabotageSms.Models;
using SabotageSms.Models.DbModels;

namespace SabotageSms.Providers
{
    public class EfGameDataProvider : IGameDataProvider
    {
        private ApplicationDbContext _db;
        
        public EfGameDataProvider(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public Player GetPlayerByPhoneNumber(string phoneNumber)
        {
            return _db.Players.SingleOrDefault(p => p.PhoneNumber == phoneNumber);
        }

        public Game GetPlayerCurrentGame(long playerId)
        {
            return _db.Players.SingleOrDefault(p => p.PlayerId == playerId).CurrentGame;
        }
        
        public Game CreateNewGame(long playerId) {
            
        }
    }
}