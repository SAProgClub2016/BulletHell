using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Time;
using System.Drawing;
using BulletHell.MathLib;
using BulletHell.MathLib.Function;
using BulletHell.Physics;
using BulletHell.GameLib.EntityLib;
using BulletHell.GameLib.EventLib;

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
        private MainChar mainChar;
        private PhysicsManager pm;
        private GameEventManager events;

        public GameEventManager Events
        {
            get
            {
                return events;
            }
            set
            {
                events = value;
            }
        }

        public EntityManager EntityManager
        {
            get
            {
                return entities;
            }
            set
            {
                entities = value;
            }
        }

        public PhysicsManager PhysicsManager
        {
            get
            {
                return pm;
            }
            set
            {
                pm = value;
            }
        }

        public MainChar Character
        {
            get
            {
                return mainChar;
            }
        }

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
                timeRateFunc = IntegrableFunction<double, double>.SplitAt(timeRateFunc, ActualTime, (PolyFunc<double,double>)curTimeRate, false);
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
                events.Time = Time;
                if (t > mostRenderedTime)
                    mostRenderedTime = t;
            }
        }

        public Game(MainChar m)
        {
            pm = new PhysicsManager(this);
            entities = //new AdvancedEntityManager(128,64,32,16,8,4,2,1,0.5);
                new AdvancedEntityManager(pm);
            events = new GameEventManager(this, 1000, 100, 10, 1);
            mainChar = m;
            //entities = new ListEntityManager();
            gameTimer = new BulletHell.Time.Timer();
            curTimeRate = 1;
            timeRateFunc = new PolyFunc<double,double>(curTimeRate);
            timeFunc = timeRateFunc.FI;
            Add(m);
        }

        public void Add(Entity e)
        {
            events.Add(e.Creation);
        }

        public void Remove(Entity e)
        {
            foreach(GameEvent ev in e.LifetimeEvents)
                events.Remove(ev);
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
                        timeRateFunc = IntegrableFunction<double, double>.SplitAt(timeRateFunc, ActualTime, (PolyFunc<double,double>)0, false);
                    }
                    else
                    {
                        timeRateFunc = IntegrableFunction<double, double>.SplitAt(timeRateFunc, ActualTime, (PolyFunc<double,double>)curTimeRate, false);
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
