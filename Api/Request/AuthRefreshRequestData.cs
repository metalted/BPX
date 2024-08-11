namespace BPX.Api.Request;

public class AuthRefreshRequestData
{
    public ulong SteamId { get; set; }
    public string LoginToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}