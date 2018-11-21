namespace ScibuAPIConnector.Models
{
    public class Token
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public string FullName { get; set; }
        public string ClientId { get; set; }
        public string IssuedDate { get; set; }
        public string ExpiresDate { get; set; }

        public Token(string accessToken, string tokenType, int expiresIn, string refreshToken, string fullName, string clientId, string issuedDate, string expiresDate)
        {
            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
            FullName = fullName;
            ClientId = clientId;
            IssuedDate = issuedDate;
            ExpiresDate = expiresDate;
        }
    }
}
