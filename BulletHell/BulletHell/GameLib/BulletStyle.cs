using BulletHell.Gfx;
using BulletHell.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib
{
    public delegate Bullet BulletStyle(double t, Particle path);

    public class BulletStyleManager
    {
        private Dictionary<string, BulletStyle> savedStyles;

        public BulletStyleManager()
        {
            savedStyles = new Dictionary<string, BulletStyle>();
        }

        public BulletStyle MakeStyle(string s, PhysicsShape p, GraphicsStyle sty)
        {
            BulletStyle ans = (t, path) =>
                {
                    return new Bullet(t, path, p.MakeDrawable(sty), p, null);
                };
            savedStyles[s] = ans;
            return ans;
        }
        public BulletStyle MakeStyle(string s, Drawable d, PhysicsShape ps)
        {
            BulletStyle ans = (t, path) =>
                {
                    return new Bullet(t, path, d, ps, null);
                };
            savedStyles[s] = ans;
            return ans;
        }

        public BulletStyle this[string s]
        {
            get
            {
                return savedStyles[s];
            }
        }
    }
}
