using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattagliaNavale_5H_Gruppo4.Models
{
    static internal class Messages
    {
        /// <summary>
        /// Property that signals the start of the game
        /// </summary>
        public static string StartGame
        {
            get
            {
                ServerMessage msg = new ServerMessage
                {
                    type = 1,
                    response = "Start game"
                };
                return JsonConvert.SerializeObject(msg, Formatting.Indented);
            }
        }

        /// <summary>
        /// Property that communicates the win of the first client
        /// </summary>
        public static string Client1Wins
        {
            get
            {
                ServerMessage msg = new ServerMessage
                {
                    type = 2,
                    endGame = "Client 1 has won"
                };
                return JsonConvert.SerializeObject(msg, Formatting.Indented);
            }
        }

        /// <summary>
        /// Property that communjcates the win of the second client
        /// </summary>
        public static string Client2Wins
        {
            get
            {
                ServerMessage msg = new ServerMessage
                {
                    type = 2,
                    endGame = "Client 2 has won"
                };
                return JsonConvert.SerializeObject(msg, Formatting.Indented);
            }
        }

        /// <summary>
        /// Property that communicates that the move hasn't hitted the other player's ships
        /// </summary>
        public static string Miss
        {
            get
            {
                ServerMessage msg = new ServerMessage
                {
                    type = 1,
                    response = "Miss"
                };
                return JsonConvert.SerializeObject(msg, Formatting.Indented);
            }
        }

        /// <summary>
        /// Property that communicates that the move has hitted the other player's ships
        /// </summary>
        public static string Hit
        {
            get
            {
                ServerMessage msg = new ServerMessage
                {
                    type = 1,
                    response = "Hit"
                };
                return JsonConvert.SerializeObject(msg, Formatting.Indented);
            }
        }

        /// <summary>
        /// Property that comminucates of a ship has been sunken
        /// </summary>
        public static string Sunken
        {
            get
            {
                ServerMessage msg = new ServerMessage
                {
                    type = 3,
                    response = "Ship sunken"
                };
                return JsonConvert.SerializeObject(msg, Formatting.Indented);
            }
        }
    }
}
