namespace FosterFlow.Web.Services;

public class TokenStorage
{
    private string? _accessToken;

    public string? GetAccessToken()
    {
        return _accessToken;
    }
    public void SetAccessToken(string tok)
    {
        _accessToken = tok;
    }
    public void ClearAccessToken()
    {
        _accessToken = null;
    }
}
