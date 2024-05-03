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
using System.IO;
using BrickBreaker.Screens;

namespace BrickBreaker
{
    public partial class GameScreen : UserControl
    {
        #region global values

        //player1 button control keys - DO NOT CHANGE
        Boolean leftArrowDown, rightArrowDown;

        // Game values
        int lives;
        int x, y, width, height, id;
        public bool isPaused = false;
        int right;

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
        Image emptyXpBar = Properties.Resources.xpBarEmpty;
        Image fullXpBar = Properties.Resources.xpBarFull;
        Rectangle xpBarRegion;


        //Lives
        List<Rectangle> lifeRectangles = new List<Rectangle>
        {
        new Rectangle(10, 10, 35, 35),
        new Rectangle(60, 10, 35, 35),
        new Rectangle(110, 10, 35, 35)
        };
        Rectangle xpRect, xpFullRect;


        #endregion

        int blocksNum;
        ResourceManager rm = Resources.ResourceManager;

        List<Powerup> activePowerups = new List<Powerup>();
        List<Powerup> fallingPowerups = new List<Powerup>();

        //Displaying Powerups
        Font powerupFont = new Font(DefaultFont.Name, 10);
        int powerUpImageSize = 40;
        int powerUpOffset = 10;

        PointF timeDisplayPoint;
        int TimeLimit;
        int currentTime;
        const int MAX_FONT_SIZE = 40;
        const int MIN_FONT_SIZE = 15;
        double fontIncrease;
        public GameScreen(bool immidiateStart)
        {
            InitializeComponent();
            OnStart(immidiateStart);
        }

        #region levelBuilder

        public void OnStart(bool immidiateStart)
        {
            //Start immidiately, or give the player a StartLevelScreen first.
            if (immidiateStart) { gameTimer.Enabled = true; }
            else
            {
                StartLevelScreen sls = new StartLevelScreen(this);
                sls.Location = new Point((this.Width - sls.Width) / 2,10);
                this.Controls.Add(sls);
            }

            right = this.Right;
            timeDisplayPoint = new PointF(right / 2, this.Bottom - 30);
            xpRect = xpFullRect = xpBarRegion = new Rectangle(0, this.Bottom - 35, this.Right, 35);

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
            resetBall();
            LevelReader(Form1.currentLevel);
        }



        public void LevelReader(int levelNumber)
        {
            int totalLevelHp = 0;
            Random random = new Random();

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


                    //Create a new block prototype
                    Block newBlock = new Block(x, y, width, height, id);
                    newBlock.hp = Convert.ToInt16(Form1.blockData[id][0]);

                    totalLevelHp += newBlock.hp;

                    //Get the correct image
                    newBlock.image = (Image)rm.GetObject(Form1.blockData[id][2]);

                    //Find if the block should contain powerups
                    if ((double)random.Next(0, 11) <= Convert.ToDouble(Form1.blockData[id][3]))
                    {
                        int powerupID = Convert.ToInt16(Form1.blockData[id][4]);
                        Powerup newPowerup = new Powerup(powerupID, newBlock.x + (newBlock.width / 2), newBlock.y + (newBlock.height / 2));

                        newPowerup.image = (Image)rm.GetObject(Form1.powerupData[powerupID][2]);

                        newBlock.powerupList.Add(newPowerup);
                    }

                    blocks.Add(newBlock);
                }
            }


            blocksNum = blocks.Count;
            TimeLimit = currentTime = totalLevelHp * 60;
            fontIncrease = (double)(MAX_FONT_SIZE - MIN_FONT_SIZE) / TimeLimit;

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
                case Keys.Escape:
                    //If the game is not paused, pause the game
                    if (!isPaused)
                    {
                        isPaused = true;
                        // Goes to pause screen
                        gameTimer.Enabled = false;
                        //Draw overlay
                        Refresh();
                        PauseScreen ps = new PauseScreen(this, calculateScore());
                        Form form = this.FindForm();
                        ps.Location = new Point((this.Width - ps.Width) / 2, (this.Height - ps.Height) / 2);
                        ps.Focus();
                        this.Controls.Add(ps);
                    }
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
                    //Application.Exit();//blow up
                    rightArrowDown = false;
                    break;
                default:
                    break;
            }
        }

        #endregion

        void resetBall()
        {
            ball.x = ((paddle.x - (ball.radius * 2)) + (paddle.width / 2));
            ball.y = paddle.y - (ball.radius * 2) - paddle.height;
            ball.yVel = -1 * Math.Abs(ball.yVel);
        }


        private void gameTimer_Tick(object sender, EventArgs e)
        {
            currentTime--;
            if (currentTime < 0) { lives = 0; }

            Form1.globalTimer++;
            paddle.Move(Convert.ToUInt16(rightArrowDown) - Convert.ToUInt16(leftArrowDown), this);

            ball.Move();
            ball.PaddleCollision(paddle);

            if (ball.WallCollision(this))
            { //run wall collision and respond if the ball has touched the bottom

                lives--;

                // Moves the ball back to origin
                resetBall();
                lifeRectangles.RemoveAt(lifeRectangles.Count - 1);
            }

            if (lives == 0)
            {
                gameTimer.Enabled = false;
                OnEnd();
            }

            //Check if ball has collided with any blocks
            for (int i = 0; i < blocks.Count; i++)
            {
                Block b = blocks[i];

                if (ball.BlockCollision(b))
                {
                    //Application.Exit(); //blow up
                    b.runCollision(); //should be switched to entirely, no lines below
                    if (b.hp < 1)
                    {
                        foreach (Powerup p in b.powerupList)
                        {
                            fallingPowerups.Add(p);
                        }
                        blocks.Remove(b);

                        double xpBarPercent = (Double)blocks.Count / blocksNum;
                        if (xpBarPercent != 1)
                        {
                            xpBarRegion.Width = (int)(right * xpBarPercent);
                            xpBarRegion.X = (right - xpBarRegion.Width);
                        };

                        if (blocks.Count == 0)
                        {
                            gameTimer.Enabled = false;
                            WinCondition();
                        }
                    }
                }
            }

            for (int p = 0; p < fallingPowerups.Count; p++)
            {
                //Choose to despawn or activate a powerup
                bool[] removeAndActivate = fallingPowerups[p].Move(this.Bottom, new Rectangle(paddle.x, paddle.y, paddle.width, paddle.height));
                if (removeAndActivate[1])
                {
                    //If the powerup hits the player, check to see if the powerup already exists and boost its strength, if not add it to the active list
                    bool addAsNewPowerup = true;
                    int ghostID = fallingPowerups[p].id;
                    foreach (Powerup q in activePowerups)
                    {
                        //If the powerups list already has this type of powerup, increase its active time and strength instaed of adding a new one.
                        if (q.id == ghostID)
                        {
                            addAsNewPowerup = false;
                            q.strength++;
                            q.activeTime += fallingPowerups[p].activeTime;
                            q.lifeSpan = q.activeTime;
                        }
                    }
                    if (addAsNewPowerup) { activePowerups.Add(fallingPowerups[p]); }
                }
                if (removeAndActivate[0])
                {
                    //If the powerup hits the player, or the screen end, remove it from the list
                    fallingPowerups.RemoveAt(p);
                };
            }

            for (int p = 0; p < activePowerups.Count; p++)
            {
                if (activePowerups[p].activeTime < 0) { activePowerups.RemoveAt(p); }
            }


            Refresh();

        }


        public void WinCondition()
        {
            Form1.currentLevel = (Form1.currentLevel == 12) ? 1 : (Form1.currentLevel + 1);

            Form form = this.FindForm();
            GameScreen gs = new GameScreen(false);

            gs.Location = new Point((form.Width - gs.Width) / 2, (form.Height - gs.Height) / 2);

            form.Controls.Add(gs);
            form.Controls.Remove(this);
        }

        double calculateScore()
        {
            int innitialScore = (int)(((double)(blocksNum - blocks.Count)) / (double)blocksNum * 10000);
            double scoreAsDouble = (double)innitialScore / 100;
            return scoreAsDouble;
        }
        public void OnEnd()
        {
            // Goes to the game over screen
            Form form = this.FindForm();
            EndScreen es = new EndScreen(calculateScore());

            es.Location = new Point((form.Width - es.Width) / 2, (form.Height - es.Height) / 2);

            form.Controls.Add(es);
            form.Controls.Remove(this);

        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
        }

        public void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // Draws paddle
            Rectangle paddleRect = new Rectangle(paddle.x, paddle.y, paddle.width, paddle.height);
            e.Graphics.DrawImage(stoneBlock, paddleRect);

            // Draws blocks

            foreach (Block b in blocks)
            {
                //e.Graphics.FillRectangle(blockBrush, b.x, b.y, b.width, b.height);
                e.Graphics.DrawImage(b.image, b.x, b.y, b.width + 2, b.height + 2);
                if (b.alphaValue != 0)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(b.alphaValue, 0, 0, 0)), b.overlay);
                }

            }

            //Draw Hearts

            foreach (Rectangle lifeRect in lifeRectangles)
            {
                e.Graphics.DrawImage(hearts, lifeRect);
            }

            //Draw Xp Bar
            Color barColor = Color.FromArgb(18, 0, 0, 0);
            SolidBrush barBrush = new SolidBrush(barColor);

            Rectangle fadeRect = xpFullRect;
            for (int i = 0; i < 10; i++)
            {
                fadeRect.Y -= i;
                fadeRect.Height += i;
                e.Graphics.FillRectangle(barBrush, fadeRect);
            }
            e.Graphics.DrawImage(fullXpBar, xpFullRect);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)), xpBarRegion);
            // e.Graphics.DrawImage(xpBar, xpRect);

            //Draw Falling Powerups
            foreach (Powerup p in fallingPowerups)
            {
                e.Graphics.DrawImage(p.image, p.rectangle);
            }

            //Draw Active Powerups
            int powerupYCoord = 80;
            foreach (Powerup p in activePowerups)
            {
                Double age = p.Age();
                int newSize = (int)(powerUpImageSize * age) + 3;
                e.Graphics.DrawImage(p.image, new Rectangle(powerUpOffset + ((powerUpImageSize - newSize) / 2), powerupYCoord + ((powerUpImageSize - newSize) / 2), newSize, newSize));
                // e.Graphics.DrawString("x" + p.strength, powerupFont, new SolidBrush(Color.FromArgb(180, 255, 255, 255)), new Point(powerUpImageSize + (2 * powerUpOffset), powerupYCoord + (powerUpImageSize / 2)));
                powerupYCoord += 45;
            }

            // Draws ball
            Rectangle ballRect = new Rectangle(ball.x, ball.y, 30, 30);
            e.Graphics.DrawImage(snowBall, ballRect);

            //Draw Time Limit
            int percentage = (TimeLimit - currentTime);
            int fontSize = (int)(percentage * fontIncrease) + MIN_FONT_SIZE;
            int colorSize = (int)(percentage * ((double)255 / (double)TimeLimit));
            Color fontColor = Color.FromArgb(255, colorSize, 255, colorSize);
            e.Graphics.DrawString("" + currentTime, new Font(Form1.pfc.Families[0], fontSize), new SolidBrush(fontColor), new PointF(timeDisplayPoint.X - fontSize, timeDisplayPoint.Y - fontSize));

            if (!gameTimer.Enabled)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)), new Rectangle(new Point(0, 0), this.Size));
            }
        }
    }
}