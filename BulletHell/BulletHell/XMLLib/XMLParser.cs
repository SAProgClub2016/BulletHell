using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BulletHell.GameLib;
using BulletHell.GameLib.EntityLib;
using BulletHell.Physics.ShapeLib;

namespace BulletHell.XMLLib
{
    public class XMLParser
    {
        Dictionary<string, Trajectory> namedTrajectories;
        Dictionary<string, EntityType> namedEntityTypes;
        Dictionary<string, Func<double[], Trajectory>> trajectoryBuilders;
        Dictionary<string, Func<double[], PhysicsShape>> physicsShapeBuilders;
        Dictionary<string, Func<double[], EntityBuilder>> entityBuilders;

        


        public Trajectory ParseTrajectory(XElement tree)
        {
            return null;
        }
        public EntityType ParseEntityType(XElement tree)
        {
            EntityType ans=null;
            string name = null;
            XAttribute att = tree.Attribute("name");
            if(att!=null)
            {
                name=att.Value;
            }
            if(name!=null && namedEntityTypes.ContainsKey(name))
            {
                return namedEntityTypes[name];
            }




            if(name!=null)
            {
                namedEntityTypes[name]=ans;
            }
            return ans;
        }
    }
}
