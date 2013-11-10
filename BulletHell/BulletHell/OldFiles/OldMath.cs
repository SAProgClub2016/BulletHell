using System;
using System.Net;
using System.Text;

namespace BulletHell.MathLib.Deprecated
{
    public delegate Pair<int> CoordTransformF(Matrix m, int r, int c);
    public delegate Pair<int> DimensionTransformF(int r, int c);

    public struct CoordTransform
    {
        public CoordTransformF CoordF;
        public DimensionTransformF DimF;
        public CoordTransform(CoordTransformF cf, DimensionTransformF df)
        {
            CoordF = cf;
            DimF = df;
        }
    }
}