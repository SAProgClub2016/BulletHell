using BulletHell.MathLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulletHell.CoordLib
{
    class ManagedCoords : Coords
    {
        Vector<int> aspect;

        public ManagedCoords(params int[] rat) : base(rat.Length, 1)
        {
            aspect = new Vector<int>(rat);
        }
    }
}
