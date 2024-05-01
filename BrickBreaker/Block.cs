using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BrickBreaker
{
    public class Block
    {

        public Rectangle overlay;
        public int alphaValue = 0;
        public int x;
        public int y; 
        public int hp;
        public Image image;
        public int width = 50;
        public int height = 25;
        public int id;
        public int lastCollisionTimeStamp;

        int opacityChange = 30;

        public Color colour;

        public static Random rand = new Random();

        public Block(int _x, int _y, int width_, int height_, int id_)
        {
            x = _x;
            y = _y;
            width = width_;
            height = height_;
            id = id_;

            overlay = new Rectangle(x, y, width, height);
        }

        public void runCollision() {
            //handle the removal of health
            hp--;
            alphaValue += (alphaValue + opacityChange > 250) ? 0 : opacityChange;
            //sprite changes
            //any other stuff
            //removal of block
        }
    }
}
