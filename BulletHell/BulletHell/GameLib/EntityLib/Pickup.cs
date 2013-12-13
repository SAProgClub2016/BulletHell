using BulletHell.GameLib.EntityLib.BulletLib;
using BulletHell.GameLib.EventLib;
using BulletHell.Gfx;
using BulletHell.Physics;
using BulletHell.Physics.ShapeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib.EntityLib
{
    public delegate GameEvent Effect(Game g, double t);

    public static class Effects
    {
        public static Effect NoEffect
        {
            get { return (g,t) => null; }
        }
        public static Effect MakeQuantityChanger<T>(ref T val, T newVal)
        {
            T oldVal = val;
            return (g,t) =>
                {
                    new GameEvent(t,
                        (game,state)=>
                            {
                                if(state==GameEventState.Undone||state==GameEventState.Rewound)
                                    val = newVal;
                            },
                        (game,state)=>
                            {
                                if(state==GameEventState.Processed)
                                    val = oldVal;
                            },
                        
                        (game,state)=>
                            {
                                if(state==GameEventState.Undone||state==GameEventState.Rewound)
                                    val = newVal;
                            })
                };
        }
    }

    public class Pickup : Entity
    {
        Effect effect;
        public Pickup(double cTime, Particle pos, Drawable d, PhysicsShape physS, EntityClass pc, Effect eff)
            : base(cTime, pos, d, physS, pc, null, null)
        {
        }
    }
}
