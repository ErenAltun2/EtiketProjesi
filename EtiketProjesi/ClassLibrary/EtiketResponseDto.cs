using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class EtiketResponseDto
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public int UserId { get; set; }
        public string Etiket { get; set; }
        public float XCenter { get; set; }
        public float YCenter { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
    }
}
