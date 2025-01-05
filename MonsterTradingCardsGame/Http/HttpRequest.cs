using MonsterTradingCardsGame.GameClasses;
using System.Text.Json;
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
        public string Request { get; private set; }
        public string Response { get; private set; }
        public Database.Database Database { get; private set; }

        public HttpRequest(string request, Database.Database db)
        {
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
            string bearer_token = GetTokenFromRequest();

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
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        // Shows the user's currently configured deck
                        case "/deck":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        // Shows the user's currently configured deck
                        case "/deck?format=plain":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        // Retrieves the stats for an individual user
                        case "/stats":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        // Retrieves the user scoreboard ordered by the user's ELO
                        case "/scoreboard":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        // Retrieves the currently available trading deals
                        case "/tradings":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        default:
                            // Retrieves the user data for the given username
                            if (endpoint.StartsWith("/users/"))
                            {
                                // Get specific User
                                Response += $" - {httpMethod} starts with {endpoint}";
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
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        // Acquire a card package
                        case "/transactions/packages":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        // Enters the lobby to start a battle
                        case "/battles":
                            Response += $" - {httpMethod} {endpoint}";
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
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        default:
                            // Configures the deck with four provided cards
                            if (endpoint.StartsWith("/users/"))
                            {
                                // Edit specific User
                                Response += $" - {httpMethod} starts with {endpoint}";
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
        private string GetTokenFromRequest()
        {
            Match match = Regex.Match(Request, "Authorization: Bearer ([^\\s]+)");

            if (match.Success)
                return match.Groups[1].Value;
            return "";
        }
    }
}
