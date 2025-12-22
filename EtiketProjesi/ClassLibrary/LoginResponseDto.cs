using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class LoginResponseDto
    {
        // API 'userId'yi sayı gönderdiği için burası int olmalı
        public int userId { get; set; }
        public string email { get; set; }
        public string isim { get; set; }
        public string soyisim { get; set; }
        public string? role { get; set; }
        public string? token { get; set; }
    }
}
