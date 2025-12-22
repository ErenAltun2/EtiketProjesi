using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class ImageResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageBase64 { get; set; }
        public DateTime yuklenmeTarihi { get; set; }
        public int ImageSetId { get; set; }
    }
}
