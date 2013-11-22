using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.MathLib;

namespace BulletHell.Physics
{
    public class Particle
    {
        private double time;
        private Vector<double> pos;
        private int dimension;
        
        public Particle Parent { get; set; }
        protected Func<double, Vector<double>> PosFunc;

        public double Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                pos = Position(time);
            }
        }
        public int Dimension
        {
            get
            {
                return dimension;
            }
        }
        public Vector<double> CurrentPosition
        {
            get
            {
                return pos;
            }
        }
        public Vector<double> CurrentVelocity(double dt)
        {
            return Velocity(Time, dt);
        }
        public Vector<double> Velocity(double t, double dt)
        {
            return (Position(t+(dt/2))-Position(t-(dt/2)))/dt;
        }

        public Vector<double> Position(double time)
        {
            Vector<double> origin;
            if (Parent == null)
                origin = new Vector<double>(Dimension);
            else
                origin = Parent.Position(time);
            return origin+PosFunc(time);
        }


        public Particle(Particle parent, params Func<double, double>[] components)
            : this(parent, Vector<double>.Aggregate<double>(components))
        {
        }
        public Particle(params Func<double, double>[] components)
            : this(null, Vector<double>.Aggregate<double>(components))
        {
        }
        public Particle(Particle parent, Func<double, Vector<double>> f)
        {
            Parent = parent;
            PosFunc = f;
            dimension = f(0).Dimension;
            Time = 0;
        }
        public Particle(Func<double, Vector<double>> f)
            : this(null, f)
        {
        }
        public override string ToString()
        {
            return "<Particle: pos: " + CurrentPosition + " >";
        }public void ChangeTrajectory(Func<double, Vector<double>> newPath, double t1, bool seal1 = true, double t2 = -1, bool seal2 = true)
        {

            Func<Func<double,Vector<double>>,double, Vector<double>> f1 = ((f,t) =>
                {
                    if(t<t1)
                    {
                        return f(t);
                    }
                    if(t2<t1 || t<t2)
                    {
                        if(seal1)
                        {
                            return newPath(t)-newPath(t1)+f(t1);
                        }
                        else
                        {
                            return newPath(t);
                        }
                    }
                    if (seal2)
                    {
                        if (seal1)
                        {
                            return f(t) - f(t2) + newPath(t2) - newPath(t1) + f(t1);
                        }
                        else
                        {
                            return f(t) - f(t2) + newPath(t2);
                        }
                    }
                    else
                    {
                        return f(t);
                    }
                });
            PosFunc = Utils.MakeClosure(PosFunc,f1);
        }
        public static Particle operator +(Particle p, Func<Vector<double>, Vector<double>> f)
        {
            Particle ans = new Particle(x => f(p.PosFunc(x)));
            ans.Time = p.Time;
            return ans;
        }
        public static Particle operator +(Func<double, double> f, Particle p)
        {
            Particle ans = new Particle(x => p.PosFunc(f(x)));
            ans.Time = p.Time;
            return ans;
        }
        public static Vector<double> MakeInside()
    }
    public class ParticleTest
    {
        public static void Main()
        {
            Particle p = new Particle(t => t, t => t);
            p.ChangeTrajectory(Vector<double>.Aggregate<double>(t => 2 * t, t => t),2);
            for (double d = 0; d < 5; d += 0.1)
            {
                p.Time = d;
                Console.WriteLine(p.CurrentPosition);
            }
            Console.ReadKey();
        }
    }
}
