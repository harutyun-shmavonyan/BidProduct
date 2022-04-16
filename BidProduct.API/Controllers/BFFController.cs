using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BidProduct.API.Controllers;

[ApiController]
[Route("[controller]")]
public class BFFController : ControllerBase
{
    private const string AccessTokenName = "access_token";
    private const string RefreshTokenName = "refresh_token";

    private readonly HttpClient _client;
    private readonly string _baseAddress;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;

    public BFFController(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        var configuration1 = configuration;

        _baseAddress = configuration1["IdentityServer:BaseUrl"];
        _clientId = configuration1["IdentityServer:ClientId"];
        _clientSecret = configuration1["IdentityServer:ClientSecret"];
        _redirectUri = "https://localhost:7143/bff/exchange-token";
    }

    [HttpGet]
    public IActionResult GetToken()
    {
        var url = $@"{_baseAddress}/connect/authorize?client_id={_clientId}&client_secret={_clientSecret}&grant_type=authorization_code&response_type=code&scope=appScope offline_access&redirect_uri={_redirectUri}";

        return Redirect(url);
    }

    [HttpGet("sign-out")]
    public new async Task<IActionResult> SignOut()
    {
        var url = $"{_baseAddress}/connect/revocation";

        var postData = new Dictionary<string, string>()
        {
            {"token", Request.Cookies[AccessTokenName]!},
            {"token_type_hint", AccessTokenName},
            {"client_id", _clientId},
            {"client_secret", _clientSecret},
        };

        using var requestContent = new FormUrlEncodedContent(postData);
        await _client.PostAsync(url, requestContent);

        Response.Cookies.Delete(AccessTokenName);
        Response.Cookies.Delete(RefreshTokenName);

        return Ok();
    }

    [HttpGet("exchange-token")]
    public async Task<IActionResult> SetToken(string code) 
    {
        var url = $"{_baseAddress}/connect/token";

        var postData = new Dictionary<string, string>()
        {
            {"client_id", _clientId},
            {"client_secret", _clientSecret},
            {"code", code},
            {"redirect_uri", _redirectUri},
            {"grant_type", "authorization_code"}
        };

        using var requestContent = new FormUrlEncodedContent(postData);

        requestContent.Headers.Clear();
        requestContent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

        var response = await _client.PostAsync(url, requestContent);
        if (!response.IsSuccessStatusCode)
            return Ok();

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true
        };

        Response.Cookies.Append(AccessTokenName, responseDic![AccessTokenName], cookieOptions);
        Response.Cookies.Append(RefreshTokenName, responseDic[RefreshTokenName], cookieOptions);

        return Ok();
    }
}