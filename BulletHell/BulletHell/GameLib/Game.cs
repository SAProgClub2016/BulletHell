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
        private List<Entity> entities;
        private BulletHell.Time.Timer gameTimer;
        private double t;
        private Func<double, double> timeFunc;
        private IntegrableFunction<double, double> timeRateFunc;
        private double curTimeRate;
        private bool paused = false;



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
                foreach (Entity e in entities)
                {
                    e.Time = t;
                }
            }
        }

        public Game()
        {
            entities = new List<Entity>();
            gameTimer = new BulletHell.Time.Timer();
            curTimeRate = 1;
            timeRateFunc = new PolyFunc<double>(curTimeRate);
            timeFunc = timeRateFunc.FI;
        }

        public void Add(Entity e)
        {
            entities.Add(e);
        }

        public static Game operator +(Game g, Entity e)
        {
            g.entities.Add(e);
            return g;
        }
        public static Game operator -(Game g, Entity e)
        {
            g.entities.Remove(e);
            return g;
        }

        public void Draw(Graphics g)
        {
            foreach (Entity e in entities)
            {
                e.Draw(e.Position, g);
            }
        }
        public IEnumerable<Entity> Entities
        {
            get
            {
                return entities;
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
    }
}
