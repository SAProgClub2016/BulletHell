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
    class XMLBuilderParser : GameXMLParser<EntityBuilder>
    {

        public XMLBuilderParser(XMLParser par)
            : base(par)
        {
        }

        protected override EntityBuilder ParseNew(XElement el)
        {
            XAttribute att = el.Attribute("type");
            if (att == null)
            {
                return null;
            }
            string type = att.Value.ToLower();
            if (type == "enemy")
            {
                int h = Enemy.DefaultHealth;
                ParseValue(el, "health", x => (int)x, ref h);
                int v = Enemy.DefaultValue;
                ParseValue(el, "value", x => (int)x, ref v);

                return Enemy.MakeEnemy(h,v);
            }
            if (type == "bullet")
            {
                int dmg = Bullet.DefaultDamage;
                ParseValue(el, "damage", x => (int)x, ref dmg);
                return Bullet.MakeBullet(dmg);
            }
            return null;
        }
    }
}
