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
    public partial class LevelEditorPanel : UserControl
    {
        Level editing;
        
        public LevelEditorPanel(Level l)
        {
            InitializeComponent();

        }


    }
}
