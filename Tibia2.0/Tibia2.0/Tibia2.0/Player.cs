using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tibia2._0
{

    class Player
    {
        public Vector3 pos;
        public bool me = false;
        public int id;

        public Player(int id, Vector3 pos)
        {
            this.id = id;
            this.pos = pos;
        }

        public void Draw(SpriteBatch sp)
        {
            sp.Draw(SpriteSheet.spriteSheet, new Vector2(pos.X, pos.Y), Color.White);
        }
    }
}
