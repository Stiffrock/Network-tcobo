using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RelayServer;
using Microsoft.Xna.Framework;

namespace Server
{
    public class Server
    {
        //Singleton in case we need to access this object without a reference (call <Class_Name>.singleton)
        public static Server singleton;
        public MySQL mySQL;
        //Create an object of the Listener class.
        Listener listener;
        public Listener Listener
        {
            get { return listener; }
        }

        //Array of clients
        Client[] client;

        //number of connected clients
        int connectedClients = 0;

        //Writers and readers to manipulate data
        MemoryStream readStream;
        MemoryStream writeStream;
        BinaryReader reader;
        BinaryWriter writer;
        Game1 game;

        private string connStr;
        string server = "localhost";
        string user = "root";
        string database = "tibia";
        string pass = "";

        /// <summary>
        /// Create a new Server object
        /// </summary>
        /// <param name="port">The port you want to use</param>
        public Server(int port, Game1 game)
        {
            this.game = game;
            connStr = "Server=" + server + ";Database=" + database + ";Uid=" + user + ";Pwd=" + pass + ";";
            mySQL = new MySQL(connStr);
            mySQL.Connect();
            //Initialize the array with a maximum of the MaxClients from the config file.
            client = new Client[ServerConfig.MaxPlayers];

            //Create a new Listener object
            listener = new Listener(port);
            listener.userAdded += new ConnectionEvent(listener_userAdded);
            listener.Start();

            //Create the readers and writers.
            readStream = new MemoryStream();
            writeStream = new MemoryStream();
            reader = new BinaryReader(readStream);
            writer = new BinaryWriter(writeStream);

            //Set the singleton to the current object
            Server.singleton = this;
        }

        /// <summary>
        /// Method that is performed when a new user is added.
        /// </summary>
        /// <param name="sender">The object that sent this message</param>
        /// <param name="user">The user that needs to be added</param>
        private void listener_userAdded(object sender, Client user)
        {
            connectedClients++;

            //Set up the events
            user.DataReceived += new DataReceivedEvent(user_DataReceived);
            user.UserDisconnected += new ConnectionEvent(user_UserDisconnected);

            //Print the new player message to the server window.
            Console.WriteLine(user.ToString() + " connected\tConnected Clients:  " + connectedClients + "\n");

            //Add to the client array
            client[user.id] = user;
        }

        /// <summary>
        /// Method that is performed when a new user is disconnected.
        /// </summary>
        /// <param name="sender">The object that sent this message</param>
        /// <param name="user">The user that needs to be disconnected</param>
        private void user_UserDisconnected(object sender, Client user)
        {
            connectedClients--;

            //Print the removed player message to the server window.
            Console.WriteLine(user.ToString() + " disconnected\tConnected Clients:  " + connectedClients + "\n");

            //Clear the array's index
            client[user.id] = null;
        }

        /// <summary>
        /// Relay messages sent from one client and send them to others
        /// </summary>
        /// <param name="sender">The object that called this method</param>
        /// <param name="data">The data to relay</param>
        private void user_DataReceived(Client sender, byte[] data)
        {
            writeStream.Position = 0;

            readStream.SetLength(0);
            readStream.Position = 0;

            readStream.Write(data, 0, data.Length);
            readStream.Position = 0;

            Protocol p;

            try
            {
                p = (Protocol)reader.ReadByte();

                if (p == Protocol.Connected)
                {
                }
                else if (p == Protocol.Disconnected)
                {
                }
                else if (p == Protocol.CreateAccount)
                {
                    bool succes = false;
                    string acc_name = reader.ReadString();
                    string acc_pass = reader.ReadString();
                    bool already_exist = mySQL.UserExists(acc_name);
                    if (already_exist == false)
                        succes = mySQL.CreateAccount(acc_name, acc_pass);
                    writeStream.Position = 0;
                    writer.Write((byte)Protocol.CreateAccountResult);
                    writer.Write(succes);
                    SendData(GetDataFromMemoryStream(writeStream), sender);
                }
                else if (p == Protocol.LoginAccount)
                {
                    string acc_name = reader.ReadString();
                    string acc_pass = reader.ReadString();
                    bool succes = mySQL.VerifyUser(acc_name, acc_pass);
                    int acc_id = mySQL.FindIdOfAccountName(acc_name);
                    int acc_total_characters = mySQL.FindTotalCharacterOfAccountId(acc_id);
                    string[] character_names = mySQL.FindCharacterList(acc_id, acc_total_characters);
                    writeStream.Position = 0;
                    writer.Write((byte)Protocol.LoginAccountResult);
                    writer.Write(succes);
                    writer.Write(acc_total_characters);
                    for (int i = 0; i < acc_total_characters; i++)
                    {
                        writer.Write(character_names[i]);
                    }
                    SendData(GetDataFromMemoryStream(writeStream), sender);
                }
                else if (p == Protocol.CreateCharacter)
                {
                    bool succes = false;
                    string acc_name = reader.ReadString();
                    string acc_pass = reader.ReadString();
                    string char_name = reader.ReadString();
                    int char_gender = reader.ReadInt32();
                    bool already_exist = mySQL.PlayerExist(char_name);
                    if (already_exist == false)
                    {
                        if (mySQL.VerifyUser(acc_name, acc_pass))
                        {
                            int acc_id = mySQL.FindIdOfAccountName(acc_name);
                            succes = mySQL.CreateCharacter(acc_id, char_name, char_gender);
                            Player[] new_players = new Player[game.players.Length + 1];
                            for (int i = 0; i < game.players.Length; i++)
                            {
                                new_players[i] = game.players[i];
                            }
                            new_players[game.players.Length] = mySQL.NewCharacterValues(mySQL.LoadCharacter(mySQL.FindIdOfCharacter(char_name)));
                            game.players = new Player[new_players.Length];
                            game.players = new_players;
                        }
                    }
                    writeStream.Position = 0;
                    writer.Write((byte)Protocol.CreateCharacterResult);
                    writer.Write(succes);
                    SendData(GetDataFromMemoryStream(writeStream), sender);
                }
                else if (p == Protocol.LoginCharacter)
                {
                    bool succes = false;
                    int char_id = 0;
                    string acc_name = reader.ReadString();
                    string acc_pass = reader.ReadString();
                    string char_name = reader.ReadString();
                    int acc_id = mySQL.FindAccountIdOfCharacter(char_name);
                    string compare_acc = mySQL.FindAccountNameOfId(acc_id);
                    string compare_pass = mySQL.FindAccountPasswordOfId(acc_id);
                    if (acc_name == compare_acc && acc_pass == compare_pass)
                    {
                        //Check if another player from the account is logged in here:
                        bool dublicate = false;
                        for (int i = 0; i < game.players.Length; i++)
                        {
                            if (game.players[i].online == true)
                            {
                                if (game.players[i].acc_id == acc_id)
                                {
                                    dublicate = true;
                                    break;
                                }
                            }
                        }
                        if (dublicate == false)
                        {
                            char_id = mySQL.FindIdOfCharacter(char_name);
                            succes = true;
                            game.players[char_id - 1].online = true;
                        }
                    }
                    writeStream.Position = 0;
                    writer.Write((byte)Protocol.LoginCharacterResult);
                    writer.Write(succes);
                    writer.Write(char_id);
                    SendData(GetDataFromMemoryStream(writeStream), sender);
                }
                else if (p == Protocol.Update)
                {
                    string acc_name = reader.ReadString();
                    string acc_pass = reader.ReadString();
                    int char_id = reader.ReadInt32();

                    if (game.players[char_id - 1].id == char_id)
                    {
                        if (acc_name == game.players[char_id - 1].acc_name && acc_pass == game.players[char_id - 1].acc_pass)
                        {
                            int count = 0;
                            for (int i = 0; i < game.players.Length; i++)
                            {
                                if (game.players[i].online == true && game.players[i].id != char_id)
                                {
                                    count++;
                                }
                            }
                            writeStream.Position = 0;
                            writer.Write((byte)Protocol.Updated);
                            writer.Write((int)count);
                            for (int i = 0; i < game.players.Length; i++)
                            {
                                if (game.players[i].online == true && game.players[i].id != char_id)
                                {
                                    writer.Write((int)game.players[i].id);
                                    writer.Write((int)game.players[i].position.X);
                                    writer.Write((int)game.players[i].position.Y);
                                    writer.Write((int)game.players[i].position.Z);
                                }
                            }
                            writer.Write((int)game.players[char_id - 1].position.X);
                            writer.Write((int)game.players[char_id - 1].position.Y);
                            writer.Write((int)game.players[char_id - 1].position.Z);
                            SendData(GetDataFromMemoryStream(writeStream), sender);
                        }
                    }
                }
                else if (p == Protocol.Move)
                {
                    string acc_name = reader.ReadString();
                    string acc_pass = reader.ReadString();
                    int char_id = reader.ReadInt32();
                    int direction = reader.ReadInt32();
                    if (game.players[char_id - 1].id == char_id)
                    {
                        if (acc_name == game.players[char_id - 1].acc_name && acc_pass == game.players[char_id - 1].acc_pass)
                        {
                            if (direction == 0)
                                game.players[char_id - 1].position.X -= 1;
                            else if (direction == 1)
                                game.players[char_id - 1].position.X += 1;
                            else if (direction == 2)
                                game.players[char_id - 1].position.Y -= 1;
                            else if (direction == 3)
                                game.players[char_id - 1].position.Y += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            SendData(data, sender);

        }

        /// <summary>
        /// Converts a MemoryStream to a byte array
        /// </summary>
        /// <param name="ms">MemoryStream to convert</param>
        /// <returns>Byte array representation of the data</returns>
        private byte[] GetDataFromMemoryStream(MemoryStream ms)
        {
            byte[] result;

            //Async method called this, so lets lock the object to make sure other threads/async calls need to wait to use it.
            lock (ms)
            {
                int bytesWritten = (int)ms.Position;
                result = new byte[bytesWritten];

                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
            }

            return result;
        }

        /// <summary>
        /// Combines one byte array with a MemoryStream
        /// </summary>
        /// <param name="data">Original Message in byte array format</param>
        /// <param name="ms">Message to append to the original message in MemoryStream format</param>
        /// <returns>Combined data in byte array format</returns>
        private byte[] CombineData(byte[] data, MemoryStream ms)
        {
            //Get the byte array from the MemoryStream
            byte[] result = GetDataFromMemoryStream(ms);

            //Create a new array with a size that fits both arrays
            byte[] combinedData = new byte[data.Length + result.Length];

            //Add the original array at the start of the new array
            for (int i = 0; i < data.Length; i++)
            {
                combinedData[i] = data[i];
            }

            //Append the new message at the end of the new array
            for (int j = data.Length; j < data.Length + result.Length; j++)
            {
                combinedData[j] = result[j - data.Length];
            }

            //Return the combined data
            return combinedData;
        }

        /// <summary>
        /// Sends a message to every client except the source.
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <param name="sender">Client that should not receive the message</param>
        //private void SendData(byte[] data, Client sender)
        //{
        //    foreach (Client c in client)
        //    {
        //        if (c != null && c != sender)
        //        {
        //            c.SendData(data);
        //        }
        //    }

        //    //Reset the writestream's position
        //    writeStream.Position = 0;
        //}

        private void SendData(byte[] data, Client sender)
        {
            sender.SendData(data);
            //Reset the writestream's position
            writeStream.Position = 0;
        }

        /// <summary>
        /// Sends data to all clients
        /// </summary>
        /// <param name="data">Data to send</param>
        //private void SendData(byte[] data)
        //{
        //    foreach (Client c in client)
        //    {
        //        if (c != null)
        //            c.SendData(data);
        //    }

        //    //Reset the writestream's position
        //    writeStream.Position = 0;
        //}
    }
}
