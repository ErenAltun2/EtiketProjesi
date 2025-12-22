using ClassLibrary; // DTO'yu kullanmak için bunu eklemeyi unutma
using System.Net.Http.Json;

namespace EtiketFrontend.Services
{
    public class EtiketApiService
    {
        private readonly HttpClient _httpClient;

        public EtiketApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // 1. Etiket Ekleme (Tek bir nesne döner)
        public async Task<EtiketResponseDto?> EtiketEkle(EtiketEkleDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Etiket/ekle", dto);

            if (response.IsSuccessStatusCode)
            {
                // dynamic yerine gerçek sınıfı kullanıyoruz
                return await response.Content.ReadFromJsonAsync<EtiketResponseDto>();
            }
            return null;
        }

        // 2. Resme göre etiketleri getir (Liste döner)
        public async Task<List<EtiketResponseDto>?> GetEtiketlerByImage(int imageId)
        {
            var response = await _httpClient.GetAsync($"api/Etiket/image/{imageId}");

            if (response.IsSuccessStatusCode)
            {
                // Liste olarak karşılıyoruz
                return await response.Content.ReadFromJsonAsync<List<EtiketResponseDto>>();
            }
            return null; // veya new List<EtiketResponseDto>() dönebilirsin
        }

        // 3. Koda göre etiketleri getir (Liste döner)
        public async Task<List<EtiketResponseDto>?> GetEtiketlerByCode(string kod)
        {
            var response = await _httpClient.GetAsync($"api/Etiket/code/{kod}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EtiketResponseDto>>();
            }
            return null;
        }

        // 4. Silme işlemi (Bool döner, bunda sorun yoktu ama aynen kalsın)
        public async Task<bool> DeleteEtiket(int etiketId)
        {
            var response = await _httpClient.DeleteAsync($"api/Etiket/{etiketId}");
            return response.IsSuccessStatusCode;
        }
    }
}