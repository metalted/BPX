namespace BPX.Api.Response
{

    public class AuthenticationResponseData
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string AccessTokenExpiry { get; set; }
        public string RefreshTokenExpiry { get; set; }
    }
}
