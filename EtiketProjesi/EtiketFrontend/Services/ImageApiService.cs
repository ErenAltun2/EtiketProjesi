using ClassLibrary;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace EtiketFrontend.Services
{
    public class ImageApiService
    {
        private readonly HttpClient _httpClient;

        public ImageApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        
        public async Task<CreateImageSetResponseDto?> CreateImageSet(CreateImageSetDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Image/create-set", dto);

            if (response.IsSuccessStatusCode)
            {
                //aynı logindeki gibi databaseden gelen verileri daha detaylı veya istediğimiz şekilde kullanmamız için CreateImageSetResponseDto kullanıyoruz.
                return await response.Content.ReadFromJsonAsync<CreateImageSetResponseDto>();
            }

            // Hata varsa null dönmek yerine hatayı fırlatmak daha iyidir,
            // böylece ekranda neden oluşmadığını görebiliriz.
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Proje Oluşturma Hatası: {errorContent}");
        }
        // --------------------------------

        public async Task<bool> UploadImages(int imageSetId, List<IBrowserFile> files)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(imageSetId.ToString()), "imageSetId");

            // Güvenlik için maksimum dosya boyutu (örn: 50 MB)
            long maxFileSize = 50 * 1024 * 1024;

            foreach (var file in files)
            {
                try
                {
                    // 1. ÖNCE RAM'E KOPYALA (Bu adım Timeout hatasını çözer)
                    // Tarayıcıdan gelen veriyi önce sunucunun hafızasına alıyoruz.
                    // Böylece Blazor ile tarayıcı arasındaki bağlantı (Circuit) meşgul edilmiyor.
                    var memoryStream = new MemoryStream();

                    // Stream'i kopyalarken maksimum boyutu belirtiyoruz
                    await file.OpenReadStream(maxAllowedSize: maxFileSize).CopyToAsync(memoryStream);

                    // Stream'i başa sarıyoruz ki okunabilsin
                    memoryStream.Position = 0;

                    // 2. HTTP İÇERİĞİNİ HAZIRLA
                    // Artık StreamContent, tarayıcıyı beklemiyor, doğrudan RAM'den okuyor.
                    var fileContent = new StreamContent(memoryStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                    content.Add(fileContent, "images", file.Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Dosya işleme hatası ({file.Name}): {ex.Message}");
                    // Hata olsa bile diğer dosyaları denemeye devam etsin mi? 
                    // Şimdilik false dönüp işlemi durduruyoruz.
                    return false;
                }
            }

            try
            {
                // 3. API'YE GÖNDER
                var response = await _httpClient.PostAsync("api/Image/upload", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ API Bağlantı Hatası: {ex.Message}");
                return false;
            }
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