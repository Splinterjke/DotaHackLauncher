using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaHackLoader.Models
{
    public class UserData
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string[] gender { get; set; }
        public string language { get; set; }
        public string register_date { get; set; }
        public string avatar { get; set; }
        public string days_left { get; set; }
        public string hwid { get; set; }
        public string package { get; set; }
        public object buy { get; set; } 
    }
}
