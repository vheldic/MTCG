namespace MonsterTradingCardsGame.Http
{
    /**
     * Feedback:
     * Ansich passen die Directories, vielleicht besser Klasse damits flacher wird
     * Beispiel: Klasse GetStatusCodeMessage, PushStatusCodesMessage
     */
    public class HttpResponse
    {
        const string HTTP_RESPONSE_HEADER = "HTTP/1.1";
        const string STATUSCODE_401_MESSAGE = "Access token is missing or invalid";
        private readonly Dictionary<string, Dictionary<string, Dictionary<int,string>>> statusCodeMessages;

        public HttpResponse()
        {
            statusCodeMessages = new Dictionary<string, Dictionary<string, Dictionary<int, string>>>
            {
                { "GET", new Dictionary<string, Dictionary<int, string>>
                    {
                        // Retrieves the user data for the given username
                        { "/users/{username}", new Dictionary<int, string>
                            {
                                { 200, "Data successfully retrieved" },
                                { 401, STATUSCODE_401_MESSAGE },
                                { 404, "User not found" },
                            }
                        },
                        // Shows a user's cards
                        { "/cards", new Dictionary<int, string>
                            {
                                { 200, "The user has cards, the response contains these" },
                                { 204, "The request was fine, but the user doesn't have any cards" },
                                { 401, STATUSCODE_401_MESSAGE },
                            }
                        },
                        // Shows the user's currently configured deck
                        { "/deck", new Dictionary<int, string>
                            {
                                { 200, "The deck has cards, the response contains these" },
                                { 204, "The request was fine, but the deck doesn't have any cards" },
                                { 401, STATUSCODE_401_MESSAGE },
                            }
                        },
                        // Retrieves the stats for an individual user
                        { "/stats", new Dictionary<int, string>
                            {
                                { 200, "The stats could be retrieved successfully" },
                                { 401, STATUSCODE_401_MESSAGE },
                            }
                        },
                        // Retrieves the user scoreboard ordered by the user's ELO
                        { "/scoreboard", new Dictionary<int, string>
                            {
                                { 200, "The scoreboard could be retrieved successfully" },
                                { 401, STATUSCODE_401_MESSAGE },
                            }
                        },
                        // Retrieves the currently available trading deals
                        { "/tradings", new Dictionary<int, string>
                            {
                                { 200, "There are trading deals available, the response contains these" },
                                { 204, "The request was fine, but there are no trading deals available" },
                                { 401, STATUSCODE_401_MESSAGE },
                            }
                        },
                        // other endpoints
                    }
                },
                { "POST", new Dictionary<string, Dictionary<int, string>>
                    {
                        // Register a new user
                        { "/users", new Dictionary<int, string>
                            {
                                { 201, "User successfully created" },
                                { 409, "User with same username already registered" },
                            }
                        },
                        // Login with existing user
                        { "/sessions", new Dictionary<int, string>
                            {
                                { 200, "User login successful" },
                                { 401, "Invalid username/password provided" },
                            }
                        },
                        // Create new card packages (requires admin)
                        { "/packages", new Dictionary<int, string>
                            {
                                { 201, "Package and cards successfully created" },
                                { 401, STATUSCODE_401_MESSAGE },
                                { 403, "Provided user is not \"admin\"" },
                                { 409, "At least one card in the packages already exists" },
                            }
                        },
                        // Acquire a card package
                        { "/transactions/packages", new Dictionary<int, string>
                            {
                                { 200, "A package has been successfully bought" },
                                { 401, STATUSCODE_401_MESSAGE },
                                { 403, "Not enough money for buying a card package" },
                                { 404, "No card package available for buying" },
                            }
                        },
                        // Enters the lobby to start a battle
                        { "/battles", new Dictionary<int, string>
                            {
                                { 200, "The battle has been carried out successfully" },
                                { 401, STATUSCODE_401_MESSAGE },
                            }
                        },
                        // Creates a new trading deal
                        { "/tradings", new Dictionary<int, string>
                            {
                                { 201, "Trading deal successfully created" },
                                { 401, STATUSCODE_401_MESSAGE },
                                { 403, "The deal contains a card that is not owned by the user or locked in the deck" },
                                { 409, "A deal with this deal ID already exists" },
                            }
                        },
                        // Carry out a trade for the deal with the provided card
                        { "/tradings/{tradingdealid}", new Dictionary<int, string>
                            {
                                { 200, "Trading deal successfully executed" },
                                { 401, STATUSCODE_401_MESSAGE },
                                { 403, "The offered card is not owned by the user, or the requirements are not met (Type, MinimumDamage), or the offered card is locked in the deck, or the user tries to trade with self" },
                                { 404, "The provided deal ID was not found" },
                            }
                        },
                        // other endpoints
                    }
                },
                { "PUT", new Dictionary<string, Dictionary<int, string>>
                    {
                        // Updates the user data for the given username
                        { "/users/{username}", new Dictionary<int, string>
                            {
                                { 200, "User sucessfully updated" },
                                { 401, STATUSCODE_401_MESSAGE },
                                { 404, "User not found" },
                            }
                        },
                        // Configures the deck with four provided cards
                        { "/deck", new Dictionary<int, string>
                            {
                                { 200, "The deck has been successfully configured" },
                                { 400, "The provided deck did not include the required amount of cards" },
                                { 401, STATUSCODE_401_MESSAGE },
                                { 403, "At least one of the provided cards does not belong to the user or is not available" },
                            }
                        },
                        // Spinns the daily wheel for coins
                        { "/coins", new Dictionary<int, string>
                            {
                                { 200, "Coins have been added successfully" },
                                { 401, STATUSCODE_401_MESSAGE },
                                { 422, "Daily wheel was already used today" },
                            }
                        },
                        // other endpoints
                    }
                },
                { "DELETE", new Dictionary<string, Dictionary<int, string>>
                    {
                        // Deletes an existing trading deal
                        { "/tradings/{tradingdealid}", new Dictionary<int, string>
                            {
                                { 200, "Trading deal successfully deleted" },
                                { 401, STATUSCODE_401_MESSAGE },
                                { 403, "The deal contains a card that is not owned by the user" },
                                { 404, "The provided deal ID was not found" },
                            }
                        },
                        // other endpoints
                    }
                }
            }; // end Dict
        }

        /// <summary>
        ///     Returns from the dictionary a the response for a specific request based on the HTTP-Method, endpoint and statuscode
        /// </summary>
        /// <param name="httpMethod">HTTP-Method from request</param>
        /// <param name="endpoint">Endpoint from request</param>
        /// <param name="statusCode">Statuscode to fetch</param>
        /// <returns>Response-Message</returns>
        public string GetResponseMessage(string httpMethod, string endpoint, int statusCode)
        {
            string response = $"{HTTP_RESPONSE_HEADER} ";

            if (!statusCodeMessages.ContainsKey(httpMethod))
                return response += "500 Unknown HTTP Method";
            if (!statusCodeMessages[httpMethod].ContainsKey(endpoint))
                return response += "500 Unknown endpoint";
            if (!statusCodeMessages[httpMethod][endpoint].ContainsKey(statusCode))
                return response += "500 Unknown statuscode";

            return response += $"{statusCode} {statusCodeMessages[httpMethod][endpoint][statusCode]}";
        }
    }
}
