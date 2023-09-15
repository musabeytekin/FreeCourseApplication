using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
using FreeCourse.Shared.DTOs;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace FreeCourse.Web.Services;

public class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ClientSettings _clientSettings;
    private readonly ServiceApiSettings _serviceApiSettings;

    public IdentityService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor,
        IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _clientSettings = clientSettings.Value;
        _serviceApiSettings = serviceApiSettings.Value;
    }


    public async Task<Response<bool>> SignIn(SignInInput signInInput)
    {
        var discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false }
        });

        if (discoveryDocument.IsError)
            throw discoveryDocument.Exception!;

        var passwordTokenRequest = new PasswordTokenRequest
        {
            ClientId = _clientSettings.WebClientForUser.ClientId,
            ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
            UserName = signInInput.Email,
            Password = signInInput.Password,
            Address = discoveryDocument.TokenEndpoint
        };

        var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

        if (token.IsError)
        {
            var responseContent = await token.HttpResponse.Content.ReadAsStringAsync();
            var errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return Response<bool>.Fail(errorDto!.Errors, 400);
        }

        var userInfoRequest = new UserInfoRequest
        {
            Token = token.AccessToken,
            Address = discoveryDocument.UserInfoEndpoint
        };

        var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);

        if (userInfo.IsError)
            throw userInfo.Exception!;

        var claimsIdentity = new ClaimsIdentity(userInfo.Claims,
            CookieAuthenticationDefaults.AuthenticationScheme, "name",
            "role"); // kullanıcı adı name claiminden okunacak rollerde role claiminden okunacak

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var authenticationProperties = new AuthenticationProperties();

        authenticationProperties.StoreTokens(new List<AuthenticationToken>()
        {
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = token.AccessToken!
            },
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = token.RefreshToken!
            },
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.ExpiresIn,
                Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o", CultureInfo.InvariantCulture)
            }
        });

        authenticationProperties.IsPersistent = signInInput.IsRemember;

        await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal, authenticationProperties);

        return Response<bool>.Success(200);
    }

    public async Task<TokenResponse> GetAccessTokenByRefreshToken()
    {
        var discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false }
        });

        if (discoveryDocument.IsError)
            throw discoveryDocument.Exception!;

        var refreshToken =
            await _httpContextAccessor.HttpContext!.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

        var refreshTokenRequest = new RefreshTokenRequest
        {
            ClientId = _clientSettings.WebClientForUser.ClientId,
            ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
            RefreshToken = refreshToken!,
            Address = discoveryDocument.TokenEndpoint
        };

        var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);

        if (token.IsError)
            return null;

        var authenticationTokens = new List<AuthenticationToken>()
        {
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = token.AccessToken!
            },
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = token.RefreshToken!
            },
            new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.ExpiresIn,
                Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o", CultureInfo.InvariantCulture)
            }
        };

        var authenticationResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync();
        authenticationResult.Properties!.StoreTokens(authenticationTokens);
        await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            authenticationResult.Principal!, authenticationResult.Properties!);

        return token;
    }

    public async Task RevokeRefreshToken()
    {
        var discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = _serviceApiSettings.IdentityBaseUri,
            Policy = new DiscoveryPolicy { RequireHttps = false }
        });

        if (discoveryDocument.IsError)
            throw discoveryDocument.Exception!;

        var refreshToken =
            await _httpContextAccessor.HttpContext!.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

        var refreshTokenRevokeRequest = new TokenRevocationRequest
        {
            ClientId = _clientSettings.WebClientForUser.ClientId,
            ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
            Address = discoveryDocument.RevocationEndpoint,
            Token = refreshToken,
            TokenTypeHint = "refresh_token"
        };

        var response = await _httpClient.RevokeTokenAsync(refreshTokenRevokeRequest);

        if (response.IsError)
            throw response.Exception!;

        await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return;
    }
}