using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BulletHell.GameLib.LevelLib;
using BulletHell.GameLib;
using BulletHell.GameLib.EntityLib;

namespace BulletHell.Editor
{
    public partial class LevelViewer : UserControl
    {
        Level viewing;
        Game g;

        public LevelViewer(Level l)
        {
            InitializeComponent();
            viewing = l;
            g = Game.LoadFromLevel(null, new Background(1280,720,40,Color.Black), l);
        }

        private void LevelViewer_Load(object sender, EventArgs e)
        {

        }
    }
}
