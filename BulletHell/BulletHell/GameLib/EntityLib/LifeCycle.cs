using BulletHell.GameLib.EventLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.GameLib.EntityLib
{
    public delegate IEnumerable<GameEvent> LifeCycle(Entity e);
}
