using System;
using System.Drawing;
using System.Windows.Forms;


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
                case 1:
                    yVel *= -1;
                    xVel *= speedMult;
                    brick.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 2:
                    xVel *= -1;
                    yVel *= speedMult;
                    brick.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 3:
                    yVel *= -1;
                    xVel *= speedMult;
                    brick.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 4:
                    xVel *= -1;
                    yVel *= speedMult;
                    brick.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
            }

            return true;
        }

        public bool PaddleCollision ( Paddle paddle, Label label ) {

            Random rand = new Random();
            float speedMult = (float)(rand.Next(-15, 16) * 0.01 + 1);
            label.Text = xVel.ToString();

            switch (Form1.CheckCollision(this, paddle, paddle.lastCollisionTimeStamp)) {
                case 0:
                    return false;
                case 1:
                    yVel *= -1;
                    xVel *= speedMult;
                    paddle.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 2:
                    xVel *= -1;
                    yVel *= speedMult;
                    paddle.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 3:
                    yVel *= -1;
                    xVel *= speedMult;
                    paddle.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
                case 4:
                    xVel *= -1;
                    yVel *= speedMult;
                    paddle.lastCollisionTimeStamp = Form1.globalTimer;
                    break;
            }

            return true;
        }

        public void WallCollision ( UserControl UC ) {
            // Collision with left wall
            if (x <= 0) {
                xVel *= -1;
            }
            // Collision with right wall
            if (x >= (UC.Width - radius)) {
                xVel *= -1;
            }
            // Collision with top wall
            if (y <= 2) {
                yVel *= -1;
            }
        }

        public bool BottomCollision ( UserControl UC ) {
            Boolean didCollide = false;

            if (y >= UC.Height) {
                didCollide = true;
            }

            return didCollide;
        }

    }
}
