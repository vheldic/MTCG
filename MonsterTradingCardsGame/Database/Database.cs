using MonsterTradingCardsGame.GameClasses;

namespace MonsterTradingCardsGame.Database
{
    public class Database
    {
        public Dictionary<string, User> Users { get; private set; }
        public Dictionary<string, string> UserSessions { get; private set; }

        public Database()
        {
            Users = new Dictionary<string, User>();
            UserSessions = new Dictionary<string, string>();
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
    }
}
