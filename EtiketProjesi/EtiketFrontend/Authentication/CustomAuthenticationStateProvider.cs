using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using EtiketFrontend.Models;

namespace EtiketFrontend.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        // 1. Sayfa yüklendiğinde çalışır: Kullanıcı giriş yapmış mı kontrol eder
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Tarayıcı hafızasından (Session) veriyi okumayı dener
                var userSessionStorageResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

                if (userSession == null)
                    return await Task.FromResult(new AuthenticationState(_anonymous));

                // Kullanıcı varsa Claims (Kimlik) oluşturulur
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.Email),
                    new Claim(ClaimTypes.Role, userSession.Role),
                    new Claim("UserId", userSession.UserId),
                    new Claim("Isim", userSession.Isim),
                    new Claim("Soyisim", userSession.Soyisim)
                }, "CustomAuth"));

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        // 2. Login ve Logout işlemlerinde bu metodu çağıracağız
        public async Task UpdateAuthenticationState(UserSession? userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                // LOGIN: Veriyi session'a yaz
                await _sessionStorage.SetAsync("UserSession", userSession);

                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.Email),
                    new Claim(ClaimTypes.Role, userSession.Role),
                    new Claim("UserId", userSession.UserId),
                    new Claim("Isim", userSession.Isim),
                    new Claim("Soyisim", userSession.Soyisim)
                }, "CustomAuth"));
            }
            else
            {
                // LOGOUT: Veriyi sil
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
            }

            // Blazor'a durumun değiştiğini bildir
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}