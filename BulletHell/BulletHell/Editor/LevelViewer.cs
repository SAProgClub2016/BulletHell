using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BulletHell.GameLib.LevelLib;

namespace BulletHell.Editor
{
    public partial class LevelViewer : UserControl
    {
        Level viewing;


        public LevelViewer(Level l)
        {
            InitializeComponent();
            viewing = l;
        }

        private void LevelViewer_Load(object sender, EventArgs e)
        {

        }
    }
}
