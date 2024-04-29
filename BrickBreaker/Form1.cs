using System;
using BrickBreaker.Properties;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Xml;

namespace BrickBreaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Block ID & Level Data
        public string[][] blockData = new string[][]
        {
        new string [] {"Hp", "Weak To", "Png"},
        new string [] {"2", "Shovel", "Grass_Block"}, //Grass Block
        new string [] {"3", "Axe", "Oak_Wood_Log"}, //Oak Wood Log
        new string [] {"1", "Hoe", "Oak_Leaves"}, //Oak Leaves
        new string [] {"3", "Axe", "Oak_Planks"}, //Oak Planks
        new string [] {"5", "Pick", "Stone"}, //Stone
        new string [] {"4", "Pick", "Iron_Ore"}, //Iron Ore
        new string [] {"3", "Pick", "Gold_Ore"}, //Gold Ore
        new string [] {"2", "Pick", "Diamond_Ore"}, //Diamond Ore
        new string [] {"8", "Pick", "Obsidian"}, //Obsidian
        new string [] {"2", "Pick", "Netherack"}, //Netherack
        new string [] {"3", "Pick", "Quartz_Ore"}, //Quartz Ore
        new string [] {"4", "Pick", "Netherite"}, //Netherite
        new string [] {"10", "Sword", "End_Portal_Block"}, //End Portal Block
        new string [] {"5", "Pick", "Stone_Bricks"}, //Stone Bricks
        new string [] {"4", "Pick", "Endstone"}, //Endstone
        new string [] {"4", "Pick", "Endstone_Bricks"}, //Endstone Bricks
        new string [] {"2", "Shovel", "Sand"}, //Sand
        new string [] {"2", "Shovel", "Gravel"}, //Gravel
        new string [] {"3", "Pick", "Cobblestone"}, //Cobblestone
        new string [] {"4", "Pick", "Coal_Ore"}, //Coal Ore
        new string [] {"1", "Sword", "Lava"}, //Lava
        new string [] {"1", "Sword", "Water"}, //Water
        new string [] {"1", "Sword", "Nether_Portal"}, //Nether Portal
        new string [] {"12", "Sword", "Bedrock"}, //Bedrock
        new string [] {"6", "Sword", "Dragon_Egg"}, //Dragon Egg
        };

        public string[][] levelData = new string[][]
        {
        new string[] {"Player Sprite", "Background Image" }
        };
        #endregion

        public static int globalTimer;
        public static int tickDeltaTime = 10;

        #region helperFunctions

        public static bool isNegative(float num) { return (Math.Abs(num) != num); }

        public static int timeSincePoint(int checkedTime)
        {
            return checkedTime < globalTimer ? (globalTimer - checkedTime) : -1;  // returns -1 if checkedtime is in the future
        }

        public static float clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static void ChangeScreen(object sender, UserControl next)
        {

            Form f; // will either be the sender or parent of sender 

            if (sender is Form)
            {
                f = (Form)sender;
            }
            else
            {
                UserControl current = (UserControl)sender;
                f = current.FindForm();
                f.Controls.Remove(current);
            }

            next.Location = new Point((f.ClientSize.Width - next.Width) / 2, (f.ClientSize.Height - next.Height) / 2);
            f.Controls.Add(next);

            next.Focus();
        }

        public static bool IsWithinRange(float num, float lowerBound, float upperBound) { return num >= lowerBound && num <= upperBound; }

        public static float GreaterOf (float num1, float num2) { return num1 > num2 ? num1 : num2; }

        #endregion

        #region gameLogic

        public static int CheckCollision(Ball ball, Paddle rectObject, int collisionTimeStamp)
        { //returns 0 (no collision) or 1-4 (collides from the rectangle's top, right side, bottom and left side respectively)

            if (timeSincePoint(collisionTimeStamp) <= 8) { return 0; }

            Point ballCenter = new Point(ball.x + ball.radius, ball.y + ball.radius);
            Point rectCenter = new Point(rectObject.x + (rectObject.width / 2), rectObject.y + (rectObject.height / 2));

            bool CollidesX = Math.Abs(ballCenter.X - rectCenter.X) <= (ball.radius + rectObject.width / 2);
            bool CollidesY = Math.Abs(ballCenter.Y - rectCenter.Y) <= (ball.radius + rectObject.height / 2);

            if (!CollidesX || !CollidesY) { return 0; } //return false values if there is no chance of collision

            //prioritize collisions with the top / bottom of an object unless rectTop < ballY < rectBottom

            if (CollidesX && IsWithinRange(ballCenter.Y, rectObject.y, rectObject.y + rectObject.height)) {
                return (ball.x > rectObject.x) ? 2 : 4;
            }
            if (CollidesY) {
                return (ball.y > rectObject.y) ? 3 : 1;
            }

            return 0; //return 0 if no collision was detected
        }

        public static int CheckCollision(Ball ball, Block rectObject, int collisionTimeStamp)
        { //returns 0 (no collision) or 1-4 (collides from the rectangle's top, right side, bottom and left side respectively)

            if (timeSincePoint(collisionTimeStamp) <= 4) { return 0; }

            Point ballCenter = new Point(ball.x + ball.radius, ball.y + ball.radius);
            Point rectCenter = new Point(rectObject.x + (rectObject.width / 2), rectObject.y + (rectObject.height / 2));

            bool CollidesX = Math.Abs(ballCenter.X - rectCenter.X) <= (ball.radius + rectObject.width / 2);
            bool CollidesY = Math.Abs(ballCenter.Y - rectCenter.Y) <= (ball.radius + rectObject.height / 2);

            if (!CollidesX || !CollidesY) { return 0; } //return false values if there is no chance of collision

            //prioritize collisions with the top / bottom of an object unless rectTop < ballY < rectBottom

            if (CollidesX && IsWithinRange(ballCenter.Y, rectObject.y, rectObject.y + rectObject.height)) {
                return (ball.x > rectObject.x) ? 2 : 4;
            }

            if (CollidesY) {
                return (ball.y > rectObject.y) ? 3 : 1;
            }

            return 0; //return 0 if no collision was detected
        }

        #endregion



        #region XMLPacking

        string x, y, width, height, id;
        List<Block> blocks = new List<Block>();


        public void LevelReader()
        {
            XmlReader reader = XmlReader.Create("Resources/GenXML.xml");

            while (reader.Read()) { //exPLODE (thanks hark)
                if (reader.NodeType == XmlNodeType.Text) {

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

        #endregion



        private void Form1_Load(object sender, EventArgs e)
        {
            // Start the program centred on the Menu Screen
            MenuScreen ms = new MenuScreen();
            this.Controls.Add(ms);

            ms.Location = new Point((this.Width - ms.Width) / 2, (this.Height - ms.Height) / 2);
        }
    }
}
