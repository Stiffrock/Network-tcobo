using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tibia2._0
{
    static class SpriteSheet
    {
        public static Texture2D spriteSheet;

        public static void LoadContent(ContentManager Content)
        {
            spriteSheet = Content.Load<Texture2D>("solidblack");
        }
    }
}
