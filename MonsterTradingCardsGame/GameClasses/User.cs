namespace MonsterTradingCardsGame.GameClasses
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Coins { get; set; }
        public int Elo { get; set; }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Coins = 20;
            Elo = 100;
        }
    }
}
