using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib.EntityLib
{
    public class EntityClass
    {
        public readonly Id PhysicsClass, RenderClass;
        public EntityClass(Id p, Id r)
        {
            PhysicsClass = p;
            RenderClass = r;
        }
        public EntityClass(Id i)
            : this(i,i)
        {
        }
    }
}
