using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;

namespace BulletHell.Game
{
    public class Bullet : Entity
    {
        public Bullet(GraphicsObject sh)
            : base(sh, null)
        {
        }
    }
}
