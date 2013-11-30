using BulletHell.GameLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib.EventLib
{
    public delegate void GameEventRunner(Game g, GameEventState oldState);
    public struct GameEvent
    {
        public readonly double Time;
        private GameEventState state;

        private GameEventRunner doer, rewind, undo;
        
        public GameEvent(double time, GameEventRunner doer, GameEventRunner rwnd, GameEventRunner undo)
        {
            Time = time;
            this.doer = doer;
            this.rewind = rwnd;
            this.undo = undo;
            state = GameEventState.Unprocessed;
        }

        public GameEventState State
        {
            get
            {
                return state;
            }
        }
        public void Do(Game g)
        {
            if (state == GameEventState.Undone)
                throw new InvalidOperationException();
            if (state == GameEventState.Processed)
                return;
            doer(g, state);
            state = GameEventState.Processed;
        }
        public void Rewind(Game g)
        {
            switch(state)
            {
                case GameEventState.Undone:
                    throw new InvalidOperationException();
                case GameEventState.Unprocessed:
                    return;
                case GameEventState.Rewound:
                    return;
                case GameEventState.Processed:
                    rewind(g, state);
                    break;
            }
            state = GameEventState.Rewound;
        }
        public void Undo(Game g)
        {
            switch(state)
            {
                case GameEventState.Undone:
                    return;
                case GameEventState.Unprocessed:
                    return;
                default:
                    undo(g, state);
                    break;
            }
            state = GameEventState.Undone;
        }
    }

    public enum GameEventState
    {
        Unprocessed, Rewound, Undone, Processed
    }
}
