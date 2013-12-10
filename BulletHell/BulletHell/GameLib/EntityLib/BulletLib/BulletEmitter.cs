using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell;
using BulletHell.Physics;
using BulletHell.MathLib;
using BulletHell.Collections;
using BulletHell.GameLib.EventLib;

namespace BulletHell.GameLib.EntityLib.BulletLib
{
    public struct BulletEmission
    {
        public double Warmup;
        public double Cooldown;
        public Trajectory[] BulletPaths;
        public EntityType EntType;

        public BulletEmission(double w, double c, EntityType eType, params Trajectory[] trajs)
            : this(w,c,trajs, eType)
        {
        }

        public BulletEmission(double w, double c, Trajectory[] bulletTrajectory, EntityType eType)
        {
            this.Warmup = w;
            this.Cooldown = c;
            this.BulletPaths = bulletTrajectory;
            this.EntType = eType;
        }
        public IEnumerable<Entity> MakeBullets(double t, double x, double y)
        {
            foreach(Trajectory j in BulletPaths)
            {
                yield return EntType.MakeEntity(t, j(t, x, y));
            }
        }
    }
    public class BulletPattern
    {

        BulletEmission[] pattern;
        double cycleTime;

        public BulletPattern(params BulletEmission[] ps)
        {
            pattern = ps;
            cycleTime = 0;
            foreach(BulletEmission b in pattern)
            {
                cycleTime += b.Warmup + b.Cooldown;
            }
        }

        public LinkedList<Entity> BulletsBetween(Particle p, double t1, double t2, double offset=0)
        {
            t1 += offset;
            t2 += offset;
            //Console.WriteLine("{0},{1}",t1,t2);
            LinkedList<Entity> ans = new LinkedList<Entity>();
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
                double t = k - this[index - 1].Cooldown-offset;
                Vector<double> x = p.Position(t); 
                BulletEmission em = this[index - 1];
                foreach (Entity e in em.MakeBullets(t,x[0],x[1]))
                {
                    ans.AddLast(e);
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
                    double t = k - offset;
                    Vector<double> x = p.Position(t);
                    foreach (Entity e in b.MakeBullets(t,x[0],x[1]))
                    {
                        //Console.WriteLine(k);
                        ans.AddLast(e);
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

        public double GetRelativeOffset { get { return delta; } }
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

        public GameEvent ChangePattern(double time, int pat, double offset=0)
        {
            EmitterState es=null;
            return new GameEvent(time,
                (g, st) => {
                    if(st==GameEventState.Unprocessed)
                    {
                        es=new EmitterState(pat, time, offset);
                        changes.Add(es);
                    }
                },
                GameEvent.DoNothing,
                (g, st) => {
                    if(st!=GameEventState.Unprocessed)
                    {
                        if (es != null)
                            changes.Remove(es);
                    }
                });
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
                foreach(Bullet bull in myPats[last.CurrentPattern].BulletsBetween(pos,Math.Max(t1,last.StartTime),next.StartTime,last.GetRelativeOffset))
                {
                    ans.AddLast(bull);
                }
            }
            if (last.CurrentPattern >= 0)
            {
                foreach (Bullet bull in myPats[last.CurrentPattern].BulletsBetween(pos, Math.Max(t1, last.StartTime), t2, last.GetRelativeOffset))
                {
                    ans.AddLast(bull);
                }
            }

            return ans;
        }

    }
}