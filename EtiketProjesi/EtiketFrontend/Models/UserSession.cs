namespace EtiketFrontend.Models
{
    public class UserSession
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Isim { get; set; } = string.Empty;
        public string Soyisim { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }
    
    //bu model kullanıcı bılgılerını sessıon da tutacak 
    //Performans ve Veritabanı Yükünü AzaltmakHer sayfa yüklendiğinde veya her işlem yapıldığında veritabanına gidip "Bu kullanıcı kimmiş?" diye sormak yerine, tarayıcının hafızasında (Session Storage) kullanıcı bilgilerini saklıyoruz.
}
