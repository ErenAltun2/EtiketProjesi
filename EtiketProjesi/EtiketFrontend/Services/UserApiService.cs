using ClassLibrary; // LoginDto ve RegisterDto burada varsayıyoruz
using EtiketFrontend.Models; // UserSession modelini dahil ettik
using System.Net.Http.Json;

namespace EtiketFrontend.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;

        public UserApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // Register yine dynamic kalabilir ama Login kesinlikle model dönmeli
        // using ClassLibrary; eklemeyi unutmayın

        public async Task<RegisterResponseDto?> Register(RegisterDto dto)
        {
            // API Adresiniz farklı olabilir (örn: api/Auth/register), kontrol edin
            var response = await _httpClient.PostAsJsonAsync("api/User/register", dto);

            if (response.IsSuccessStatusCode)
            {
                // ARTIK DYNAMIC DEĞİL, DTO DÖNÜYORUZ
                // Bu sayede "!= null" hatası düzelir.
                return await response.Content.ReadFromJsonAsync<RegisterResponseDto>();
            }

            // Hata varsa null dönebilir veya hatayı fırlatabilirsiniz
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Kayıt Hatası: {error}");
        }

        // --- DÜZELTİLEN KISIM BURASI ---
        // Artık 'dynamic' yerine 'UserSession' döndürüyoruz.
        public async Task<LoginResponseDto?> Login(LoginDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/User/login", dto);

            if (response.IsSuccessStatusCode)
            {
                // API'den gelen veriyi artık hatasız okuyacak, çünkü DTO tipleri uyuşuyor (int userId)
                return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Giriş Hatası: {errorContent}");
        }
        // -------------------------------

        public async Task<bool> DeleteUser(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/User/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}