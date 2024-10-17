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
        private readonly Dictionary<string, Dictionary<string, Dictionary<int,string>>> statusCodeMessages;

        public HttpResponse()
        {
            statusCodeMessages = new Dictionary<string, Dictionary<string, Dictionary<int, string>>>
            {
                { "POST", new Dictionary<string, Dictionary<int, string>>
                    {
                        { "/users", new Dictionary<int, string>
                            {
                                { 201, "User successfully created" },
                                { 409, "User with same username already registered" },
                            }
                        },
                        { "/sessions", new Dictionary<int, string>
                            {
                                { 200, "User login successful" },
                                { 401, "Invalid username/password provided" },
                            }
                        },
                        // other endpoints
                    }
                },
            }; // end Dict
        }

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
