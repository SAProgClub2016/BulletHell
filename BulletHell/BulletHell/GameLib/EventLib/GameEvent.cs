using BulletHell.GameLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletLib.GameLib.EventLib
{
    public delegate void GameEventRunner(Game g);
    public struct GameEvent
    {
        public readonly double Time;
        public readonly GameEventRunner Do, Rewind, Undo;
        public GameEvent(double time, GameEventRunner doer, GameEventRunner rwnd, GameEventRunner undo)
        {
            Time = time;
            Do = doer;
            Rewind = rwnd;
            Undo = undo;
        }
    }
}
