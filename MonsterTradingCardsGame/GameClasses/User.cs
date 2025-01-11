namespace MonsterTradingCardsGame.GameClasses
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public int Coins { get; set; }
        public int Elo { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GamesPlayed { get; set; }

        public User() { }
        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Coins = 20;
            GamesPlayed = 0;
            Wins = 0;
            Draws = 0;
            Losses = 0;
            Elo = 100;
        }
    }
}
