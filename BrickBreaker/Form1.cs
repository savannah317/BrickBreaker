using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace BrickBreaker {
    public partial class Form1 : Form {
        public Form1 () {
            InitializeComponent();
        }

        public static int globalTimer;
        public static int tickDeltaTime = 10;

        public static int timeSincePoint ( int checkedTime ) {
            return checkedTime < globalTimer ? (globalTimer - checkedTime) : -1;  // returns -1 if checkedtime is in the future
        }

        public static float clamp ( float value, float min, float max ) {
            return Math.Max(min, Math.Min(max, value));
        }

        public static void ChangeScreen ( object sender, UserControl next ) {

            Form f; // will either be the sender or parent of sender 

            if (sender is Form) {
                f = (Form)sender;
            } else {
                UserControl current = (UserControl)sender;
                f = current.FindForm();
                f.Controls.Remove(current);
            }

            next.Location = new Point((f.ClientSize.Width - next.Width) / 2, (f.ClientSize.Height - next.Height) / 2);
            f.Controls.Add(next);

            next.Focus();
        }

        public static bool IsWithinRange ( float num, float lowerBound, float upperBound ) { return num >= lowerBound && num <= upperBound; }



        public static int CheckCollision ( Ball ball, Paddle rectObject, int collisionTimeStamp ) { //returns 0 (no collision) or 1-4 (collides from the rectangle's top, right side, bottom and left side respectively)

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

        public static int CheckCollision ( Ball ball, Block rectObject, int collisionTimeStamp ) { //returns 0 (no collision) or 1-4 (collides from the rectangle's top, right side, bottom and left side respectively)

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

        private void Form1_Load ( object sender, EventArgs e ) {
            // Start the program centred on the Menu Screen
            MenuScreen ms = new MenuScreen();
            this.Controls.Add(ms);

            ms.Location = new Point((this.Width - ms.Width) / 2, (this.Height - ms.Height) / 2);
        }
    }
}
