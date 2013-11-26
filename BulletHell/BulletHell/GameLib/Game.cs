using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Time;
using System.Drawing;
using BulletHell.MathLib;

namespace BulletHell.GameLib
{
    public class Game
    {
        private EntityManager entities;
        private BulletHell.Time.Timer gameTimer;
        private double t;
        private Func<double, double> timeFunc;
        private IntegrableFunction<double, double> timeRateFunc;
        private double curTimeRate;
        private bool paused = false;
        private double mostRenderedTime = -1;
        private double rewindLimit = 0;
        private int entCount;

        public int EntityCount
        {
            get
            {
                return entCount;
            }
        }

        public long ActualTime
        {
            get
            {
                return gameTimer.Time;
            }
        }
        public double CurrentTime
        {
            get
            {
                return timeFunc(ActualTime) / 150;
            }
        }
        // When paused will not return 0, but the background timerate
        public double CurrentTimeRate
        {
            get
            {
                return curTimeRate;
            }
            set
            {
                paused = false;
                curTimeRate = value;
                timeRateFunc = IntegrableFunction<double, double>.SplitAt(timeRateFunc, ActualTime, (PolyFunc<double>)curTimeRate, false);
                timeFunc = timeRateFunc.FI;
                //Console.WriteLine("Q {0}, {1}",curTimeRate);
            }
        }
        public void ResetTime()
        {
            gameTimer.Reset();
        }
        public double Time
        {
            get
            {
                return t;
            }
            set
            {
                t = value;
                if (t < rewindLimit)
                {
                    CurrentTimeRate = 0;
                }
                foreach (Entity e in Entities)
                {
                    e.Time = t;
                }
                if (t > mostRenderedTime)
                    mostRenderedTime = t;
            }
        }

        public Game()
        {
            entities = new AdvancedEntityManager(128,64,32,16,8,4,2,1,0.5);
            //entities = new ListEntityManager();
            gameTimer = new BulletHell.Time.Timer();
            curTimeRate = 1;
            timeRateFunc = new PolyFunc<double>(curTimeRate);
            timeFunc = timeRateFunc.FI;
        }

        public void Add(Entity e)
        {
            entities.Add(e);
        }

        public void Remove(Entity e)
        {
            entities.Remove(e);
        }

        public static Game operator +(Game g, Entity e)
        {
            g.Add(e);
            return g;
        }
        public static Game operator -(Game g, Entity e)
        {
            g.Remove(e);
            return g;
        }

        public void Draw(Graphics g)
        {
            entCount = 0;
            foreach (Entity e in Entities)
            {
                entCount++;
                if (Time > e.CreationTime && (Utils.IsZero(e.InvisibilityTime + 1) || e.InvisibilityTime > Time))   
                    e.Draw(e.Position, g);
            }
        }
        public IEnumerable<Entity> Entities
        {
            get
            {

                IEnumerable<Entity> ans = null;
                ans=entities.Entities(Time);
                return ans;
            }
        }

        public bool TogglePause()
        {
            Paused = !Paused;
            return paused;
        }
        public bool Paused
        {
            get
            {
                return paused;
            }
            set
            {
                if (paused != value)
                {
                    paused = value;
                    if (paused)
                    {
                        timeRateFunc = IntegrableFunction<double, double>.SplitAt(timeRateFunc, ActualTime, (PolyFunc<double>)0, false);
                    }
                    else
                    {
                        timeRateFunc = IntegrableFunction<double, double>.SplitAt(timeRateFunc, ActualTime, (PolyFunc<double>)curTimeRate, false);
                    }
                    timeFunc = timeRateFunc.FI;
                }
            }
        }

        public double MostRenderedTime { get { return mostRenderedTime; } }

        public IEnumerable<Entity> BulletShooters
        {
            get
            {
                return entities.BulletShooters(Time);
            }
        }
    }
}
