namespace EtiketAPI.Models
{
    public class Image
    {
        public int Id { get; set; }
        public int ImageSetId { get; set; } // Hangi gruba ait
        public string Name { get; set; }
        public byte[] image { get; set; }
        public DateTime yuklenmeTarihi { get; set; }

    }
}
