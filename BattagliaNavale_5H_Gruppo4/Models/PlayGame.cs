using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace BattagliaNavale_5H_Gruppo4.Models
{
    /// <summary>
    /// Method that will manage al the game logics.
    /// </summary>
    class PlayGame : WebSocketBehavior
    {
        //At first it will use the OnOpen override method in order to permit the two players (client)
        //to connect to this web server.
        //The web server has to accept the connection from the first two clients and then after that reject other connections
        //Once the clients are connected they have to communicate to the web server the positions of their battle ship
        //so that the game can start.
        
        //List of connected clients
        private static List<WebSocket> _clientSockets = new List<WebSocket>();

        //number of clients 
        static int _count = 0;

        /// <summary>
        /// Method that will manage new connections from clients
        /// </summary>
        protected override void OnOpen()
        {
            _count++;
            WebSocket newClient = Context.WebSocket;

            Console.WriteLine($"Request of connection from the client number {_count}. Let's see if i can accept him...");

            //I can only accept two players at a time
            if(_count > 2)
            {
                Console.WriteLine($"Cannot accept client number {_count}... Closing connection");
                string closeString = "Server cannot accept anymore clients!";
                
                //Action<bool> completed;
                //newClient.SendAsync(closeString, completed);

                newClient.Send(closeString);

                //Close the connection with the client
                newClient.Close();
            }
            else
            {
                Console.WriteLine($"Succesfully established connection with client number {_count}!");
                _clientSockets.Add(newClient);
            }
        }

        /// <summary>
        /// Method that will proceed to close the connection with the client when it disconnetcs
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClose(CloseEventArgs e)
        {
            int index = _clientSockets.IndexOf(Context.WebSocket);

            //if the index is -1 it means that the client that is disconnecting isn't part of the list
            //so it's only a client that tries to connect and immediately disconnects because it gets rejected by the server
            //
            //If the index is not -1 the client id part of the list and so one of the two players
            //In that case i remove him from the list so that a new client can start to play
            if (index != -1)
            {
                Console.WriteLine($"Client number {index + 1} has disconnected!");

                _clientSockets.RemoveAt(index); //I remove the old client from the list so that a new client can connect and start a new game
            }
            else
                Console.WriteLine($"Client number {_count} has disconnected!");

        }
    }
}
