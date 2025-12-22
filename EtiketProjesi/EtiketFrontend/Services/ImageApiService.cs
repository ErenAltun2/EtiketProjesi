using ClassLibrary;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms; // <--- BU SATIRI EKLEMEYİ UNUTMAYIN

namespace EtiketFrontend.Services
{
    public class ImageApiService
    {
        private readonly HttpClient _httpClient;

        public ImageApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        // --- DÜZELTİLEN METOT BURASI ---
        public async Task<CreateImageSetResponseDto?> CreateImageSet(CreateImageSetDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Image/create-set", dto);

            if (response.IsSuccessStatusCode)
            {
                // Artık dynamic (JsonElement) değil, gerçek bir sınıf dönüyoruz.
                // Bu sayede "!= null" hatası çözülecek.
                return await response.Content.ReadFromJsonAsync<CreateImageSetResponseDto>();
            }

            // Hata varsa null dönmek yerine hatayı fırlatmak daha iyidir,
            // böylece ekranda neden oluşmadığını görebilirsiniz.
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Proje Oluşturma Hatası: {errorContent}");
        }
        // --------------------------------

        public async Task<bool> UploadImages(int imageSetId, List<IBrowserFile> files)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(imageSetId.ToString()), "imageSetId");

            foreach (var file in files)
            {
                // Max dosya boyutu 10MB olarak ayarlanmış
                var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "images", file.Name);
            }

            var response = await _httpClient.PostAsync("api/Image/upload", content);
            return response.IsSuccessStatusCode;
        }

        // Şimdilik diğerleri dynamic kalabilir ama ileride onları da DTO yapmanız önerilir.
        public async Task<ImagesListResponseDto?> GetImagesByCode(string kod)
        {
            Console.WriteLine($"🔍 Kod ile resim aranıyor: {kod}");
            var response = await _httpClient.GetAsync($"api/Image/kod/{kod}");

            Console.WriteLine($"📡 Response Status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📦 Response: {content}");

                var result = await response.Content.ReadFromJsonAsync<ImagesListResponseDto>();
                Console.WriteLine($"✅ Parse edildi: {result?.resimSayisi} resim");
                return result;
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Hata: {error}");
            return null;
        }

        public async Task<ImageResponseDto?> GetImageById(int imageId)
        {
            var response = await _httpClient.GetAsync($"api/Image/{imageId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ImageResponseDto>();  // ✅ DTO kullan
            }
            return null;
        }

        public async Task<CreateImageSetResponseDto?> GetImageSetInfo(string kod)
        {
            var response = await _httpClient.GetAsync($"api/Image/set-info/{kod}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CreateImageSetResponseDto>();
            }
            return null;
        }

        public async Task<List<CreateImageSetResponseDto>?> GetUserProjects(int userId)
        {
            var response = await _httpClient.GetAsync($"api/Image/user-projects/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = System.Text.Json.JsonDocument.Parse(content);

                if (jsonDoc.RootElement.TryGetProperty("projeler", out var projelerElement))
                {
                    var projeler = new List<CreateImageSetResponseDto>();

                    foreach (var item in projelerElement.EnumerateArray())
                    {
                        projeler.Add(new CreateImageSetResponseDto
                        {
                            imageSetId = item.GetProperty("id").GetInt32(),
                            paylasmaKodu = item.GetProperty("paylasmKodu").GetString(),
                            olusturmaTarihi = item.GetProperty("olusturmaTarihi").GetDateTime().ToString("dd.MM.yyyy HH:mm"),
                            resimSayisi = item.GetProperty("resimSayisi").GetInt32()
                        });
                    }

                    return projeler;
                }
            }
            return null;
        }

        public async Task<bool> DeleteImageSet(int imageSetId)
        {
            var response = await _httpClient.DeleteAsync($"api/Image/delete-set/{imageSetId}");
            return response.IsSuccessStatusCode;
        }
    }
}