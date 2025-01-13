using MonsterTradingCardsGame.GameClasses;
using Npgsql;

namespace MonsterTradingCardsGame.Database
{
    public class Database
    {
        private string connectionString = "Host=localhost;Port=5432;Database=mtcgdb;User Id=velid;Password=Pass2020;";
        private NpgsqlConnection? connection;

        public Database()
        {
            DeleteDB();
            SetupDB();
        }

        public void DeleteDB()
        {
            string query = """
                DROP TABLE IF EXISTS decks;
                DROP TABLE IF EXISTS packages;
                DROP TABLE IF EXISTS cards;
                DROP TABLE IF EXISTS usersessions;
                DROP TABLE IF EXISTS users;
                """;

            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }

            Console.WriteLine("Deleted all tables from the database");
        }

        public void SetupDB()
        {
            string query = """
                CREATE TABLE IF NOT EXISTS users (
                    userID SERIAL PRIMARY KEY,
                    username VARCHAR(255) UNIQUE NOT NULL,
                    password VARCHAR(255) NOT NULL,
                    name VARCHAR(255), 
                    bio VARCHAR(255), 
                    image VARCHAR(255), 
                    coins INTEGER DEFAULT 20 NOT NULL,
                    elo INTEGER DEFAULT 100 NOT NULL,
                    wins INTEGER DEFAULT 0 NOT NULL,
                    draws INTEGER DEFAULT 0 NOT NULL,
                    losses INTEGER DEFAULT 0 NOT NULL,
                    gamesPlayed INTEGER DEFAULT 0 NOT NULL,
                    spindate DATE
                );

                CREATE TABLE IF NOT EXISTS usersessions (
                    sessionID SERIAL PRIMARY KEY,
                    userID INTEGER,
                    token VARCHAR(255) NOT NULL,
                    FOREIGN KEY (userID) REFERENCES users(userID)
                );
                
                CREATE TABLE IF NOT EXISTS cards (
                    cardID VARCHAR(255) PRIMARY KEY,
                    name VARCHAR(255) NOT NULL,
                    element VARCHAR(50) NOT NULL,
                    damage DECIMAL NOT NULL,
                    type VARCHAR(50) NOT NULL,
                    monstertype VARCHAR(50),
                    userID INTEGER,
                    FOREIGN KEY (userID) REFERENCES users(userID)
                );
                
                CREATE TABLE IF NOT EXISTS packages (
                    packID SERIAL PRIMARY KEY,
                    card1ID VARCHAR(255),
                    card2ID VARCHAR(255),
                    card3ID VARCHAR(255),
                    card4ID VARCHAR(255),
                    card5ID VARCHAR(255),
                    FOREIGN KEY (card1ID) REFERENCES cards(cardID),
                    FOREIGN KEY (card2ID) REFERENCES cards(cardID),
                    FOREIGN KEY (card3ID) REFERENCES cards(cardID),
                    FOREIGN KEY (card4ID) REFERENCES cards(cardID),
                    FOREIGN KEY (card5ID) REFERENCES cards(cardID)
                );

                CREATE TABLE IF NOT EXISTS decks (
                    deckID SERIAL PRIMARY KEY,
                    userID INTEGER,
                    card1ID VARCHAR(255),
                    card2ID VARCHAR(255),
                    card3ID VARCHAR(255),
                    card4ID VARCHAR(255),
                    FOREIGN KEY (userID) REFERENCES users(userID),
                    FOREIGN KEY (card1ID) REFERENCES cards(cardID),
                    FOREIGN KEY (card2ID) REFERENCES cards(cardID),
                    FOREIGN KEY (card3ID) REFERENCES cards(cardID),
                    FOREIGN KEY (card4ID) REFERENCES cards(cardID)
                );
                """;

            // Start database connection
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Created all tables in the database");
        }

        /// <summary>
        ///     Checks, if the given user exists in the database
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>If username exists</returns>
        public bool CheckUserExists(string username)
        {
            bool exists = false;
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT 1 FROM users WHERE username = @username LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    exists = cmd.ExecuteScalar() != null;
                }
                connection.Close();
            }

            return exists;
        }

        /// <summary>
        ///     Registers a user by adding to the database
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="password">Password of user</param>
        public void RegisterUser(string username, string password)
        {
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@username, @password)", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
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
            bool passwordIsCorrect = false;
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT 1 FROM users WHERE username = @username AND password = @password LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    passwordIsCorrect = cmd.ExecuteScalar() != null;
                }
                connection.Close();
            }
            if (!passwordIsCorrect) return "";

            // Generate token and add session to database
            string token = GenerateToken(username);
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO usersessions (userID, token) VALUES ((SELECT userID FROM users WHERE username = @username), @token)", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@token", token);

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }

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
            bool isValid = false;
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT 1 FROM usersessions WHERE token = @token LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@token", token);

                    isValid = cmd.ExecuteScalar() != null;
                }
                connection.Close();
            }

            return isValid;
        }

        /// <summary>
        ///     Checks, if user is admin
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>If user is admin</returns>
        public bool CheckUserIsAdmin(string username)
        {
            // Check if user is admin
            if (username != "admin") return false;
            return true;
        }

        /// <summary>
        ///     Gets the username from the token
        /// </summary>
        /// <param name="token">Bearer token used for user action</param>
        /// <returns>Username to which the token is associated</returns>
        public string GetUsernameFromToken(string token)
        {
            string? username = "";
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT username FROM users WHERE userID = (SELECT userID FROM usersessions WHERE token = @token)", connection))
                {
                    cmd.Parameters.AddWithValue("@token", token);

                    username = cmd.ExecuteScalar() as string;
                }
                connection.Close();
            }

            return username ?? "";
        }

        /// <summary>
        ///     Gets the stats of a user
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>Stats of user as json-string</returns>
        public User? GetUser(string username)
        {
            User? user = null;
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT username, name, bio, image, elo, wins, draws, losses, gamesplayed FROM users WHERE username = @username", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Username = reader["username"].ToString() ?? "",
                                Name = reader["name"].ToString() ?? "",
                                Bio = reader["bio"].ToString() ?? "",
                                Image = reader["image"].ToString() ?? "",
                                Elo = (int)reader["elo"],
                                Wins = (int)reader["wins"],
                                Draws = (int)reader["draws"],
                                Losses = (int)reader["losses"],
                                GamesPlayed = (int)reader["gamesplayed"]
                            };
                        }
                    }
                }
                connection.Close();
            }

            return user;
        }

        /// <summary>
        ///     Edits the profile page of user
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="name">Name of user</param>
        /// <param name="bio">Bio of user</param>
        /// <param name="image">Image of user</param>
        public void EditProfile(string username, string name, string bio, string image)
        {
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE users SET name = @name, bio = @bio, image = @image WHERE username = @username", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@bio", bio);
                    cmd.Parameters.AddWithValue("@image", image);

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        ///     Updates the stats of a user after a battle
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="elo">New elo of user</param>
        /// <param name="wins">New amount of user's wins</param>
        /// <param name="draws">New amount of user's draws</param>
        /// <param name="losses">New amount of user's losses</param>
        /// <param name="gamesplayed">New amount of user's played games</param>
        public void UpdateUserStats(string username, int elo, int wins, int draws, int losses, int gamesplayed)
        {
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE users SET elo = @elo, wins = @wins, draws = @draws, losses = @losses, gamesPlayed = @gamesplayed WHERE username = @username", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@elo", elo);
                    cmd.Parameters.AddWithValue("@wins", wins);
                    cmd.Parameters.AddWithValue("@draws", draws);
                    cmd.Parameters.AddWithValue("@losses", losses);
                    cmd.Parameters.AddWithValue("@gamesPlayed", gamesplayed);

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        ///     Gets the amount of coins the user has
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>Amount of coins the user has</returns>
        public int GetUsersCoins(string username)
        {
            int coins;

            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT coins FROM users WHERE username = @username", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    coins = (int)cmd.ExecuteScalar();
                }
                connection.Close();
            }

            return coins;
        }
        
        /// <summary>
        ///     Updates the amount of coins the user has
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="newCoins">New amount of coins the user has</param>
        public void UpdateUsersCoins(string username, int newCoins)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("UPDATE users SET coins = @newcoins WHERE username = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@newcoins", newCoins);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        /// <summary>
        ///     Checks, if the given card belongs to the given user
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="cardID">Id of card</param>
        /// <returns>If the given card belongs to the given user</returns>
        public bool CheckUserOwnsCard(string username, string cardID)
        {
            bool owns = false;
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT 1 FROM cards WHERE cardID = @cardID AND userID = (SELECT userID FROM users WHERE username = @username) LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@cardID", cardID);

                    owns = cmd.ExecuteScalar() != null;
                }
                connection.Close();
            }

            return owns;
        }

        /// <summary>
        ///     Gets all cards that belong to the given user
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>List of card's from user</returns>
        public List<Card?> GetUsersCards(string username)
        {
            List<Card?> cards = new List<Card?>();
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM cards WHERE userID = (SELECT userID FROM users WHERE username = @username)", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string? name = reader["Name"].ToString();
                            string? elementString = reader["Element"].ToString();
                            int damage = Convert.ToInt32(reader["Damage"]);
                            string? type = reader["Type"].ToString();

                            if (type == CardTypes.Spell.ToString())
                                cards.Add(GetCardFromReaderAttributes(elementString, name, damage, type, null));
                            else if (type == CardTypes.Monster.ToString())
                            {
                                string? monsterTypeString = reader["Monstertype"].ToString();
                                cards.Add(GetCardFromReaderAttributes(elementString, name, damage, type, monsterTypeString));
                            }
                        }
                    }
                }
                connection.Close();
            }

            return cards;
        }

        /// <summary>
        ///     Gets the deck of the given user that that is saved in the database
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>Saved deck of user</returns>
        public List<Card?> GetUsersDeck(string username)
        {
            List<Card?> deck = new List<Card?>();
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Get data from every card in deck
                for (int i = 1; i <= 4; i++)
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT c.* FROM cards c " +
                        $"JOIN decks d ON c.cardID = d.card{i}ID " +
                        "WHERE d.userID = (SELECT userID FROM users WHERE username = @username)", connection))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string? elementString = reader["Element"].ToString();
                                string? name = reader["Name"].ToString();
                                int damage = Convert.ToInt32(reader["Damage"]);
                                string? type = reader["Type"].ToString();
                                string? monsterTypeString = reader["Monstertype"].ToString();

                                if (type == CardTypes.Spell.ToString())
                                    deck.Add(GetCardFromReaderAttributes(elementString, name, damage, type, null));
                                else if (type == CardTypes.Monster.ToString())
                                    deck.Add(GetCardFromReaderAttributes(elementString, name, damage, type, monsterTypeString));
                            }
                        }
                    }
                }
                connection.Close();
            }

            return deck;
        }

        /// <summary>
        ///     Updates the deck of the given user
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="cardIDs">List with IDs of cards to update deck</param>
        public void UpdateUsersDeck(string username, List<string> cardIDs)
        {
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO decks (userID, card1ID, card2ID, card3ID, card4ID) " +
                "VALUES ((SELECT userID FROM users WHERE username = @username), @card1ID, @card2ID, @card3ID, @card4ID)", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    for (int i = 0; i < cardIDs.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@card{i + 1}ID", cardIDs[i]);
                    }

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        ///     Creates a card object based on the attributes from the reader
        /// </summary>
        /// <param name="elementString">Name of card's element</param>
        /// <param name="name">Name of the card</param>
        /// <param name="damage">Damage of the card</param>
        /// <param name="type">Type of card</param>
        /// <param name="monsterTypeString">Monster type of card (in case card is a monster)</param>
        /// <returns>Card object with the given attributes</returns>
        private Card? GetCardFromReaderAttributes(string? elementString, string? name, int damage, string? type, string? monsterTypeString)
        {
            Card card = null;
            // Get element
            ElementType element = new ElementType();

            if (elementString == ElementType.Normal.ToString())
                element = ElementType.Normal;
            else if (elementString == ElementType.Water.ToString())
                element = ElementType.Water;
            else if (elementString == ElementType.Fire.ToString())
                element = ElementType.Fire;

            // Create card object
            if (type == CardTypes.Spell.ToString())
            {
                card = new Spell(name ?? "", element, damage);
            }
            else if (type == CardTypes.Monster.ToString())
            {
                // Get monstertype
                MonsterType monsterType = new MonsterType();

                if (monsterTypeString == MonsterType.Goblin.ToString())
                    monsterType = MonsterType.Goblin;
                else if (monsterTypeString == MonsterType.Dragon.ToString())
                    monsterType = MonsterType.Dragon;
                else if (monsterTypeString == MonsterType.Wizzard.ToString())
                    monsterType = MonsterType.Wizzard;
                else if (monsterTypeString == MonsterType.Ork.ToString())
                    monsterType = MonsterType.Ork;
                else if (monsterTypeString == MonsterType.Knight.ToString())
                    monsterType = MonsterType.Knight;
                else if (monsterTypeString == MonsterType.Kraken.ToString())
                    monsterType = MonsterType.Kraken;
                else if (monsterTypeString == MonsterType.FireElve.ToString())
                    monsterType = MonsterType.FireElve;

                card = new Monster(name ?? "", monsterType, element, damage);
            }

            return card;
        }

        /// <summary>
        ///     Checks, if a card with the given id exists
        /// </summary>
        /// <param name="cardID">ID of the card</param>
        /// <returns>If card exists</returns>
        public bool CheckCardExists(string cardID)
        {
            bool exists = false;
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT 1 FROM cards WHERE cardID = @cardID LIMIT 1", connection))
                {
                    cmd.Parameters.AddWithValue("@cardID", cardID);
                    exists = cmd.ExecuteScalar() != null;
                }
                connection.Close();
            }

            return exists;
        }

        /// <summary>
        ///     Creates and adds a card of type monster to the database
        /// </summary>
        /// <param name="cardID">Id of card</param>
        /// <param name="name">Name of card</param>
        /// <param name="element">Element of card</param>
        /// <param name="damage">Damage of card</param>
        /// <param name="monstertype">Type of monster</param>
        public void CreateMonsterCard(string cardID, string name, string element, int damage, string monstertype)
        {
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO cards (cardID, name, element, damage, type, monstertype) " +
                "VALUES (@cardID, @name, @element, @damage, @type, @monstertype)", connection))
                {
                    cmd.Parameters.AddWithValue("@cardID", cardID);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@element", element);
                    cmd.Parameters.AddWithValue("@damage", damage);
                    cmd.Parameters.AddWithValue("@type", CardTypes.Monster.ToString());
                    cmd.Parameters.AddWithValue("@monstertype", monstertype);

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        ///     Creates and adds a card of type spell to the database
        /// </summary>
        /// <param name="cardID">Id of card</param>
        /// <param name="name">Name of card</param>
        /// <param name="element">Element of card</param>
        /// <param name="damage">Damage of card</param>
        public void CreateSpellCard(string cardID, string name, string element, int damage)
        {
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO cards (cardID, name, element, damage, type) " +
                "VALUES (@cardID, @name, @element, @damage, @type)", connection))
                {
                    cmd.Parameters.AddWithValue("@cardID", cardID);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@element", element);
                    cmd.Parameters.AddWithValue("@damage", damage);
                    cmd.Parameters.AddWithValue("@type", CardTypes.Spell.ToString());

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        ///     Creates a package with the given list of cards
        /// </summary>
        /// <param name="cardIDs">List with ids of cards</param>
        public void CreatePackage(List<string> cardIDs)
        {
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO packages (card1ID, card2ID, card3ID, card4ID, card5ID) " +
                "VALUES (@card1ID, @card2ID, @card3ID, @card4ID, @card5ID)", connection))
                {
                    for (int i = 0; i < cardIDs.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@card{i + 1}ID", cardIDs[i]);
                    }

                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        ///     Checks, if there is a package in the database
        /// </summary>
        /// <returns>If a package is available</returns>
        public bool CheckPackagesAvailable()
        {
            bool exists = false;
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT 1 FROM packages LIMIT 1", connection))
                {
                    exists = cmd.ExecuteScalar() != null;
                }
                connection.Close();
            }

            return exists;
        }

        /// <summary>
        ///     User buys a package, acquires the cards and deletes the package from the database
        /// </summary>
        /// <param name="username">Username of user</param>
        public void BuyPackage(string username)
        {
            // Get the latest available pack
            int packID = GetFirstAvailablePackID();
            List<string> cardIDs = GetCardsFromPackage(packID);

            // Update user's coins
            int newCoins = GetUsersCoins(username) - 5;
            UpdateUsersCoins(username, newCoins);

            // Add user's ID to all cards in the purchased pack
            AddUserToPackCards(username, cardIDs);

            // Delete the purchased pack
            DeletePackage(packID);
        }

        /// <summary>
        ///     Gets the first available package to purchase
        /// </summary>
        /// <returns>The id of an available package</returns>
        public int GetFirstAvailablePackID()
        {
            int packID;
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT packID FROM packages LIMIT 1", connection))
                {
                    packID = (int)cmd.ExecuteScalar();
                }

                connection.Close();
            }

            return packID;
        }

        /// <summary>
        ///     Gets the cards in a package
        /// </summary>
        /// <param name="packID">Id of package</param>
        /// <returns>List with cards from the given package</returns>
        public List<string> GetCardsFromPackage(int packID)
        {
            List<string> cardIDs = new List<string>();
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM packages WHERE packID = @packID", connection))
                {
                    cmd.Parameters.AddWithValue($"@packID", packID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cardIDs.Add(reader["card1ID"].ToString() ?? "");
                            cardIDs.Add(reader["card2ID"].ToString() ?? "");
                            cardIDs.Add(reader["card3ID"].ToString() ?? "");
                            cardIDs.Add(reader["card4ID"].ToString() ?? "");
                            cardIDs.Add(reader["card5ID"].ToString() ?? "");
                        }
                    }
                }
                connection.Close();
            }

            return cardIDs;
        }

        /// <summary>
        ///     Sets the user as the owner of the acquired cards
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="cardIDs">List with IDs of cards</param>
        public void AddUserToPackCards(string username, List<string> cardIDs)
        {
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                foreach (string id in cardIDs)
                {
                    using (NpgsqlCommand cmd = new NpgsqlCommand("UPDATE cards SET userID = (SELECT userID FROM users WHERE username = @username)" +
                        "WHERE cardID = @cardID", connection))
                    {
                        cmd.Parameters.AddWithValue($"@username", username);
                        cmd.Parameters.AddWithValue($"@cardID", id);

                        cmd.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }

        /// <summary>
        ///     Deletes the purchased package
        /// </summary>
        /// <param name="packID">Id of package</param>
        public void DeletePackage(int packID)
        {
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM packages WHERE packID = @packID", connection))
                {
                    cmd.Parameters.AddWithValue($"@packID", packID);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        /// <summary>
        ///     Gets the scoreboard sorted by the elo
        /// </summary>
        /// <returns>List of users sorted by their elo</returns>
        public List<User> GetScoreboard()
        {
            List<User> scoreboard = new List<User>();
            using (connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT username, elo, wins, draws, losses, gamesplayed FROM users ORDER BY elo DESC", connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                Username = reader["username"].ToString() ?? "",
                                Elo = (int)reader["elo"],
                                Wins = (int)reader["wins"],
                                Draws = (int)reader["draws"],
                                Losses = (int)reader["losses"],
                                GamesPlayed = (int)reader["gamesplayed"]
                            };
                            scoreboard.Add(user);
                        }
                    }
                }
                connection.Close();
            }

            return scoreboard;
        }

        /// <summary>
        ///     Checks if given user can spin on the daily wheel
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>If user can spin on wheel</returns>
        public bool CheckCanSpinWheel(string username)
        {
            bool canSpin = true;

            // Check if user already spinned today
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT spindate FROM users WHERE username = @username", connection))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    object? lastSpinDateObject = cmd.ExecuteScalar();
                    if (lastSpinDateObject != DBNull.Value)
                    {
                        DateTime currentDate = DateTime.Now;
                        DateTime lastSpinDate = (DateTime)lastSpinDateObject;

                        canSpin = (currentDate - lastSpinDate).TotalDays >= 1;
                    }
                }
                connection.Close();
            }

            return canSpin;
        }

        /// <summary>
        ///     Spins the daily wheel
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <returns>Amount of coins won</returns>
        public int SpinWheel(string username)
        {
            // Get random integer between 1 and 10
            int coinsWon = new Random().Next(1, 11);

            // Add coins won to user's coins
            AwardCoins(username, coinsWon);

            // Update date of wheel spin
            UpdateWheelSpinDate(username);

            return coinsWon;
        }

        /// <summary>
        ///     Adds coins to user
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="coins">Coins to add</param>
        public void AwardCoins(string username, int coinsWon)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("UPDATE users SET coins = coins + @coinswon WHERE username = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@coinswon", coinsWon);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        /// <summary>
        ///     Updates the date when user spinned the wheel
        /// </summary>
        /// <param name="username">Username of user</param>
        public void UpdateWheelSpinDate(string username)
        {
            DateTime currentDate = DateTime.Now;

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (NpgsqlCommand command = new NpgsqlCommand("UPDATE users SET spindate = @currentdate WHERE username = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@currentdate", currentDate);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
