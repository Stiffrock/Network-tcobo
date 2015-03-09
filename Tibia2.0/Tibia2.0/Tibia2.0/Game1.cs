using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace Tibia2._0
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        int my_id;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TcpClient client;
        Rectangle window_bounds;
        string IP = "127.0.0.1";
        float speed;
        byte[] readBuffer;
        int PORT = 1490;
        MemoryStream readStream, writeStream;
        bool once = true;
        bool i_am_host = true;
        int game_height = 720, game_width = 1280, min_height = 600, min_width = 800;
        int BUFFER_SIZE = 2048;
        BinaryReader reader;
        BinaryWriter writer;
        Intro intro;
        Texture2D intro_image;

        int direction_left = 0, direction_right = 1, direction_up = 2, direction_down = 3;

        MouseState ms, oms;
        KeyboardState ks, oks;

        List<Player> players = new List<Player>();
        Player player;

        TimeSpan time_out_timer;
        float time_out_seconds;

        enum GameState
        {
            intro,
            play
        }
        GameState gameState;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            readStream = new MemoryStream();
            writeStream = new MemoryStream();
            writer = new BinaryWriter(writeStream);
            reader = new BinaryReader(readStream);

            client = new TcpClient();
            client.NoDelay = true;

            time_out_seconds = 10f;
            time_out_timer = TimeSpan.FromSeconds(time_out_seconds);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            IsMouseVisible = true;

            graphics.PreferredBackBufferHeight = game_height;
            graphics.PreferredBackBufferWidth = game_width;
            window_bounds = Window.ClientBounds;
            Window.AllowUserResizing = true;
            graphics.ApplyChanges();

            spriteBatch = new SpriteBatch(GraphicsDevice);
            //Images here
            //Images End
            try
            {
                client.Connect(IP, PORT);
                readBuffer = new byte[BUFFER_SIZE];
                client.GetStream().BeginRead(readBuffer, 0, BUFFER_SIZE, StreamReceived, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Connecting to the server.");
                this.Exit();
            }
            intro = new Intro();
            intro.LoadContent(Content);
            SpriteSheet.LoadContent(Content);

            gameState = GameState.intro;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            oms = ms;
            ms = Mouse.GetState();
            oks = ks;
            ks = Keyboard.GetState();
            GameWindowControll();
            switch (gameState)
            {
                case GameState.intro:
                    intro.Update(gameTime, window_bounds, ms, oms, ks, oks);
                    if (intro.create_account == 1)
                    {
                        intro.create_account = 2;
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.CreateAccount);
                        writer.Write((string)intro.entered_name);
                        writer.Write((string)intro.entered_pass_1);
                        SendData(GetDataFromMemoryStream(writeStream));
                        time_out_timer = TimeSpan.FromSeconds(time_out_seconds);
                    }
                    else if (intro.login_account == 1)
                    {
                        intro.login_account = 2;
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.LoginAccount);
                        writer.Write((string)intro.entered_name);
                        writer.Write((string)intro.entered_pass_1);
                        SendData(GetDataFromMemoryStream(writeStream));
                        time_out_timer = TimeSpan.FromSeconds(time_out_seconds);
                    }
                    else if (intro.create_character == 1)
                    {
                        intro.create_character = 2;
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.CreateCharacter);
                        writer.Write((string)intro.entered_name);
                        writer.Write((string)intro.entered_pass_1);
                        writer.Write((string)intro.entered_charactername);
                        writer.Write((int)intro.gender);
                        SendData(GetDataFromMemoryStream(writeStream));
                        time_out_timer = TimeSpan.FromSeconds(time_out_seconds);
                    }
                    else if (intro.login_character == 1)
                    {
                        intro.login_character = 2;
                        writeStream.Position = 0;
                        writer.Write((byte)Protocol.LoginCharacter);
                        writer.Write((string)intro.entered_name);
                        writer.Write((string)intro.entered_pass_1);
                        writer.Write((string)intro.entered_charactername);
                        SendData(GetDataFromMemoryStream(writeStream));
                        time_out_timer = TimeSpan.FromSeconds(time_out_seconds);
                    }
                    break;
                case GameState.play:

                    writeStream.Position = 0;
                    writer.Write((byte)Protocol.Update);
                    writer.Write((string)intro.entered_name);
                    writer.Write((string)intro.entered_pass_1);
                    writer.Write((int)my_id);
                    SendData(GetDataFromMemoryStream(writeStream));

                    if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                        Move(direction_left);
                    else if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                        Move(direction_right);
                    else if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
                        Move(direction_up);
                    else if (ks.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
                        Move(direction_down);
                    
                    break;
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }


        public void Move(int direction)
        {
            writeStream.Position = 0;
            writer.Write((byte)Protocol.Move);
            writer.Write((string)intro.entered_name);
            writer.Write((string)intro.entered_pass_1);
            writer.Write((int)my_id);
            writer.Write((int)direction);
            SendData(GetDataFromMemoryStream(writeStream));
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            switch (gameState)
            {
                case GameState.intro:
                    spriteBatch.Begin();
                    intro.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameState.play:
                    spriteBatch.Begin();
                    player.Draw(spriteBatch);
                    for (int i = 0; i < players.Count; i++)
                    {
                        players[i].Draw(spriteBatch);
                    }
                    spriteBatch.End();
                    break;
            }
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }


        private void StreamReceived(IAsyncResult ar)
        {
            int bytesRead = 0;

            try
            {
                lock (client.GetStream())

                    bytesRead = client.GetStream().EndRead(ar);
            }
            catch (Exception ex)
            {
            }
            if (bytesRead == 0)
            {
                client.Close();
                return;
            }

            byte[] data = new byte[bytesRead];

            for (int i = 0; i < bytesRead; i++)
            {
                data[i] = readBuffer[i];
            }

            ProcessData(data);

            client.GetStream().BeginRead(readBuffer, 0, BUFFER_SIZE, StreamReceived, null);
        }

        private void ProcessData(byte[] data)
        {
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
                else if (p == Protocol.CreateAccountResult)
                {
                    intro.create_succes = reader.ReadBoolean();
                    intro.create_account = 3;
                }
                else if (p == Protocol.LoginAccountResult)
                {
                    intro.characters.Clear();
                    intro.login_succes = reader.ReadBoolean();
                    int characters = reader.ReadInt32();
                    for (int i = 0; i < characters; i++)
                    {
                        string character = reader.ReadString();
                        intro.characters.Add(character);
                    }
                    intro.character_boxes = new Rectangle[characters];
                    intro.login_account = 3;
                }
                else if (p == Protocol.CreateCharacterResult)
                {
                    intro.character_succes = reader.ReadBoolean();
                    intro.create_character = 3;
                }
                else if (p == Protocol.LoginCharacterResult)
                {
                    intro.login_character_succes = reader.ReadBoolean();
                    my_id = reader.ReadInt32();
                    if (intro.login_character_succes == true) 
                    {
                        gameState = GameState.play;
                        player = new Player(my_id, Vector3.Zero);
                    }
                    intro.login_character = 3;
                }
                else if (p == Protocol.Updated)
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        int id = reader.ReadInt32();
                        int posX = reader.ReadInt32();
                        int posY = reader.ReadInt32();
                        int posZ = reader.ReadInt32();
                        bool new_player = true;
                        foreach (Player pl in players)
                        {
                            if (pl.id == id)
                            {
                                pl.pos = new Vector3(posX, posY, posZ);
                                new_player = false;
                            }
                        }
                        if (new_player == true)
                        {
                            players.Add(new Player(id, new Vector3(posX, posY, posZ)));
                        }
                        
                    }
                    player.pos.X = reader.ReadInt32();
                    player.pos.Y = reader.ReadInt32();
                    player.pos.Z = reader.ReadInt32();
                }
                else if (p == Protocol.Moved)
                {
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

        }

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

        public void SendData(byte[] b)
        {
            //Try to send the data.  If an exception is thrown, disconnect the client
            try
            {
                lock (client.GetStream())
                {
                    client.GetStream().BeginWrite(b, 0, b.Length, null, null);
                }
            }
            catch (Exception e)
            {
            }
        }
        public void GameWindowControll()
        {
            window_bounds = Window.ClientBounds;
            if (window_bounds.Height < min_height)
            {
                graphics.PreferredBackBufferHeight = min_height;
                graphics.ApplyChanges();
            }
            if (window_bounds.Width < min_width)
            {
                graphics.PreferredBackBufferWidth = min_width;
                graphics.ApplyChanges();
            }
        }
    }
}
