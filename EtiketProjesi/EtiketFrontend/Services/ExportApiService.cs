using System.Net.Http;

namespace EtiketFrontend.Services
{
    public class ExportApiService
    {
        private readonly HttpClient _httpClient;

        public ExportApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<byte[]?> DownloadYoloDataset(string paylasimKodu)
        {
            var response = await _httpClient.GetAsync($"api/Export/yolo/{paylasimKodu}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }
    }
}