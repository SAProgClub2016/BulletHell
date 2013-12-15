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
using System.Xml.XPath;

namespace BulletHell.XMLLib
{
    class XMLPhysicsShapeParser : GameXMLParser<PhysicsShape>
    {

        public XMLPhysicsShapeParser(XMLParser par) : base(par)
        {
        }

        protected override PhysicsShape ParseNew(XElement el)
        {
            XAttribute att = el.Attribute("type");
            if(att==null)
            {
                return null;
            }
            string type = att.Value.ToLower();
            if(type == "ellipse")
            {
                XElement rad = el.Element("radius");
                if(rad==null)
                    throw new FormatException("Expected ellipse to have a radius");
                double r = (double)rad;
                int dim = 2;
                XElement dime = el.Element("dimension");
                if(dime!=null)
                    dim = (int) dime;
                return new Ellipse(r,dim);
            }
            if(type == "point")
            {
                int dim = 2;
                XElement dime = el.Element("dimension");
                if (dime != null)
                    dim = (int)dime;
                return new Point(dim);
            }
            return null;
        }
    }
}
