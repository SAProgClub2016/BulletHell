﻿using BulletHell.Collections;
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
        private static readonly double TOLERANCE = 0.1;
        private Game g;
        private LayeredLinkedList<GameEvent> events;
        private bool rewinding;
        private LookupLinkedListSet<GameEvent> unprocessed;

        public GameEventManager(Game game, params double[] intervals)
        {
            g = game;
            time = -1;
            events = new LayeredLinkedList<GameEvent>(0, ev => ev.Time, intervals);
            unprocessed = new LookupLinkedListSet<GameEvent>();
        }

        public void Add(GameEvent e)
        {
            if (e.State == GameEventState.Undone)
                return;
            if(e.Time<Time)
            {
                if (e.State == GameEventState.Unprocessed || e.State == GameEventState.Rewound)
                    e.Do(g);
            }
            events.Add(e);
            if (e.State == GameEventState.Unprocessed)
                unprocessed.Add(e);
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
                if(!rewinding)
                {
                    LinkedList<GameEvent> rList = new LinkedList<GameEvent>();
                    foreach(GameEvent e in events.ElementsAfter(time))
                    {
                        e.Undo(g);
                        rList.AddLast(e);
                    }
                    foreach (GameEvent e in rList)
                        events.Remove(e);
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
                //Console.WriteLine(rewinding);
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
                        //if (range.x > e.Time || range.y < e.Time)
                        //    throw new InvalidOperationException();
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

                        if (range.x > e.Time || range.y < e.Time)
                            throw new InvalidOperationException();
                        switch(e.State)
                        {
                            case GameEventState.Unprocessed:
                                e.Do(g);
                                e.Rewind(g);
                                break;
                                //throw new InvalidOperationException();
                            case GameEventState.Processed:
                                e.Rewind(g);
                                break;
                            case GameEventState.Undone:
                                toRemove.AddLast(e);
                                break;
                        }
                        //Console.WriteLine(e.State);
                    }
                }
                foreach (GameEvent e in toRemove)
                {
                    events.Remove(e);
                }
                
                LinkedList<GameEvent> removes = new LinkedList<GameEvent>();
                    
                foreach (GameEvent e in unprocessed)
                {
                    if (oldTime > time)
                        return;
                    if (e.State != GameEventState.Unprocessed)
                        removes.AddLast(e);
                    else if (range.x < e.Time && e.Time < range.y)
                        throw new InvalidOperationException();
                    else if (e.Time < Time)
                        throw new InvalidOperationException();
                }
                foreach(GameEvent e in removes)
                {
                    unprocessed.RemovePermanently(e);
                }
            }
        }
    }
}