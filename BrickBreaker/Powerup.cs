using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreaker
{
    public class Powerup
    {
        public int id, x, y, radius;
        int fallSpeed;
        public double lifeSpan, activeTime;
        public Image image;
        int time = 0;
        public Rectangle rectangle;
        public int strength = 1;
        public Powerup(int _id, int _x, int _y)
        {
            id = _id;
            x = _x;
            y = _y;
            fallSpeed = Convert.ToInt16(Form1.powerupData[id][0]);
            activeTime = lifeSpan = Convert.ToDouble(Form1.powerupData[id][1]);
            radius = Convert.ToInt16(Form1.powerupData[id][3]);
        }

        public double Age()
        {
            time++;
            activeTime -= 1;
            return activeTime / lifeSpan;
        }
        public bool[] Move(int bottom, Rectangle paddleRect)
        {
            bool[] removeMyself = new bool[] { false, false };
            time++;
            y += fallSpeed;
            rectangle = new Rectangle(x - radius, y - radius, 2 * radius, 2 * radius);

            if (y > bottom)
            {
                removeMyself[0] = true;
            }
            if (paddleRect.IntersectsWith(rectangle))
            {
                removeMyself[0] = true;
                removeMyself[1] = true;
            }

            return removeMyself;
        }
    }
}
