using BattagliaNavale_5H_Gruppo4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace BattagliaNavale_5H_Gruppo4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WebSocketServer wbsv = new WebSocketServer("ws://127.0.0.1:9000");

            wbsv.AddWebSocketService<PlayGame>("/PlayGame");

            wbsv.Start();
            Console.WriteLine("Server started on 127.0.0.1:9000");

            Console.ReadKey();
            wbsv.Stop();
        }
    }
}
