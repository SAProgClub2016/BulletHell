using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletHell.Time;
using System.Drawing;

namespace BulletHell.GameLib
{
    public class Game
    {
        private List<Entity> entities;

        private double t;
        public double Time
        {
            get
            {
                return t;
            }
            set
            {
                t = value;
                foreach (Entity e in entities)
                {
                    e.Time = t;
                }
            }
        }

        public Game()
        {
            entities = new List<Entity>();
        }

        public void Add(Entity e)
        {
            entities.Add(e);
        }

        public static Game operator +(Game g, Entity e)
        {
            g.entities.Add(e);
            return g;
        }
        public static Game operator -(Game g, Entity e)
        {
            g.entities.Remove(e);
            return g;
        }

        public void Draw(Graphics g)
        {
            foreach (Entity e in entities)
            {
                e.Draw(e.Position, g);
            }
        }
        public IEnumerable<Entity> Entities
        {
            get
            {
                return entities;
            }
        }
    }
}
