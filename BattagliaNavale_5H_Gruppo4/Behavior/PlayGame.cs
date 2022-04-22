using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        //Positions of the ships of the first client
        ClientShips _firstClientShips;
        //Positions of the ships of the second client
        ClientShips _secondClientShips;

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
            if (_count > 2)
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

        /// <summary>
        /// Method that will manage messagges from the two clients playing the game
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMessage(MessageEventArgs e)
        {
            //When the server receives a message i have to check which of the two clients has send it 
            //so that i can use that particular socket from answering
            WebSocket client = _clientSockets[0];

            if (client != _clientSockets[0])
                client = _clientSockets[1];

            //When i receive a message from a client i have to suppose that it'll be the preformed json so i can deserialize it
            ClientMessage msg = JsonConvert.DeserializeObject<ClientMessage>(e.Data);

            //The message from the client can have two status (type): 4 or 5
            //Type 3 is the message that i receive when the two client connects and send me the positions of their ships
            //Type 4 is the message that i receive when a client makes a move, so it has only the name of thr table choosen
            //and i need to answer hit or miss


            if (msg.type == 4) //Client has sent the positions of the ships so i need to save them
            {
                SaveShips(msg, client);

                //If both clients have sent the position of the ships i need to send a message that signals the start of the game
                if (_firstClientShips.IsFull && _secondClientShips.IsFull)
                {
                    foreach (var c in _clientSockets)
                        c.Send(Messages.StartGame);
                }
            }
            else if (msg.type == 5) //Client has made a move so i need to check if any rival ships were hitted and if they are all sunken
            {
                //If a client hits a ship i replace the name of the slot with "HIT" and then send a messagge to the client 
                //If a client doens't hit anything i'll just send the message signaling the miss

                bool hit = false;

                if (client == _firstClientShips.Client)
                {
                    foreach (var s in _secondClientShips.Ships)
                        for (int i = 0; i < s.positions.Length; i++)
                            if (s.positions[i] == msg.move)
                            {
                                s.positions[i] = "HIT";
                                hit = true;
                            }
                }
                else
                {
                    foreach (var s in _firstClientShips.Ships)
                        for (int i = 0; i < s.positions.Length; i++)
                            if (s.positions[i] == msg.move)
                            {
                                s.positions[i] = "HIT";
                                hit = true;
                            }
                }

                //If the flag hit is true it means that the client has hitted a part pf a ship so i need to let him know that
                //Is the flag is false i need to signal the client that he hasn't hitted anything
                if (hit)
                    client.Send(Messages.Hit);
                else
                    client.Send(Messages.Miss);

                //Now that i've checked the move i need to verify if any ships has been sunken
                CheckShips(client);

                //Now i can see if the fame is over
                CheckGameStatus();
            }

        }

        /// <summary>
        /// This method will check if one of the clients has all the ship sunken
        /// In that case i will signal che win of the other client
        /// </summary>
        private void CheckGameStatus()
        {
            //Check of the first client
            int sunkenShip = 0;

            int hitCounter = 0;
            foreach (var ship in _firstClientShips.Ships)
            {
                foreach (var pos in ship.positions)
                    if (pos == "HIT")
                        hitCounter++;

                if (hitCounter == ship.positions.Length)
                    sunkenShip++;
            }

            if(sunkenShip == _firstClientShips.Ships.Length)
            {
                //Client 2 has won
                _firstClientShips.Client.Send(Messages.Client2Wins);
                _secondClientShips.Client.Send(Messages.Client2Wins);
            }

            //If the second client has't won i need to check if the first one has
            sunkenShip = 0;

            hitCounter = 0;
            foreach (var ship in _secondClientShips.Ships)
            {
                foreach (var pos in ship.positions)
                    if (pos == "HIT")
                        hitCounter++;

                if (hitCounter == ship.positions.Length)
                    sunkenShip++;
            }

            if (sunkenShip == _secondClientShips.Ships.Length)
            {
                //Client 1 has won
                _firstClientShips.Client.Send(Messages.Client1Wins);
                _secondClientShips.Client.Send(Messages.Client1Wins);
            }
        }

        /// <summary>
        /// Method that will check if a ships is sunken
        /// </summary>
        private void CheckShips(WebSocket client)
        {
            //I receive the client that has made the move so i only need to check the other ships client
            //because that's where i've worked before
            WebSocket rivalClient;

            if (client == _clientSockets[0])
                rivalClient = _clientSockets[1];
            else
                rivalClient = _clientSockets[0];

            int sunkenShip = 0;

            if (rivalClient == _firstClientShips.Client)
            {
                int hitCounter = 0;
                foreach (var ship in _firstClientShips.Ships)
                {
                    foreach (var pos in ship.positions)
                        if (pos == "HIT")
                            hitCounter++;

                    if(hitCounter == ship.positions.Length)
                        sunkenShip++;
                }
            }
            else
            {
                int hitCounter = 0;
                foreach (var ship in _secondClientShips.Ships)
                {
                    foreach (var pos in ship.positions)
                        if (pos == "HIT")
                            hitCounter++;

                    if (hitCounter == ship.positions.Length)
                        sunkenShip++;
                }
            }

            //If the players has a sunken ship or more then one i will send him a message
            client.Send(Messages.Sunken);
        }

        /// <summary>
        /// Method that will proceed to save the ships of the client
        /// </summary>
        /// <param name="msg">message received from the client</param>
        /// <param name="client">client that has sent the message</param>
        private void SaveShips(ClientMessage msg, WebSocket client)
        {
            if (client == _clientSockets[0])
            {
                //First Client
                _firstClientShips = new ClientShips(true, client, msg.ships);
            }
            else
            {
                //Second client
                _secondClientShips = new ClientShips(true, client, msg.ships);
            }
        }
    }
}
