using MonsterTradingCardsGame.GameClasses;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace MonsterTradingCardsGame.Http
{
    /**
     * Feedback: 
     * StatusCodes die öfter verwendet werden als Konstante machen
     * Switch Blöcke in eigene Methoden packen (damit Methoden kleiner werden) zb GetSwitch, PostSwitch,...
     * Switch Blöcke vielleicht sogar umdrehen (zuerst endpoint checken, dann method)
     */
    public class HttpRequest
    {
        const string HTTP_RESPONSE_HEADER = "HTTP/1.1";
        const string HTTP_METHOD_ERROR = $"{HTTP_RESPONSE_HEADER} 502 False or inproper HTTP Method";
        const string ENDPOINT_ERROR = $"{HTTP_RESPONSE_HEADER} 502 Endpoint does not exist";
        private static readonly object battleResponseLock = new object();
        public string Request { get; private set; }
        public string Response { get; private set; }
        public Database.Database Database { get; private set; }
        public TcpListener Listener { get; private set; }

        public HttpRequest(TcpListener listener, string request, Database.Database db)
        {
            Listener = listener;
            Request = request;
            Response = "";
            Database = db;
            HandleRequest();
        }

        /// <summary>
        ///     Handles the request based on the HTTP-Method and endpoint.
        /// </summary>
        private async void HandleRequest()
        {
            HttpResponse httpResponse = new HttpResponse();

            // Get each Line from Request
            string[] requestLines = Request.Split("\n");

            // Get HTTP-Method and endpoint from first line of Request
            string[] requestFirstLineParts = requestLines[0].Split(" ");
            string httpMethod = requestFirstLineParts[0];
            string endpoint = requestFirstLineParts[1];

            // Get Bearer Token from Request
            string bearer_token = GetTokenFromRequest(Request);

            // Get data from Request-body
            string jsonBody = requestLines.LastOrDefault()?.Trim() ?? string.Empty;

            Response = $"{HTTP_RESPONSE_HEADER} 501 Not implemented yet";   // Temp
            switch (httpMethod)
            {
                case "GET":
                    switch (endpoint)
                    {
                        // Shows a user's cards
                        case "/cards":
                            // Check if token is used and valid
                            if (!Database.CheckTokenIsValid(bearer_token))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                break;
                            }
                            string username = Database.GetUsernameFromToken(bearer_token);

                            // Get user's cards
                            List<Card?> cards = Database.GetUsersCards(username);
                            if (cards.Count == 0)
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 204);
                                break;
                            }

                            Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200) + "\r\nX-Description: " + JsonSerializer.Serialize(cards);
                            break;

                        // Shows the user's currently configured deck
                        case "/deck":
                            // Check if token is used and valid
                            if (!Database.CheckTokenIsValid(bearer_token))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                break;
                            }
                            username = Database.GetUsernameFromToken(bearer_token);

                            // Get user's deck
                            List<Card?> deck = Database.GetUsersDeck(username);
                            if (deck.Count == 0)
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 204);
                                break;
                            }

                            Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200) + "\r\nX-Description: " + JsonSerializer.Serialize(deck);
                            break;

                        // Shows the user's currently configured deck
                        case "/deck?format=plain":
                            endpoint = "/deck";
                            // Check if token is used and valid
                            if (!Database.CheckTokenIsValid(bearer_token))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                break;
                            }
                            username = Database.GetUsernameFromToken(bearer_token);

                            // Get user's deck
                            deck = Database.GetUsersDeck(username);
                            if (deck.Count == 0)
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 204);
                                break;
                            }

                            Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200) + "\r\n" + CardsListToString(deck);
                            break;

                        // Retrieves the stats for an individual user
                        case "/stats":
                            // Check if token is used and valid
                            if (!Database.CheckTokenIsValid(bearer_token))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                break;
                            }
                            username = Database.GetUsernameFromToken(bearer_token);

                            // Get user data and create json object
                            User? user = Database.GetUser(username);
                            var userstats = new
                            {
                                Elo = user?.Elo,
                                Wins = user?.Wins,
                                Draws = user?.Draws,
                                Losses = user?.Losses,
                                GamesPlayed = user?.GamesPlayed
                            };

                            Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200) + "\r\nX-Description: " + JsonSerializer.Serialize(userstats);
                            break;

                        // Retrieves the user scoreboard ordered by the user's ELO
                        case "/scoreboard":
                            // Check if token is used and valid
                            if (!Database.CheckTokenIsValid(bearer_token))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                break;
                            }

                            // Get scoreboard and create json Array
                            List<User> users = Database.GetScoreboard();
                            JsonArray jsonArray = new JsonArray();
                            foreach (User userObject in users)
                            {
                                var userdata = new
                                {
                                    Username = userObject?.Username ?? "",
                                    Elo = userObject?.Elo,
                                    Wins = userObject?.Wins,
                                    Draws = userObject?.Draws,
                                    Losses = userObject?.Losses,
                                    GamesPlayed = userObject?.GamesPlayed,
                                };
                                jsonArray.Add(userdata);
                            }

                            Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200) + "\r\nX-Description: " + JsonSerializer.Serialize(jsonArray);
                            break;

                        // Retrieves the currently available trading deals
                        case "/tradings":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        default:
                            // Retrieves the user data for the given username
                            if (endpoint.StartsWith("/users/"))
                            {
                                string userToGet = endpoint.Split("/")[2];
                                endpoint = "/users/{username}";

                                // Check if User exists
                                if (!Database.CheckUserExists(userToGet))
                                {
                                    Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 404);
                                    break;
                                }

                                // Check if token is used and valid
                                if (!Database.CheckTokenIsValid(bearer_token) || userToGet != Database.GetUsernameFromToken(bearer_token))
                                {
                                    Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                    break;
                                }

                                // Get specific User
                                user = Database.GetUser(userToGet);
                                var userdata = new
                                {
                                    Name = user?.Name,
                                    Bio = user?.Bio,
                                    Image = user?.Image
                                };

                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200) + "\r\nX-Description: " + JsonSerializer.Serialize(userdata);
                            }
                            else Response = ENDPOINT_ERROR;
                            break;
                    }
                    break;

                case "POST":
                    switch (endpoint)
                    {
                        // Register a new user
                        case "/users":
                            var userObject = JsonSerializer.Deserialize<User>(jsonBody);
                            string username = userObject?.Username ?? "";
                            string password = userObject?.Password ?? "";

                            // Check if user already exists
                            if (Database.CheckUserExists(username))
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 409);
                            else
                            {
                                // Register
                                Database.RegisterUser(username, password);
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 201);
                            }
                            break;

                        // Login with existing user
                        case "/sessions":
                            userObject = JsonSerializer.Deserialize<User>(jsonBody);
                            username = userObject?.Username ?? "";
                            password = userObject?.Password ?? "";

                            // Check user credentials
                            string token = Database.LoginUser(username, password);
                            if (string.IsNullOrEmpty(token))
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                            else
                            {
                                // Login
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200) + "\r\nToken: " + token;
                            }
                            break;

                        // Create new card packages (requires admin)
                        case "/packages":
                            // Check if token is used and valid
                            if (!Database.CheckTokenIsValid(bearer_token))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                break;
                            }
                            username = Database.GetUsernameFromToken(bearer_token);

                            // Check if user is admin
                            if (!Database.CheckUserIsAdmin(username))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 403);
                                break;
                            }

                            // Get cards from body
                            JsonArray body = [];
                            try
                            {
                                body = JsonNode.Parse(jsonBody) as JsonArray ?? [];
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error: " + e.Message);
                                break;
                            }

                            List<string> cardIDs = new List<string>();
                            foreach (var cardObject in body)
                            {
                                string? cardID = cardObject["Id"]?.GetValue<string>() ?? "";

                                // Check if cards already exist
                                if (Database.CheckCardExists(cardID))
                                {
                                    Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 409);
                                    break;
                                }
                                cardIDs.Add(cardID);

                                // Get card values
                                string name = cardObject["Name"].GetValue<string>();
                                string element = cardObject["Element"].GetValue<string>();
                                double damage = cardObject["Damage"].GetValue<double>();

                                // Create cards
                                if (cardObject["Type"]?.ToString() == CardTypes.Spell.ToString())
                                    Database.CreateSpellCard(cardID, name, element, (int)damage);
                                else
                                {
                                    string monstertype = cardObject["Monstertype"]?.GetValue<string>();
                                    Database.CreateMonsterCard(cardID, name, element, (int)damage, monstertype);
                                }
                            };

                            // Create Package
                            Database.CreatePackage(cardIDs);
                            Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 201);
                            break;

                        // Acquire a card package
                        case "/transactions/packages":
                            // Check if token is used and valid
                            if (!Database.CheckTokenIsValid(bearer_token))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                break;
                            }
                            username = Database.GetUsernameFromToken(bearer_token);

                            // Check if user has enough coins
                            if (Database.GetUsersCoins(username) < 5)
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 403);
                                break;
                            }

                            // Check if there are available packages
                            if (!Database.CheckPackagesAvailable())
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 404);
                                break;
                            }

                            // Buy package
                            Database.BuyPackage(username);
                            Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200);
                            break;

                        // Enters the lobby to start a battle
                        case "/battles":
                            // Check if token is used and valid
                            if (!Database.CheckTokenIsValid(bearer_token))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                break;
                            }
                            username = Database.GetUsernameFromToken(bearer_token);

                            User? user = Database.GetUser(username);
                            Battle battle = new Battle(user, Database);

                            // Lock battle
                            lock (battle)
                            {
                                _ = WaitInLobby(battle);
                                while (string.IsNullOrEmpty(battle.Log)) { }
                            }

                            // Lock response
                            lock (battleResponseLock)
                            {
                                var log = new
                                {
                                    Log = battle.Log,
                                };
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200) + "\r\nX-Description:" + JsonSerializer.Serialize(log) + "\r\n";
                            }
                            break;

                        // Creates a new trading deal
                        case "/tradings":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        default:
                            // Carry out a trade for the deal with the provided card
                            if (endpoint.StartsWith("/tradings/"))
                            {
                                // Post specific trade
                                Response += $" - {httpMethod} starts with {endpoint}";
                            }
                            else Response = ENDPOINT_ERROR;
                            break;
                    }
                    break;

                case "PUT":
                    switch (endpoint)
                    {
                        // Updates the user data for the given username
                        case "/deck":
                            // Check if token is used and valid
                            if (!Database.CheckTokenIsValid(bearer_token))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                break;
                            }
                            string username = Database.GetUsernameFromToken(bearer_token);

                            // Get cards from body
                            JsonArray body = [];
                            try
                            {
                                body = JsonNode.Parse(jsonBody) as JsonArray ?? [];
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error: " + e.Message);
                                break;
                            }

                            // Check if 4 cards have been selected
                            if (body.Count != 4)
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 400);
                                break;
                            }

                            // Get card objects
                            List<string> cardIDs = new List<string>();
                            foreach (string id in body)
                            {
                                if (!Database.CheckUserOwnsCard(username, id)) 
                                    break;
                                
                                cardIDs.Add(id);
                            }

                            // Check if all cards belong to user
                            if (cardIDs.Count != 4)
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 403);
                                break;
                            }

                            // Update deck
                            Database.UpdateUsersDeck(username, cardIDs);
                            Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200);
                            break;

                        // Spins the daily wheel for coins
                        case "/coins":
                            // Check if token is used and valid
                            if (!Database.CheckTokenIsValid(bearer_token))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                break;
                            }
                            username = Database.GetUsernameFromToken(bearer_token);

                            if (!Database.CheckCanSpinWheel(username))
                            {
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 422);
                                break;
                            }

                            int coinsWon = Database.SpinWheel(username);
                            Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200) + $"\r\nX-Description: {coinsWon} Coins won";
                            break;

                        default:
                            // Configures the deck with four provided cards
                            if (endpoint.StartsWith("/users/"))
                            {
                                string userToGet = endpoint.Split("/")[2];
                                endpoint = "/users/{username}";

                                // Check if User exists
                                if (!Database.CheckUserExists(userToGet))
                                {
                                    Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 404);
                                    break;
                                }

                                // Check if token is used and valid
                                username = Database.GetUsernameFromToken(bearer_token);
                                if (!Database.CheckTokenIsValid(bearer_token) || userToGet != username)
                                {
                                    Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                                    break;
                                }

                                // Get userdata from body
                                var userdata = JsonSerializer.Deserialize<User>(jsonBody);
                                string? name = userdata?.Name;
                                string? bio = userdata?.Bio;
                                string? image = userdata?.Image;

                                Database.EditProfile(username, name ?? "", bio ?? "", image ?? "");
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 200);
                            }
                            else Response = ENDPOINT_ERROR;
                            break;
                    }
                    break;

                case "DELETE":
                    // Deletes an existing trading deal
                    if (endpoint.StartsWith("/tradings/"))
                    {
                        // Delete specific trade
                        Response += $" - {httpMethod} Starts with {endpoint}";
                    }
                    else Response = ENDPOINT_ERROR;
                    break;

                default:
                    Response = HTTP_METHOD_ERROR;
                    break;
            }

            Response += "\r\n";
        }

        /// <summary>
        ///     Gets the bearer token from the request header using regex
        /// </summary>
        /// <returns>The token sent in the request header</returns>
        private string GetTokenFromRequest(string request)
        {
            Match match = Regex.Match(request, "Authorization: Bearer ([^\\s]+)");

            if (match.Success)
                return match.Groups[1].Value;
            return "";
        }

        /// <summary>
        ///     Creates output of cards in list as string
        /// </summary>
        /// <param name="cards">Cards in list</param>
        /// <returns>List of cards as string</returns>
        private string CardsListToString(List<Card> cards)
        {
            string cards_output = "";
            for (int i = 0; i < cards.Count; i++)
            {
                cards_output += $"{i + 1}. {cards[i].Name} ({cards[i].ElementType}): {cards[i].Damage} Damage";
                if (i != cards.Count - 1) cards_output += "\r\n";
            }
            return cards_output;
        }

        /// <summary>
        ///     Waits in lobby for another user to start battle
        /// </summary>
        /// <param name="battle">Battle waiting for opponent</param>
        /// <returns></returns>
        private async Task WaitInLobby(Battle battle)
        {
            Console.WriteLine("Waiting for the next client on port 10001...");

            try
            {
                TcpClient secondClient = await Listener.AcceptTcpClientAsync();

                Console.WriteLine($"Second client connected: {secondClient.Client.RemoteEndPoint}");

                // Second client's network stream
                using (NetworkStream networkStream = secondClient.GetStream())
                {
                    try
                    {
                        var requestBytes = new byte[1024];
                        await networkStream.ReadAsync(requestBytes, 0, requestBytes.Length);
                        string request = Encoding.UTF8.GetString(requestBytes);
                        Console.WriteLine($"Received request from {secondClient.Client.RemoteEndPoint}:\r\n{request}\r\n");

                        string bearer_token2 = GetTokenFromRequest(request);
                        string username2 = Database.GetUsernameFromToken(bearer_token2);
                        string localResponse;
                        if (!Database.CheckTokenIsValid(bearer_token2))
                        {
                            localResponse = new HttpResponse().GetResponseMessage("POST", "/battles", 401);
                        }
                        else
                        {
                            User? user = Database.GetUser(username2);
                            battle.AddPlayer(user);
                            battle.StartBattle();
                            var log = new
                            {
                                Log = battle.Log,
                            };
                            localResponse = new HttpResponse().GetResponseMessage("POST", "/battles", 200) + "\r\nX-Description:" + JsonSerializer.Serialize(log);
                        }

                        Console.WriteLine($"Sending response:\r\n{localResponse}");

                        byte[] responseData = Encoding.UTF8.GetBytes(localResponse);
                        await networkStream.WriteAsync(responseData, 0, responseData.Length);
                        //await networkStream.FlushAsync();
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        // Close second client connection
                        Console.WriteLine($"Client disconnected: {secondClient.Client.RemoteEndPoint}\r\n");
                        secondClient.Close();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("No connection available.");
            }
        }
    }
}
