/*  Created by: 
 *  Project: Brick Breaker
 *  Date: 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Xml;
using BrickBreaker.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Resources;

namespace BrickBreaker
{
    public partial class GameScreen : UserControl
    {
        #region global values

        //player1 button control keys - DO NOT CHANGE
        Boolean leftArrowDown, rightArrowDown;

        // Game values
        int lives;
        int score;
        int blocksNum;
        int x, y, width, height, id;

        // Paddle and Ball objects
        Paddle paddle;
        Ball ball;

        // list of all blocks for current level
        List<Block> blocks = new List<Block>();

        // Brushes
        SolidBrush paddleBrush = new SolidBrush(Color.White);
        SolidBrush ballBrush = new SolidBrush(Color.Transparent);
        SolidBrush blockBrush = new SolidBrush(Color.Red);
        Pen ballPen = new Pen(Color.Black);

        Image dirtBlock = Properties.Resources.dirt;
        Image stoneBlock = Properties.Resources.stone;
        Image hearts = Properties.Resources.heartIcon2;
        Image snowBall = Properties.Resources.snowball;
        Image xpBar = Properties.Resources.xpBarEmpty;
        Image fullXpBar = Properties.Resources.xpBarFull;

        //Lives
        Rectangle life1 = new Rectangle(265, 330, 25, 25);
        Rectangle life2 = new Rectangle(315, 330, 25, 25);
        Rectangle life3 = new Rectangle(365, 330, 25, 25);
        Rectangle xpRect = new Rectangle(200, 370, 250, 5);
        Rectangle xpFullRect = new Rectangle(-300, 370, 250, 10);


        #endregion

        public GameScreen()
        {
            InitializeComponent();
            OnStart();
        }

        #region levelBuilder

        public void OnStart()
        {
            //set life counter
            lives = 3;

            //set all button presses to false.
            leftArrowDown = rightArrowDown = false;

            // setup starting paddle values and create paddle object
            int paddleWidth = 80;
            int paddleHeight = 20;
            int paddleX = ((this.Width / 2) - (paddleWidth / 2));
            int paddleY = (this.Height - paddleHeight) - 60;
            int paddleSpeed = 9;
            paddle = new Paddle(paddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.White);

            // setup starting ball values
            int ballX = this.Width / 2 - 10;
            int ballY = this.Height - paddle.height - 90;

            // Creates a new ball
            int xSpeed = 5;
            int ySpeed = 5;
            int ballSize = 20;


            ball = new Ball(ballX, ballY, xSpeed, ySpeed, ballSize);

            LevelReader(Form1.currentLevel);

            // start the game engine loop
            gameTimer.Enabled = true;
        }

        public void LevelReader(int levelNumber)
        {
            string path = "Resources/Level" + levelNumber + ".xml";
            XmlReader reader = XmlReader.Create(path);


            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    x = Convert.ToInt32(reader.ReadString());

                    reader.ReadToNextSibling("y");
                    y = Convert.ToInt32(reader.ReadString());

                    reader.ReadToNextSibling("width");
                    width = Convert.ToInt32(reader.ReadString());

                    reader.ReadToNextSibling("height");
                    height = Convert.ToInt32(reader.ReadString());

                    reader.ReadToNextSibling("id");
                    id = Convert.ToInt32(reader.ReadString());

                    Block newBlock = new Block(x, y, width, height, id);
                    newBlock.hp = Convert.ToInt16(Form1.blockData[id][0]);
                    ResourceManager rm = Resources.ResourceManager;
                    newBlock.image = (Image)rm.GetObject(Form1.blockData[id][2]);
                    //newBlock.image = (Image)rm.GetObject("oak_planks");
                    blocks.Add(newBlock);
                }

            }
        }

        #endregion

        #region inputKeys

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //player 1 button presses
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                case Keys.A:
                    leftArrowDown = true;
                    break;
                case Keys.D:
                    rightArrowDown = true;
                    break;
                default:
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            //player 1 button releases
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
                case Keys.A:
                    leftArrowDown = false;
                    break;
                case Keys.D:
                    rightArrowDown = false;
                    break;
                default:
                    break;
            }
        }

        #endregion



        private void gameTimer_Tick(object sender, EventArgs e)
        {
            Form1.globalTimer++;
            paddle.Move(Convert.ToUInt16(rightArrowDown) - Convert.ToUInt16(leftArrowDown), this);


            ball.Move();
            ball.PaddleCollision(paddle);

            if (ball.WallCollision(this))
            { //run wall collision and respond if the ball has touched the bottom

                lives--;

                // Moves the ball back to origin
                ball.x = ((paddle.x - (ball.radius)) + (paddle.width / 2));
                ball.y = (this.Height - paddle.height) - 85;

                if (lives == 0)
                {
                    gameTimer.Enabled = false;
                    OnEnd();
                }
            }


            //Check if ball has collided with any blocks
            for (int i = 0; i < blocks.Count; i++)
            {
                Block b = blocks[i];
                if (ball.BlockCollision(b))
                {
                    b.runCollision(); //should be switched to entirely, no lines below
                    if (b.hp < 1) { blocks.Remove(b); }

                }
            }

            //float xpBarMult = blocks.Count / blocksNum;    **BLOCKS NUM IS NEVER USED, THIS LOGIC WORKS FOR XP / GAME ENDING IF IT REPRESENTS TOTAL NUM OF BLOCKS
            //xpFullRect = new Rectangle (50, 367, (int)(1000 * xpBarMult), 50); //scale the xp bar mask based on the % of blocks remaining

            //if (xpBarMult == 0) { /*endGame*/ }

            Refresh();

        }

        public void OnEnd()
        {
            // Goes to the game over screen
            Form form = this.FindForm();
            MenuScreen es = new MenuScreen();

            es.Location = new Point((form.Width - es.Width) / 2, (form.Height - es.Height) / 2);

            form.Controls.Add(es);
            form.Controls.Remove(this);
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
            gameTimer.Interval = 10;
        }

        public void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // Draws paddle
            paddleBrush.Color = paddle.colour;
            //e.Graphics.FillRectangle(paddleBrush, paddle.x, paddle.y, paddle.width, paddle.height);
            Rectangle paddleRect = new Rectangle(paddle.x, paddle.y, paddle.width, paddle.height);
            e.Graphics.DrawImage(stoneBlock, paddleRect);

            // Draws blocks
            foreach (Block b in blocks)
            {
                //e.Graphics.FillRectangle(blockBrush, b.x, b.y, b.width, b.height);
                e.Graphics.DrawImage(b.image, b.x, b.y, b.width + 2, b.height + 2);
            }

            //Draw Hearts

            e.Graphics.DrawImage(hearts, life1);
            e.Graphics.DrawImage(hearts, life2);
            e.Graphics.DrawImage(hearts, life3);

            e.Graphics.DrawImage(xpBar, xpRect);
            e.Graphics.DrawImage(fullXpBar, xpFullRect);

            // Draws ball
            Rectangle ballRect = new Rectangle(ball.x, ball.y, 30, 30);
            e.Graphics.DrawImage(snowBall, ballRect);

        }
    }
}