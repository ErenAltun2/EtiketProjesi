namespace EtiketAPI.Models
{
    public class ImageSet
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Kim yükledi
        public string paylasmKodu { get; set; } // 6 basamaklı kod
        public DateTime olusturmaTarihi { get; set; }
    }
}
