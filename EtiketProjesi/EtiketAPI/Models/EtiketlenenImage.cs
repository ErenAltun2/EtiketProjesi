namespace EtiketAPI.Models
{
    public class EtiketlenenImage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ImageId { get; set; }
        public string etiket { get; set; }

        // YOLO için koordinatlar
        public float x_center { get; set; }
        public float y_center { get; set; }
        public float width { get; set; }
        public float height { get; set; }

        public DateTime etiketlenmeTarihi { get; set; }

    }
}
