using ClassLibrary;
using EtiketAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EtiketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtiketController : ControllerBase
    {
        private readonly EtiketServis _etiketServis;

        public EtiketController(EtiketServis etiketServis)
        {
            _etiketServis = etiketServis;
        }

        [HttpPost("ekle")]
        public async Task<IActionResult> EtiketEkle([FromBody] EtiketEkleDto dto)
        {
            var sonuc = await _etiketServis.AddEtiket(
                dto.UserId,
                dto.ImageId,
                dto.Etiket,
                dto.XCenter,
                dto.YCenter,
                dto.Width,
                dto.Height
            );

            return Ok(sonuc);
        }


        //bır resmın ıcınde bırden fazla drone ucak gıbı nesne olabılır bırden fazla bır resım boundıng box ıcıne alınıyor o yuzden 
        [HttpGet("image/{imageId}")]
        public async Task<IActionResult> GetByImage(int imageId)
        {
            var etiketler = await _etiketServis.GetEtiketlerByImageId(imageId);
            return Ok(etiketler);
        }

        //bu servıste de yazdıgım gıbı paylasım kodunu verdım arkadasımıza o benım etıketledıgım seylerı gorebılmesı ıcın butun etıketler arasından paylasım kodu aynı olanları alıyorum burada. 
        [HttpGet("code/{paylasimKodu}")]
        public async Task<IActionResult> GetByCode(string paylasimKodu)
        {
            var etiketler = await _etiketServis.GetEtiketlerByCode(paylasimKodu);

            if (etiketler == null)
                return NotFound("Paylaşım kodu bulunamadı");

            return Ok(etiketler);
        }

        [HttpDelete("{etiketId}")]
        public async Task<IActionResult> Delete(int etiketId)
        {
            var silindi = await _etiketServis.DeleteEtiket(etiketId);

            if (!silindi)
                return NotFound("Etiket bulunamadı");

            return Ok("Etiket silindi");
        }

    }
}
