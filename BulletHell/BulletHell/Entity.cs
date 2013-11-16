using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Game;
using BulletHell;

namespace BulletHell.Game
{
    public class Entity
    {
        GraphicsObject i;
        BulletEmitter e;

        public GraphicsObject MyShape
        {
            get
            {
                return i;
            }
            set
            {
                i = value;
            }
        }
        public BulletEmitter Emitter
        {
            get
            {
                return e;
            }
        }

        public Entity(GraphicsObject sh, BulletEmitter e)
        {
            i = sh;
            this.e = e;
        }

    }
}
