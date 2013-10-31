using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BulletHell.Time;

namespace BulletHell
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public void GameLoop()
        {
            BulletHell.Time.Timer timer = new BulletHell.Time.Timer();
            while (this.Created)
            {
                timer.Reset();
                GameLogic();
                RenderScene();
                Application.DoEvents();
                while (timer.Time < 50) ;
            }
        }
        private void RenderScene()
        {
        }
        private void GameLogic()
        {
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
