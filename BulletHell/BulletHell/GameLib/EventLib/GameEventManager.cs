using BulletHell.Collections;
using BulletHell.MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib.EventLib
{
    public class GameEventManager
    {
        private double time;
        private const double TOLERANCE = 0.1;
        private Game g;
        private LayeredLinkedList<GameEvent> events;
        private bool rewinding;
        private double mostRenderedTime;
        private LinkedList<GameEvent> newAdds1;
        private bool na1;
        private LinkedList<GameEvent> newAdds2;


        public GameEventManager(Game game, params double[] intervals)
        {
            g = game;
            time = -1;
            events = new LayeredLinkedList<GameEvent>(0, ev => ev.Time, intervals);
            newAdds1 = new LinkedList<GameEvent>();
            newAdds2 = new LinkedList<GameEvent>();
        }

        public void Add(GameEvent e)
        {
            if (e.State == GameEventState.Undone)
                return;
            if (na1)
                newAdds1.AddLast(e);
            else
                newAdds2.AddLast(e);
        }

        public void Remove(GameEvent e)
        {
            throw new InvalidOperationException();
            //if (e.State != GameEventState.Undone)
            //    e.Undo(g);
            //events.Remove(e);
        }

        public bool Rewinding
        {
            get
            {
                return rewinding;
            }
            set
            {
                if (value == rewinding)
                    return;
                rewinding = value;
                if (!rewinding)
                {
                    LinkedList<GameEvent> rList = new LinkedList<GameEvent>();
                    foreach (GameEvent e in events.ElementsAfter(time).Reverse())
                    {
                        e.Undo(g);
                        rList.AddLast(e);
                    }
                    foreach (GameEvent e in rList)
                        events.Remove(e);
                    mostRenderedTime = Time;
                }
            }
        }
        public double Time
        {
            get
            {
                return time;
            }
            set
            {
                na1 = !na1;
                LinkedList<GameEvent> curFrame = (!na1 ? newAdds1 : newAdds2);
                if(curFrame.Count>0)
                    Rewinding=false;
                foreach(GameEvent e in curFrame)
                {
                    
            if(e.Time>MostRenderedTime)
            {
                mostRenderedTime = e.Time;
            }
                    if (e.Time < Time)
                    {
                        if (e.State == GameEventState.Unprocessed || e.State == GameEventState.Rewound)
                            e.Do(g);
                    }
                    events.Add(e);
                }
                curFrame.Clear();
                if (Utils.IsZero(value - time))
                    return;
                double oldTime = time;
                time = value;
                Pair<double> range = new Pair<double>(Math.Min(oldTime, time) - (oldTime<time?TOLERANCE:0), Math.Max(oldTime, time)+(oldTime<time?0:TOLERANCE));

                LinkedList<GameEvent> toRemove = new LinkedList<GameEvent>();

                if(oldTime<time)
                {
                    foreach(GameEvent e in events.ElementsBetween(range.x,range.y))
                    {
                        switch(e.State)
                        {
                            case GameEventState.Undone:
                                toRemove.AddLast(e);
                                break;
                            case GameEventState.Rewound:
                            case GameEventState.Unprocessed:
                                e.Do(g);
                                break;
                        }
                    }
                }
                else // rewinding
                {
                    Rewinding = true;
                    foreach(GameEvent e in events.ElementsBetweenBackwards(range.x,range.y))
                    {
                        switch(e.State)
                        {
                            case GameEventState.Unprocessed:
                                e.Do(g);
                                e.Rewind(g);
                                break;
                            case GameEventState.Processed:
                                e.Rewind(g);
                                break;
                            case GameEventState.Undone:
                                toRemove.AddLast(e);
                                break;
                        }
                    }
                }
                foreach (GameEvent e in toRemove)
                {
                    events.Remove(e);
                }
            }
        }

        public double MostRenderedTime
        {
            get
            {
                return mostRenderedTime;
            }
        }
    }
}
