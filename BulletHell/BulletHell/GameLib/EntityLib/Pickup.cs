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
        public static Effect MakeQuantityChanger<T>(Value<T> val, T newVal)
        {
            T oldVal = val.Value;
            return (g, t) =>
            {
                return new GameEvent(t,
                    (game, state) =>
                    {
                        if (state == GameEventState.Undone || state == GameEventState.Rewound)
                            val.Value = newVal;
                    },
                    (game, state) =>
                    {
                        if (state == GameEventState.Processed)
                            val.Value = oldVal;
                    },

                    (game, state) =>
                    {
                        if (state == GameEventState.Processed)
                            val.Value = newVal;
                    });
            };
        }
        public static Effect MakeQuantityChanger<T>(Value<T> val, Func<T,T> transform)
        {
            return MakeQuantityChanger<T>(val, transform(val.Value));
        }
    }

    public class Pickup : Entity
    {
        Effect effect;
        public Pickup(double cTime, Particle pos, Drawable d, PhysicsShape physS, EntityClass pc, Effect eff)
            : base(cTime, pos, d, physS, pc, null, null)
        {
            effect = eff ?? Effects.NoEffect;
        }
    }
}
