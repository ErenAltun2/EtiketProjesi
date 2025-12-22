using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class CreateImageSetResponseDto
    {
        public int imageSetId { get; set; }
        public string paylasmaKodu { get; set; } // string veya int olabilir, API'ye bağlı
        public string olusturmaTarihi { get; set; }
        public int resimSayisi { get; set; } = 0;
    }
}
    