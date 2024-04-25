using BrickBreaker.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace BrickBreaker
{
    public partial class Form1 : Form
    {
        string x, y, width, height, id;
        List<Block> blocks = new List<Block>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Start the program centred on the Menu Screen
            MenuScreen ms = new MenuScreen();
            this.Controls.Add(ms);

            ms.Location = new Point((this.Width - ms.Width) / 2, (this.Height - ms.Height) / 2);

        }

        public void LevelReader()
        {
            XmlReader reader = XmlReader.Create("Resources/GenXML.xml");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    x = reader.ReadString();

                    reader.ReadToNextSibling("y");
                    y = reader.ReadString();

                    reader.ReadToNextSibling("width");
                    width = reader.ReadString();

                    reader.ReadToNextSibling("height");
                    height = reader.ReadString();

                    reader.ReadToNextSibling("id");
                    id = reader.ReadString();

                }

            }

        }
    }
}
