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
        

        public int x;
        public int y; 
        public int hp;
        public int width = 50;
        public int height = 25;
        public int id;
        public int lastCollisionTimeStamp;

        public Color colour;

        public static Random rand = new Random();

        public Block(int _x, int _y, int width_, int height_, int id_)
        {
            x = _x;
            y = _y;
            width = width_;
            height = height_;
            id = id_;
        }

        public void runCollision() {
            //handle the removal of health
            //removal of block
            //sprite changes
            //any other stuff
        }
    }
}
