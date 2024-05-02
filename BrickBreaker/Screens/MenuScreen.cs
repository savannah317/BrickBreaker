using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrickBreaker.Screens;

namespace BrickBreaker
{
    public partial class MenuScreen : UserControl
    {

        Image minecraftLogo = Properties.Resources.minecraftLogo;
        Rectangle titleRec;
        public MenuScreen()
        {
            InitializeComponent();

            titleRec = new Rectangle(0,-50,this.Right, 500);

            titleLabel.Font = new Font(Form1.pfc.Families[0], titleLabel.Font.Size);
            playButton.Font = levelButton.Font = exitButton.Font = new Font(Form1.pfc.Families[0], playButton.Font.Size);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            // Goes to the game screen
            GameScreen gs = new GameScreen(false);
            Form form = this.FindForm();

            form.Controls.Add(gs);
            form.Controls.Remove(this);

            gs.Location = new Point((form.Width - gs.Width) / 2, (form.Height - gs.Height) / 2);
        }

        private void MenuScreen_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(minecraftLogo, titleRec);
        }

        private void levelButton_Click(object sender, EventArgs e)
        {
            // Goes to the level screen
            LevelScreen ls = new LevelScreen();
            Form form = this.FindForm();

            form.Controls.Add(ls);
            form.Controls.Remove(this);

            ls.Location = new Point((form.Width - ls.Width) / 2, (form.Height - ls.Height) / 2);
        }
    }
}
