using BulletHell.GameLib;
using BulletHell.GameLib.EntityLib;
using BulletHell.GameLib.EntityLib.BulletLib;
using BulletHell.Gfx;
using BulletHell.Physics.ShapeLib;
using System;
using System.Collections.Generic;
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

        }
    }
}
