using MonsterTradingCardsGame.GameClasses;

namespace MonsterTradingCardsGame.Database
{
    public class Database
    {
        public Dictionary<string, User> Users { get; private set; }

        public Database()
        {
            Users = new Dictionary<string, User>();
        }
    }
}
