using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell;
using BulletHell.Physics;
using BulletHell.MathLib;
using BulletHell.Collections;

namespace BulletHell.GameLib.EntityLib.BulletLib
{
    public struct BulletEmission
    {
        public double Warmup;
        public double Cooldown;
        public Trajectory[] BulletPaths;
        public BulletStyle bSty;

        public BulletEmission(double w, double c, BulletStyle sty, params Trajectory[] trajs)
            : this(w,c,trajs,sty)
        {
        }

        public BulletEmission(double w, double c, Trajectory[] bulletTrajectory, BulletStyle sty)
        {
            this.Warmup = w;
            this.Cooldown = c;
            this.BulletPaths = bulletTrajectory;
            this.bSty = sty;
        }
    }
    public class BulletPattern
    {

        BulletEmission[] pattern;
        double cycleTime;
        EntityClass pc;


        public BulletPattern(EntityClass cls, params BulletEmission[] emissions)
            : this(emissions,cls)
        {
        }
        
        public BulletPattern(BulletEmission[] ps, EntityClass bulletClass)
        {
            pc = bulletClass;
            pattern = ps;
            cycleTime = 0;
            foreach(BulletEmission b in pattern)
            {
                cycleTime += b.Warmup + b.Cooldown;
            }
        }

        public LinkedList<Bullet> BulletsBetween(Particle p, double t1, double t2)
        {
            //Console.WriteLine("{0},{1}",t1,t2);
            LinkedList<Bullet> ans = new LinkedList<Bullet>();
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
                BulletEmission em = this[index - 1];
                foreach (Trajectory j in em.BulletPaths)
                {
                    ans.AddLast(em.bSty(t,j(t,x[0],x[1]),pc));
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
                    foreach (Trajectory j in b.BulletPaths)
                    {
                        //Console.WriteLine(k);
                        ans.AddLast(b.bSty(k,j(k,x[0],x[1]),pc));
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
    public class EmitterState
    {
        public readonly int CurrentPattern;
        public readonly double StartTime;
        private readonly double delta;
        public EmitterState(int pat, double time=0, double offset = 0)
        {
            CurrentPattern = pat;
            StartTime = time;
            delta = offset-StartTime;
        }
        public double GetRelativeTime(double t)
        {
            return t + delta;
        }
    }
    public class BulletEmitter
    {
        List<BulletPattern> myPats;
        EmitterState curState;

        LayeredLinkedList<EmitterState> changes;

        public BulletEmitter(params BulletPattern[] pats)
            : this(0,pats)
        {
        }

        public BulletEmitter(int defState,params BulletPattern[] pats)
        {
            myPats = new List<BulletPattern>(pats);
            curState = new EmitterState(defState);
            changes = new LayeredLinkedList<EmitterState>(0, st => st.StartTime, 1000, 100, 10, 1);
            changes.Add(curState);
        }



        public LinkedList<Bullet> BulletsBetween(Particle pos, double t1, double t2)
        {
            LinkedList<Bullet> ans = new LinkedList<Bullet>();
            EmitterState last = changes.LastBefore(t1);

            foreach(EmitterState next in changes.ElementsBetween(t1,t2))
            {
                if(last.CurrentPattern<0)
                {
                    last=next;
                    continue;
                }
                foreach(Bullet bull in myPats[last.CurrentPattern].BulletsBetween(pos,last.GetRelativeTime(Math.Max(t1,last.StartTime)),last.GetRelativeTime(next.StartTime)))
                {
                    ans.AddLast(bull);
                }
            }
            if (last.CurrentPattern >= 0)
            {
                foreach (Bullet bull in myPats[last.CurrentPattern].BulletsBetween(pos, last.GetRelativeTime(Math.Max(t1, last.StartTime)), last.GetRelativeTime(t2)))
                {
                    ans.AddLast(bull);
                }
            }

            return ans;
        }

    }
}