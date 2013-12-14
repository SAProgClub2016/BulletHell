﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using BulletHell.GameLib;
using BulletHell.GameLib.EntityLib;
using BulletHell.Physics.ShapeLib;
using BulletHell.Gfx;
using BulletHell.GameLib.EntityLib.BulletLib;
using System.Drawing;

namespace BulletHell.XMLLib
{
    public abstract class GameXMLParser<T>
    {
        Dictionary<string, T> namedT;
        Dictionary<string, Func<double[], T>> TBuilders;
        XMLParser parent;

        public XMLParser Parent { get { return parent; } }

        public GameXMLParser(XMLParser par)
        {
            parent = par;
            namedT = new Dictionary<string, T>();
            TBuilders = new Dictionary<string, Func<double[], T>>();
        }

        public T Parse(XElement el)
        {
            XAttribute name = el.Attribute("name");
            string n = null;
            if(name!=null)
            {
                n = (string)name;
            }
            if(n!=null && namedT.ContainsKey(n))
            {
                return namedT[n];
            }
            T ans = ParseNew(el);
            if (n != null)
                namedT[n] = ans;
            return ans;
        }

        protected abstract T ParseNew(XElement el);
    }

    public class XMLParser
    {
        GameXMLParser<EntityType> entType;
        GameXMLParser<Trajectory> trajs;
        GameXMLParser<EntityBuilder> entBuild;
        GameXMLParser<BulletEmitter> emitter;
        GameXMLParser<PhysicsShape> shaps;
        GameXMLParser<GraphicsStyle> styles;
        GameXMLParser<Brush> 

        public XMLParser()
        {
            entType = new XMLEntityTypeParser(this);
            styles = new XMLGraphicsStyleParser(this);
        }


        public Trajectory ParseTrajectory(XElement tree)
        {
            return trajs.Parse(tree);
        }
        public EntityType ParseEntityType(XElement entity)
        {
            return entType.Parse(entity);
        }

        public PhysicsShape ParsePhysicsShape(XElement shape)
        {
            return shaps.Parse(shape);
        }

        public BulletEmitter ParseBulletEmitter(XElement bulletEmitter)
        {
            return emitter.Parse(bulletEmitter);
        }

        public GraphicsStyle ParseGraphicsStyle(XElement graphicsStyle)
        {
            return styles.Parse(graphicsStyle);
        }

        public Drawable ParseDrawable(XElement drawable)
        {
            throw new NotImplementedException();
        }

        public EntityClass ParseEntityClass(XElement entClass)
        {
            throw new NotImplementedException();
        }

        public EntityBuilder ParseEntityBuilder(XElement @class)
        {
            throw new NotImplementedException();
        }
    }
}