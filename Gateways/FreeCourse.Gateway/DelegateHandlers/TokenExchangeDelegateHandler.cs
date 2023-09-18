using IdentityModel.Client;

namespace FreeCourse.Gateway.DelegateHandlers;

public class TokenExchangeDelegateHandler : DelegatingHandler
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string _token;


    public TokenExchangeDelegateHandler(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestToken = request.Headers.Authorization.Parameter;
        
        var token = await GetToken(requestToken);
        
        request.SetBearerToken(token);
        
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetToken(string requestToken)
    {
        if (!string.IsNullOrEmpty(_token))
        {
            return _token;
        }

        var discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _configuration["IdentityServerURL"],
            Policy = new DiscoveryPolicy
            {
                RequireHttps = false
            }
        });

        if (discoveryDocument.IsError)
            throw discoveryDocument.Exception!;

        TokenExchangeTokenRequest tokenExchangeTokenRequest = new()
        {
            Address = discoveryDocument.TokenEndpoint,
            ClientId = _configuration["ClientId"],
            ClientSecret = _configuration["ClientSecret"],
            GrantType = _configuration["TokenGrantType"],
            SubjectToken = requestToken,
            SubjectTokenType = "urn:ietf:params:oauth:token-type:access_token",
            Scope = "openid discount_fullpermission payment_fullpermission"

        };
        
        var tokenResponse = await _httpClient.RequestTokenExchangeTokenAsync(tokenExchangeTokenRequest);
        if(tokenResponse.IsError)
            throw tokenResponse.Exception!;
        
        _token = tokenResponse.AccessToken!;

        return _token;
    }
}