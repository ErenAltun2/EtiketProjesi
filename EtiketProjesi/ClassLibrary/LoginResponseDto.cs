using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class LoginResponseDto
    {
        //bu sayfadakıler ıse az once logınDTO ıle backend e gıden bılgılerın sonucunda bıze verılen bılgıler 
        //yanı bır numara ıle gıdıyor benım numaram bu dıye backend bıze bu numaraya kayıtlı bılgıler bunlar dıyıp bıze verılerı verıyor. 
        public int userId { get; set; }
        public string email { get; set; }
        public string isim { get; set; }
        public string soyisim { get; set; }
        public string? role { get; set; }
        public string? token { get; set; }
    }
}
