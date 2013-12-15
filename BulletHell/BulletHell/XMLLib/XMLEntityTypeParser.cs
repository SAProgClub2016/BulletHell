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
    public class XMLEntityTypeParser : GameXMLParser<EntityType>
    {

        public XMLEntityTypeParser(XMLParser par) : base(par)
        {
        }
        protected override EntityType ParseNew(System.Xml.Linq.XElement el)
        {
            Trajectory traj = null;
            PhysicsShape shap = null;
            EntityBuilder buil = null;
            EntityClass ec = null;
            Drawable d = null;
            GraphicsStyle gs = null;
            BulletEmitter em = null;

            XElement @class = el.Element("Class");
            if (@class != null)
                buil = Parent.ParseEntityBuilder(@class);

            XElement entClass = el.Element("EntityClass");
            if (entClass != null)
                ec = Parent.ParseEntityClass(entClass);

            XElement trajectory = el.Element("Trajectory");
            if (trajectory != null)
                traj = Parent.ParseTrajectory(trajectory);

            XElement drawable = el.Element("Drawable");
            if (drawable != null)
                d = Parent.ParseDrawable(drawable);

            XElement graphicsStyle = el.Element("GraphicsStyle");
            if (graphicsStyle != null)
                gs = Parent.ParseGraphicsStyle(graphicsStyle);

            XElement bulletEmitter = el.Element("BulletEmitter");
            if (bulletEmitter != null)
                em = Parent.ParseBulletEmitter(bulletEmitter);

            XElement shape = el.Element("PhysicsShape");
            if (shape != null)
                shap = Parent.ParsePhysicsShape(shape);


            return new EntityType(traj, d, shap, ec, em, gs, buil);
        }
    }
}
