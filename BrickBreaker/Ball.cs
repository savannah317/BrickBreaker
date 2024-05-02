using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;


namespace BrickBreaker {
    public class Ball {
        public int x, y, radius, lastCollisionTimeStamp;
        public float xVel, yVel;
        public Color colour;

        public static Random rand = new Random();

        public Ball ( int _x, int _y, int _xVel, int _yVel, int _ballSize ) {
            x = _x;
            y = _y;
            xVel = _xVel;
            yVel = _yVel;
            radius = _ballSize / 2;
        }

        public void Move () {
            x += (int)xVel;
            y += (int)yVel;
        }

        public bool BlockCollision ( Block brick ) {

            Random rand = new Random();
            float speedMult = (float)(rand.Next(-15, 16) * 0.01 + 1);

            switch (Form1.CheckCollision(this, brick, brick.lastCollisionTimeStamp)) {
                case 0:
                    return false;
                case 1: //top
                    yVel *= Form1.isNegative(yVel) ? 1 : -1;
                    xVel *= speedMult;
                    this.y -= (int)yVel;

                    brick.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 2: //right side
                    xVel *= Form1.isNegative(xVel) ? -1 : 1;
                    yVel *= speedMult;
                    this.x += (int)xVel;

                    brick.lastCollisionTimeStamp = Form1.globalTimer;     
                    break;
                case 3: //bottom
                    yVel *= Form1.isNegative(yVel) ? -1 : 1;
                    xVel *= speedMult;
                    this.y += (int)yVel;

                    brick.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 4: //left
                    xVel *= Form1.isNegative(xVel) ? 1 : -1;
                    yVel *= speedMult;
                    this.x -= (int)xVel;

                    brick.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
            }

            return true;
        }

        public bool PaddleCollision ( Paddle paddle ) {

            Random rand = new Random();
            float speedMult = (float)(rand.Next(-15, 16) * 0.01 + 1);
            

            switch (Form1.CheckCollision(this, paddle, paddle.lastCollisionTimeStamp)) {
                case 0:
                    return false;
                case 1: //top
                    yVel *= Form1.isNegative(yVel) ? 1 : -1;
                    xVel *= speedMult;
                    this.y -= (int)yVel;

                    paddle.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 2: //right
                    xVel *= Form1.isNegative(xVel) ? -1 : 1;
                    yVel *= speedMult;
                    this.x += (int)xVel;

                    paddle.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 3:
                    yVel *= Form1.isNegative(yVel) ? -1 : 1;
                    xVel *= speedMult;
                    this.y += (int)yVel;

                    paddle.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 4:
                    xVel *= Form1.isNegative(xVel) ? 1 : -1;
                    yVel *= speedMult;
                    this.x -= (int)xVel;

                    paddle.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
            }

            return true;
        }

        public bool WallCollision ( UserControl UC ) {
            
            xVel *= (x <= 0 || x >= (UC.Width - radius)) ? -1 : 1;
            yVel *= (y <= radius) ? -1 : 1;

            return (y >= UC.Height - radius);
        }
    }
}