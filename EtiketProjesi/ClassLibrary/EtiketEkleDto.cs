using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class EtiketEkleDto
    {
        public int UserId { get; set; }
        public int ImageId { get; set; }
        public string Etiket { get; set; }

        // YOLO koordinatları (0-1 arası normalize edilmiş)
        public float XCenter { get; set; }
        public float YCenter { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
