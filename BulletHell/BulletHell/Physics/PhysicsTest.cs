using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.Physics
{
    class PhysicsTest
    {
        public static void Main()
        {
            Particle p1 = new Particle(x=>3*x,y=>4*y);
            Particle p2 = new Particle(p1, x => 3 * Math.Cos(x), y => 3 * Math.Sin(y));
            for (double i = 0; i < 100; i++)
            {
                p1.Time = i;
                p2.Time = i;
                System.Console.WriteLine("p1: {0}", p1);
                System.Console.WriteLine("p2: {0}", p2);
            }
            Console.ReadKey();
        }
    }
}
