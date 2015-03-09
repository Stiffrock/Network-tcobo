using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    static class ServerConfig
    {
        public static int BufferSize = 2048;
        public static int Port = 1490;
        public static byte NewPlayerProtocol = 1;
        public static byte DisconnectedPlayerProtocol = 0;
        public static int MaxPlayers = 1000;
    }
}
