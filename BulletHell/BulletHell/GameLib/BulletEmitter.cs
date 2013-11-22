using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell;
using BulletHell.Physics;
using BulletHell.MathLib;

namespace BulletHell.GameLib
{
    public struct BulletEmission
    {
        public double Warmup;
        public double Cooldown;
        public BulletTrajectory[] Bullets;

        public BulletEmission(double w, double c, BulletTrajectory[] bulletTrajectory)
        {
            this.Warmup = w;
            this.Cooldown = c;
            this.Bullets = bulletTrajectory;
        }
    }
    public class BulletEmitter
    {
        BulletEmission[] pattern;
        double cycleTime;

        public BulletEmitter(BulletEmission[] ps)
        {
            pattern = ps;
            cycleTime = 0;
            foreach(BulletEmission b in pattern)
            {
                cycleTime += b.Warmup + b.Cooldown;
            }
        }

        public List<Bullet> BulletsBetween(Particle p, double t1, double t2)
        {
            //Console.WriteLine("{0},{1}",t1,t2);
            List<Bullet> ans = new List<Bullet>();
            double m = Math.Floor(t1 / cycleTime);
            //Console.WriteLine(m);
            //Console.WriteLine(cycleTime);
            double bas = m*cycleTime;
            int index = 0;
            double k;
            k = bas;
            //Console.WriteLine("BULLETSBETWEEN {0}", k);
            while(k<t1)
            {
                BulletEmission b = this[index];
                k += b.Warmup + b.Cooldown;
                index++;
            }
            //Console.WriteLine(k);
            if (index != 0 && k - this[index - 1].Cooldown - t1 > 0.001 && k - this[index - 1].Cooldown - t2 < 0.001)
            {
                double t = k - this[index - 1].Cooldown;
                Vector<double> x = p.Position(t); 
                foreach (BulletTrajectory j in this[index - 1].Bullets)
                {
                    ans.Add(j(t,x[0],x[1]));
                    //Console.WriteLine(k - this[index - 1].Cooldown);
                }
            }
            index %= pattern.Length;
            while(k<t2)
            {
                BulletEmission b = this[index];
                k += b.Warmup;
                if (k < t2)
                {
                    Vector<double> x = p.Position(k);
                    foreach (BulletTrajectory j in b.Bullets)
                    {
                        Console.WriteLine(k);
                        ans.Add(j(k,x[0],x[1]));
                    }
                }
                k += b.Cooldown;
                index++;
                index %= pattern.Length;
            }
            //Console.WriteLine("LENGTHASDF: {0}", ans.Count);
            return ans;
        }
        public BulletEmission this[int index]
        {
            get
            {
                return pattern[index];
            }
        }
    }

}