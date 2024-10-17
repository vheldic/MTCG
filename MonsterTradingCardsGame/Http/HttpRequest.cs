using MonsterTradingCardsGame.GameClasses;
using MonsterTradingCardsGame.Database;
using System.Text.Json;

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

        private async Task HandleRequest()
        {
            HttpResponse httpResponse = new HttpResponse();

            // Get each Line from Request
            string[] requestLines = Request.Split("\n");

            // Get HTTP-Method and endpoint from first line of Request
            string[] requestFirstLineParts = requestLines[0].Split(" ");
            string httpMethod = requestFirstLineParts[0];
            string endpoint = requestFirstLineParts[1];

            // Get data from Request-body
            string jsonBody = requestLines.LastOrDefault()?.Trim() ?? string.Empty;

            var userObject = JsonSerializer.Deserialize<User>(jsonBody);
            string username = userObject.Username;
            string password = userObject.Password;

            Response = $"{HTTP_RESPONSE_HEADER} 501 Not implemented yet";   // Temp
            switch (httpMethod)
            {
                case "GET":
                    switch (endpoint)
                    {
                        case "/cards":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        case "/deck":
                            Response += $" - {httpMethod} {endpoint}";
                            break;
                        
                        case "/deck?format=plain":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        case "/stats":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        case "/scoreboard":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        case "/tradings":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        default:
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
                        case "/users":
                            // Check if user already exists
                            if (Database.Users.ContainsKey(username))
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 409);
                            else
                            {
                                // Register
                                Database.Users.Add(username, new User(username, password));
                                // Generate Token
                                string token = GenerateToken(username);
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 201);
                            }
                            break;

                        case "/sessions":
                            // Check user credentials
                            if (Database.Users[username].Password != password)
                                Response = httpResponse.GetResponseMessage(httpMethod, endpoint, 401);
                            else
                            {
                                // Login
                                // Generate Token
                                string token = GenerateToken(username);
                                Response = $"{httpResponse.GetResponseMessage(httpMethod, endpoint, 200)}\r\n{token}";
                            }
                            break;

                        case "/packages":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        case "/transactions/packages":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        case "/battles":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        case "/tradings":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        default:
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
                        case "/deck":
                            Response += $" - {httpMethod} {endpoint}";
                            break;

                        default:
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
        }

        private string GenerateToken(string username)
        {
            return $"{username}-mtcgToken";
        }
    }
}
