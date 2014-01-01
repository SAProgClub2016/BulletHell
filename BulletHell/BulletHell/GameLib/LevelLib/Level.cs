using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.GameLib.EntityLib;
using System.Drawing;


namespace BulletHell.GameLib.LevelLib
{
    public interface Level : IEnumerable<Entity>
    {
        double Width { get; }
        double Height { get; }

        IEnumerable<Id> RenderOrder { get; }
    }
    public struct LevelData
    {
        EntitySpawner spawner;
        List<Id> renderOrder;
        double w, h;
        Color bgColor;


    }
}
