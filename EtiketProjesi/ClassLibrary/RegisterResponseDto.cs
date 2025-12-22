using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class RegisterResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = true;
    }
}
