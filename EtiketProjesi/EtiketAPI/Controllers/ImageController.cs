using EtiketAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClassLibrary;
using EtiketAPI.Models;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;

namespace EtiketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageServis _ımageServis;

        public ImageController(ImageServis ımageServis)
        {
            _ımageServis = ımageServis;
        }

        //burda resımlerı yuklemek ıcın bır dosya olusturuyoruz.
        [HttpPost("create-set")]
        public async Task<IActionResult> CreateImageSet([FromBody] CreateImageSetDto dto)
        {
            var Dosya = await _ımageServis.CreateImageSet(dto.UserId);  //burada klasor olusturuyom ve kımın actıgını tutuyorum yanı eren dosya actı dıyorum 
                                                                        // dıger bılgılerı otomatık zaten bakarsak olusturulma tarıhı datetımenow dedım dıger bılgıde essız kod bu kodda zaten CreateImageSet ın ıcınde var 
            return Ok(new
            {
                message = "Klasör Oluşturuldu",
                imageSetId = Dosya.Id,
                paylasmaKodu = Dosya.paylasmKodu,
                olusturmaTarihi = Dosya.olusturmaTarihi
            });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImages([FromForm] int imageSetId, [FromForm] List<IFormFile> images)
        {
            if (images == null || !images.Any())
            {
                return BadRequest(new { message = "Lütfen en az bir resim seçin" });
            }
            var ResimFormatlari = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            foreach (var image in images)
            {
                var Resminuzantisi = Path.GetExtension(image.FileName).ToLower();    //burada tek tek yuklenen her resmın uygun olup olmadıgına bakmak ıcın uzantısına bakacagız
                // bu resmın uzantısı sadece jpg vs degerı ıcerır yanı eren.jpg vs ıcermez  bu yuzden asagıdakı kodda su var resımformatlari resım uzantısını ıcerıyor mu dıye soruyor 
                if (!ResimFormatlari.Contains(Resminuzantisi))
                {
                    return BadRequest(new { message = $"Geçersiz Dosya Formatı {image.Name}" });
                }


            }

            var ImageUploadList = new List<ImageUploadDto>();  //bırden fazla resım atabıldıgımız ıcın lıste seklınde olusturuyorum

            foreach (var image in images)
            {
                using (var MemoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(MemoryStream);
                    ImageUploadList.Add(new ImageUploadDto
                    {
                        Name = image.FileName,
                        ImageData = MemoryStream.ToArray()
                    });
                }
            }
            // her resımı yukarıda byte donusturdum saklamak ıcın sımdı ıse kaydetmek kaldı 
            var basarili = await _ımageServis.SaveImages(imageSetId, ImageUploadList);
            if (!basarili)
            {
                return NotFound(new { message = "Kaydedilecek Klasör Bulunamadı" });
            }
            return Ok(new { message = "Resimler Başarıyla Yüklendi", resimSayisi = ImageUploadList.Count });
        }

        //şimdi ise kod ile resimleri getirme yani arkadışım paylastıgım kodu girince ekrana resimlerin gelmesi gerekiyor
        [HttpGet("kod/{paylasmaKodu}")]
        public async Task<IActionResult>GetImagesByCode(string paylasmaKodu)
        {
            var images= await _ımageServis.ImageGetByCode(paylasmaKodu);
            if (images == null || !images.Any())
                return NotFound(new { message = "Bu koda ait resim bulunamadı" });


            //gelecek olan resımlerı gostermek ıcın frontend de base64 e cevırerek gonderecegız   dıger degerler sabıt kalıyor daha anlasılır olsun dıye result degerının ıcıne attım hepsını 

            var result = images.Select(img => new
            {
                img.Id,
                img.Name,
                ImageBase64 = Convert.ToBase64String(img.image),
                img.yuklenmeTarihi,
                ImageSetId = img.ImageSetId
            });

            return Ok(new
            {
                message = "Resimler bulundu",
                resimSayisi = images.Count,
                resimler = result
            });
        }

        //sımdı ıse etıketleme ıslemı sırasında ekranda tek tek fotograflar olacak o yuzden o kodu yazmam gerekıyor yukarıda kod ıle resımlerı getırme demek aslında resımlerı ekrana gostermeyecek kısının o resımlere erısımı olacak 
        [HttpGet("{imageId}")]
        public async Task <IActionResult> GetImageById(int imageId)
        {
            var image = await _ımageServis.GetImageById(imageId);
            if(image == null)
            {
                return NotFound(new { message = "Resim Bulunamadı" });   //bu hata soyle olabılır resımler yuklendı bırıne bastık buyukttuk etıketlemek ıcın acılmadı bu hatayı verecek bıze .
            }

            return Ok(new
            {
                image.Id,
                image.Name,
                ImageBase64 = Convert.ToBase64String(image.image),
                image.yuklenmeTarihi,
                ImageSetId = image.ImageSetId
            });
        }


        //bu yazacagım ıle de servıste tanımladıgım GetImageSetByCode methodu napıyordu paylasım kodunu gırıyorduk ve bu klasoru resımlerı yukleyenı vs gorebılıyorduk onu paylasım kodunu gosteren bu oluyor. 
        [HttpGet("set-info/{paylasimkodu}")]
        public async Task<IActionResult> GetImageSetInfo(string paylasimkodu)
        {
            var imageSet = await _ımageServis.GetImageSetByCode(paylasimkodu);
            if (imageSet == null)
            {
                return NotFound(new { message = "Bu koda ait bir Klasör Bulunamadı" });
            }
            var images = await _ımageServis.ImageGetByCode(paylasimkodu);

            // CreateImageSetResponseDto ile uyumlu yap:
            return Ok(new
            {
                imageSetId = imageSet.Id,  // <--- "imageSetId" olarak değiştir
                userId = imageSet.UserId,   // <--- lowercase
                paylasmaKodu = imageSet.paylasmKodu,
                olusturmaTarihi = imageSet.olusturmaTarihi.ToString("yyyy-MM-dd HH:mm:ss"),
                resimSayisi = images?.Count ?? 0
            });
        }

        // Kullanıcının kendi projelerini getir
        [HttpGet("user-projects/{userId}")]
        public async Task<IActionResult> GetUserProjects(int userId)
        {
            var imageSets = await _ımageServis.GetUserImageSets(userId);

            if (imageSets == null || !imageSets.Any())
                return Ok(new { message = "Henüz proje oluşturmadınız", projeler = new List<object>() });

            var result = new List<object>();

            foreach (var imageSet in imageSets)
            {
                // ImageServis'te yeni bir metod oluştur
                var imageCount = await _ımageServis.GetImageCountBySetId(imageSet.Id);

                result.Add(new
                {
                    imageSet.Id,
                    imageSet.paylasmKodu,
                    imageSet.olusturmaTarihi,
                    resimSayisi = imageCount
                });
            }

            return Ok(new { message = "Projeler bulundu", projeler = result });
        }


        [HttpDelete("delete-set/{imageSetId}")]
        public async Task<IActionResult> DeleteImageSet(int imageSetId)
        {
            var result = await _ımageServis.DeleteImageSet(imageSetId);

            if (!result)
                return NotFound(new { message = "Proje bulunamadı" });

            return Ok(new { message = "Proje silindi" });
        }
    }
}
