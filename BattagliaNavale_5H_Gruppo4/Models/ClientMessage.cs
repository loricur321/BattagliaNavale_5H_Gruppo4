using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattagliaNavale_5H_Gruppo4.Models
{

    public class ClientMessage
    {
        public int type { get; set; }
        public Ship[] ships { get; set; }
        public string move { get; set; }
    }

    public class Ship
    {
        public string[] positions { get; set; }
    }

}
