using MonsterTradingCardsGame.GameClasses;

namespace MonsterTradingCardsGame.Database
{
    public class Database
    {
        public Dictionary<string, User> Users { get; private set; }
        public Dictionary<string, string> UserSessions { get; private set; }
        public Dictionary<string, List<Card>> Cards { get; private set; }
        public Dictionary<string, List<Card>> Decks { get; private set; }

        public Database()
        {
            Users = new Dictionary<string, User>();
            UserSessions = new Dictionary<string, string>();
            Cards = new Dictionary<string, List<Card>>();
            Decks = new Dictionary<string, List<Card>>();
        }

        /// <summary>
        ///     Checks, if the given user exists in the database
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>If username exists</returns>
        public bool CheckUserExists(string username)
        {
            if (Users.ContainsKey(username)) return true;
            return false;
        }

        /// <summary>
        ///     Registers a user by adding to the database
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="password">Password of user</param>
        public void RegisterUser(string username, string password)
        {
            Users.Add(username, new User(username, password));
        }

        /// <summary>
        ///     Logs in a user with their username and password and adds the session to the database
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="password">Password of user</param>
        /// <returns>Generated token for user actions</returns>
        public string LoginUser(string username, string password)
        {
            // Check if password is correct
            if (Users[username].Password != password) return "";

            // Generate token and add session to database
            string token = GenerateToken(username);
            UserSessions.Add(token, username); // TODO: später mit userid?

            return token;
        }

        /// <summary>
        ///     Generates a token for user action based on the given username
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>Generated token for user action</returns>
        private string GenerateToken(string username)
        {
            return $"{username}-mtcgToken";
        }

        /// <summary>
        ///     Checks, if the given token is valid
        /// </summary>
        /// <param name="token">Bearer token used for user action</param>
        /// <returns>If token is valid</returns>
        public bool CheckTokenIsValid(string token)
        {
            // Check if given token is saved in database
            if (!UserSessions.ContainsKey(token)) return false;
            return true;
        }

        /// <summary>
        ///     Gets the username from the token
        /// </summary>
        /// <param name="token">Bearer token used for user action</param>
        /// <returns>Username to which the token is associated</returns>
        public string GetUsernameFromToken(string token)
        {
            return UserSessions[token];
        }

        /// <summary>
        ///     Gets all cards that belong to the given user
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>List of card's from user</returns>
        public List<Card> GetUsersCards(string username)
        {
            return Cards[username];
        }

        /// <summary>
        ///     Gets the card belonging to the user with the given username that has the given id
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="id">Id of card</param>
        /// <returns>The user's card with the given id</returns>
        public Card? GetUsersCardById(string username, string id)
        {
            foreach (Card card in Cards[username])
            {
                if (card.Name == id) return card; // TODO: id statt name
            }
            return null;
        }

        /// <summary>
        ///     Gets the deck of the given user that that is saved in the database
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>Saved deck of user</returns>
        public List<Card> GetUsersDeck(string username)
        {
            if (!Decks.ContainsKey(username)) return [];
            return Decks[username];
        }

        /// <summary>
        ///     Updates the deck of the user
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="cards">List of cards to update deck</param>
        public void UpdateUsersDeck(string username, List<Card> cards)
        {
            Decks[username] = cards;
        }

        /// <summary>
        ///     Gets the stats of a user
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>Stats of user as json-string</returns>
        public User GetUser(string username)
        {
            return Users[username];
        }

        /// <summary>
        ///     Gets the scoreboard sorted by the elo
        /// </summary>
        /// <returns>List of users sorted by their elo</returns>
        public List<User> GetScoreboard()
        {
            List<User> scoreboard = new List<User>();
            // TODO: sort by elo
            foreach (KeyValuePair<string, User> user in Users)
            {
                scoreboard.Add(user.Value);
            }

            return scoreboard;
        }
    }
}
