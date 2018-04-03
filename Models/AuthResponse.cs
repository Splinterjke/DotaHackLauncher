using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaHackLoader.Models
{
    public class AuthResponse
    {
        public int error { get; set; }
        public string message { get; set; }
        public string token { get; set; }
    }
}
