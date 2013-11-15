using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell;

namespace BulletHell.Game
{
    public struct BulletEmission
    {
        public double Warmup;
        public double Cooldown;
        public BulletTrajectory[] Bullets;
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

        public List<Bullet> BulletsBetween(double t1, double t2)
        {
            List<Bullet> ans = new List<Bullet>();
            double m = Math.Floor(t1 / cycleTime);
            double bas = m*cycleTime;
            int index = 0;
            double k;
            k = bas;
            while(k<t1)
            {
                BulletEmission b = this[index];
                k += b.Warmup + b.Cooldown;
                index++;
            }
            if (k- this[index - 1].Cooldown>t1)
            {
                foreach (BulletTrajectory j in this[index - 1].Bullets)
                {

                }
            }
            while(t2>0)
            {
                BulletEmission b = this[index];
                k += b.Warmup + b.Cooldown;
                
                index++;
                index %= pattern.Length;
            }
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