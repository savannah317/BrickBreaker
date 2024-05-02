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
        int totalNumOfBlocks;
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

        //Lives - this should probably be changed for some dynamic list of lives rather than 3 individual objects
        Rectangle life1 = new Rectangle(265, 330, 25, 25);
        Rectangle life2 = new Rectangle(315, 330, 25, 25);
        Rectangle life3 = new Rectangle(365, 330, 25, 25);
        Rectangle xpRect = new Rectangle(200, 370, 250, 5);
        Rectangle xpFullRect = new Rectangle(-300, 370, 250, 10);


        #endregion

        public GameScreen() {
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

            LevelReader(1);

            // start the game engine loop
            gameTimer.Enabled = true;
        }

        public void LevelReader(int levelNumber) {
            string path = "Resources/Level" + levelNumber + ".xml";
            XmlReader reader = XmlReader.Create(path);


            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Text) {
                    x = Convert.ToInt32(reader.ReadString());

                    reader.ReadToNextSibling("y");
                    y = Convert.ToInt32(reader.ReadString());

                    reader.ReadToNextSibling("width");
                    width = Convert.ToInt32(reader.ReadString());

                    reader.ReadToNextSibling("height");
                    height = Convert.ToInt32(reader.ReadString());

                    reader.ReadToNextSibling("id");
                    id = Convert.ToInt32(reader.ReadString());

                    
                    Block newBlock = new Block(x, y, width + 2, height + 2, id);

                    if (newBlock.heldPowerID == 0) {
                        //randomly generate a power some of the time
                    }

                    blocks.Add(newBlock);
                }
            }
            totalNumOfBlocks = blocks.Count;
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



        private void gameTimer_Tick(object sender, EventArgs e) {
            Form1.globalTimer++;
            paddle.Move(Convert.ToUInt16(rightArrowDown) - Convert.ToUInt16(leftArrowDown), this);

            ball.Move();
            ball.PaddleCollision(paddle);

            if (ball.WallCollision(this)) { //run wall collision and respond if the ball has touched the bottom
                lives--;

                // Moves the ball back to origin
                ball.x = ((paddle.x - (ball.radius * 2)) + (paddle.width / 2));
                ball.y = (this.Height - paddle.height + ball.radius) - 85;

                if (lives == 0) {
                    gameTimer.Enabled = false;
                    OnEnd();
                }
            }

            //Check if ball has collided with any blocks
            for (int i = 0; i < blocks.Count; i++) {
                Block b = blocks[i];
                if (ball.BlockCollision(b)) {
                    //blocks.Remove(b);
                    b.runCollision(blocks); //unused but should be switched to, rather than relying on the above line
                }
            }

            float xpBarMult = totalNumOfBlocks / blocks.Count;    //BLOCKS NUM IS NEVER USED, THIS LOGIC WORKS FOR XP / GAME ENDING IF IT REPRESENTS TOTAL NUM OF BLOCKS
            xpFullRect = new Rectangle (50, 367, (int)(100 * xpBarMult), 50); //scale the xp bar mask based on the % of blocks remaining

            //if (xpBarMult == 0) { /*endGame*/ }

            Refresh();
        }

        public void OnEnd() {
            Form1.ChangeScreen(this, new MenuScreen()); //should make an actual 'game over' display
        }

        private void GameScreen_Load(object sender, EventArgs e) {
            gameTimer.Interval = 10;
        }

        public void GameScreen_Paint(object sender, PaintEventArgs e) {
            // Draws paddle
            Rectangle paddleRect = new Rectangle(paddle.x, paddle.y, paddle.width, paddle.height);
            e.Graphics.DrawImage(stoneBlock, paddleRect);

            // Draws blocks
            foreach (Block b in blocks) {
                e.Graphics.DrawImage(dirtBlock, b.x, b.y, b.width, b.height);
            }

            //Draw Hearts

            e.Graphics.DrawImage(hearts, life1);
            e.Graphics.DrawImage(hearts, life2);
            e.Graphics.DrawImage(hearts, life3);

            e.Graphics.DrawImage(xpBar, xpRect);
            e.Graphics.DrawImage(fullXpBar, xpFullRect);

            // Draws ball
            //e.Graphics.FillRectangle(ballBrush, ball.x, ball.y, ball.size, ball.size);
            Rectangle ballRect = new Rectangle(ball.x + ball.radius, ball.y + ball.radius, 30, 30);
            e.Graphics.DrawImage(snowBall, ballRect);

        }
    }
}
