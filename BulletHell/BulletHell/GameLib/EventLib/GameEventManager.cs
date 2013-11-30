using BulletHell.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletLib.GameLib.EventLib
{
    public class GameEventManager
    {
        LayeredLinkedList<GameEvent> events;
        public GameEventManager(params double[] intervals)
        {
            events = new LayeredLinkedList<GameEvent>(0, ev => ev.Time, intervals);
        }
    }
}
