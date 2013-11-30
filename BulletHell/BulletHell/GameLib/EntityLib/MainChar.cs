using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Gfx;
using BulletHell.Physics;
using BulletHell.MathLib.Function;
using BulletHell.MathLib;

namespace BulletHell.GameLib.EntityLib
{
    public class MainChar : Entity
    {
        double vx = 0, vy = 0,x0,y0;
        double xc = 0, yc = 0;
        IntegrableFunction<double, double> vxf, vyf;
        Func<double, double> xp, yp;
        double speed;

        public double Speed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        private void SetVX(double time, double vx)
        {
            this.vx = vx;
            vxf = IntegrableFunction<double, double>.SplitAt(vxf, time, (PolyFunc<double, double>)vx, false);
            xp = t => x0 + vxf.FI(t);
        }
        private void SetVY(double time, double vy)
        {
            this.vy = vy;
            vyf = IntegrableFunction<double, double>.SplitAt(vyf, time, (PolyFunc<double, double>)vy, false);
            yp = t => y0 + vyf.FI(t);
        }
        public double XComp
        {
            get
            {
                return xc;
            }
            set
            {
                xc = value;
                Vector<double> v=new Vector<double>(xc, yc);
                if (Utils.IsZero(v.Length2))
                    SetVX(Position.Time, 0);
                else
                {
                    v.Length = Speed;
                    SetVX(Position.Time, v[0]);
                    SetVY(Position.Time, v[1]);
                }
            }
        }
        public double YComp
        {
            get
            {
                return yc;
            }
            set
            {
                yc = value;
                Vector<double> v = new Vector<double>(xc, yc);
                if (Utils.IsZero(v.Length2))
                    SetVY(Position.Time, 0);
                else
                {
                    v.Length = Speed;
                    SetVX(Position.Time, v[0]);
                    SetVY(Position.Time, v[1]);
                }
            }
        }
        public MainChar(Drawable d, double x0, double y0, double s = 20, GraphicsStyle g = null) : base(0,null, d, new Point(2), new PhysicsClass("MainChar"), null, g)
        {
            speed = s;
            this.x0 = x0;
            this.y0 = y0;
            vxf = new PolyFunc<double, double>(0);
            vyf = new PolyFunc<double, double>(0);
            xp = t => x0;
            yp = t => y0;
            Position = new Particle(t => xp(t), t => yp(t));
        }
    }
}
