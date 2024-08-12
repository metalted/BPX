namespace BPX.Api.Request
{

    public class AuthLoginRequestData
    {
        public ulong SteamId { get; set; }
        public string SteamName { get; set; } = null!;
        public string AuthenticationTicket { get; set; } = null!;
    }
}