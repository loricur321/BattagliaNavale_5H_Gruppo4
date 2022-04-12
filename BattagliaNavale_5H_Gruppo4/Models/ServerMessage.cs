using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattagliaNavale_5H_Gruppo4.Models
{
    public class ServerMessage
    {
        public int type { get; set; }
        public string response { get; set; }
        public string endGame { get; set; }
        public string shipStatus { get; set; }
    }
}
