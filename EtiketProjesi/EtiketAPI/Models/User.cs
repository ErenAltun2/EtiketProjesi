namespace EtiketAPI.Models
{
    public class User
    {
            
        public int Id { get; set; }
        public string isim { get; set; }
        public string soyisim { get; set; }
        public string telNo { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public string rol { get; set; } = "User";

    }
}
