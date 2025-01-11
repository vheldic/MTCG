namespace MonsterTradingCardsGame.GameClasses
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
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

        public User(string username, int gamesplayed, int wins, int draws, int losses, int elo)
        {
            Username = username;
            Password = "";
            Coins = -1;
            GamesPlayed = gamesplayed;
            Wins = wins;
            Draws = draws;
            Losses = losses;
            Elo = elo;
        }
    }
}
