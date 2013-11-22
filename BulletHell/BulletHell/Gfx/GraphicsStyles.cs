using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BulletHell.Gfx
{
    public class GraphicsStyle
    {
        public static GraphicsStyle DEFAULT = new GraphicsStyle(Brushes.White, Pens.Black);
        public Brush Brush { get; set; }
        public Pen Pen { get; set; }
        public GraphicsStyle(Brush b = null, Pen p = null)
        {

            Brush = b;
            Pen = p;
        }
    }
}
