using EtiketAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EtiketAPI.Services
{
    public class UserService
    {   
        //bu bır c# projesı bu yuzden burada ınject kullanamıyoruz tablolara erısebılmemız ıcın etıketdb context den bır property olusturdum sonrasında 
        //bu class ın constructor ına bu olusturdugum degerı gırdım bu sayede bu sınıf her kullanıldıgında db context de kullanılacak
        //zaten bunu proje calısır calısmaz gerektıgı ıcın program.cs ın ıcıne buılder.services.AddScoped<>() seklınde ekleyecegız

        private readonly EtiketDbContext _context;

        public UserService(EtiketDbContext context)
        {
            _context = context;
        }


        public async Task <User?> Register(string isim , string soyisim, string telNo, string email , string password)
        {
            var MevcutKullanici = await _context.Users.FirstOrDefaultAsync(x => x.email == email);
            if(MevcutKullanici != null)
            {
                return null;
            }

            //User user = new User();
            //user.isim = isim;
            //user.soyisim = soyisim;
            //user.email = email;
            //user.telNo = telNo;
            //user.password = password;

            User YenıKullanici = new User
            {
                isim = isim,
                soyisim = soyisim,
                telNo = telNo,
                email = email,
                password = password,

            };

            _context.Users.Add(YenıKullanici);
            await _context.SaveChangesAsync();

            return YenıKullanici;
        }

        public async Task <User?> Login (string email , string password)
        {
            var User = await _context.Users.FirstOrDefaultAsync(x=>x.email == email && x.password==password);

            if(User != null)
            {
                return  User;
            }
            return null;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }


        public async Task<bool> DeleteUser(int id)
        {
            var kullanici= await _context.Users.FirstOrDefaultAsync(x=>x.Id==id);

            if(kullanici == null) 
            {
                return false;
            }
            
            _context.Users.Remove(kullanici);
            _context.SaveChangesAsync();
            return true;
        }



    }
}
