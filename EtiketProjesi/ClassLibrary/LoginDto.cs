using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    //bu fotografta gordugunuz kullanıcının web sayfasında gırecegı bılgıler oluyor 
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
