using BulletHell.GameLib;
using BulletHell.GameLib.EntityLib;
using BulletHell.GameLib.EntityLib.BulletLib;
using BulletHell.Gfx;
using BulletHell.Physics.ShapeLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BulletHell.XMLLib
{
    public class XMLGraphicsStyleParser : GameXMLParser<GraphicsStyle>
    {
        public XMLGraphicsStyleParser(XMLParser par) : base(par)
        {
        }
        protected override GraphicsStyle ParseNew(XElement el)
        {
            Brush b = null;
            Pen p = null;

            XElement brush = el.Element("Brush");
            if (brush != null)
            {
                string col = brush.Value.Trim();
                b = Parent.BrushFromStringColor(col);
            }

            XElement pen = el.Element("Pen");
            if (pen != null)
            {
                string col = pen.Value.Trim();
                p = Parent.PenFromStringColor(col);
            }

            return new GraphicsStyle(b, p);
        }
    }
}
