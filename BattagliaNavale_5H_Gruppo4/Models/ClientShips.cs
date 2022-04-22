using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace BattagliaNavale_5H_Gruppo4.Models
{
    internal class ClientShips
    {
        public bool IsFull { get; set; }

        public WebSocket Client { get; set; }

        public Ship[] Ships { get; set; }

        public ClientShips () { }

        public ClientShips (bool isFull, WebSocket client, Ship[] ship)
        {
            this.IsFull = isFull;
            this.Client = client;
            this.Ships = ship;
        }
    }
}
