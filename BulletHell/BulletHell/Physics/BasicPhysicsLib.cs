using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.MathLib;

namespace BulletHell.Physics
{
    class Particle
    {
        private double time;
        private Vector<double> pos;
        private int dimension;
        
        public Particle Parent { get; set; }
        Func<double, Vector<double>> PosFunc;

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
        }
    }
}
