using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EtiketAPI.Services;
using ClassLibrary;
namespace EtiketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }
        //controllerda o zaman aynı seyı yapacagım cunku burda da c# kodları yazılıyor burda da servis ı ımport edıyorum aslında 
        // servıslerde de dbcontext(tabloları) edıyordum

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto) 
        {
            var kullanici = await _userService.Register(dto.Isim , dto.Soyisim , dto.TelNo , dto.Email,dto.Password);

            //burda şu oluyor userservice tanımladıgımız register methoduna dto aracılıgıyla bılgılerı alıyoruz sonrasında bu bılgılerı regıster methodu cagırıp servıse ıletıyor 
            // servısten donen deger ya null oalcak veya kullaniciya iletecek yanı bu kullanici null deger ve kullanıcı parametrelerını tutacak

            if(kullanici== null)
            {
                return BadRequest(new { message = "bu kullanici daha önce kayıt olmuş" });
            }
            return Ok(new { message = "Kayıt başarılı",
                userId = kullanici.Id,
                isim = kullanici.isim,
                email = kullanici.email,
                role = kullanici.rol
            }) ;
        }

        // Giriş yap
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var kullanici = await _userService.Login(dto.Email, dto.Password);

            if (kullanici == null)
                return Unauthorized(new { message = "Email veya şifre hatalı" });

            return Ok(new
            {
                message = "Giriş başarılı",
                userId = kullanici.Id,
                isim = kullanici.isim,
                soyisim = kullanici.soyisim,
                email = kullanici.email,
                role = kullanici.rol
            });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var silindi = await _userService.DeleteUser(id);

            if (!silindi)
                return NotFound(new { message = "Kullanıcı bulunamadı" });

            return Ok(new { message = "Kullanıcı silindi" });
        }
    }
}
