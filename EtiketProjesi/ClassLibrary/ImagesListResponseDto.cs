using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class ImagesListResponseDto
    {
        public string message { get; set; }
        public int resimSayisi { get; set; }
        public List<ImageResponseDto> resimler { get; set; }
    }
}
