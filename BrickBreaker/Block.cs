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
        //tool id order: neutral, pickaxe, axe, shovel (0-3)

        public int x, y, width, height, hp, id, heldPowerID, toolWeaknessID, lastCollisionTimeStamp;
        public Image image;

        public int alphaValue = 0;
        public Rectangle overlay;


        public List<Powerup> powerupList = new List<Powerup>();

        int opacityChange = 30;

        public Color colour;

        public Block(int _x, int _y, int width_, int height_, int id_)
        {
            x = _x;
            y = _y;
            width = width_;
            height = height_;
            id = id_;

            overlay = new Rectangle(x, y, width, height);
        }
        //   public int runCollision(List<Block> b) {
        //         hp--; //may need to change if we want some blocks to only be vulnerable to certain tools or smthn
        //          if (hp > 0) { return 0; }
        //          b.Remove(this); //remove the block if it's hp is <= 0
        //          return heldPowerID;
        // }


        public void runCollision()
        {
            //handle the removal of health
            hp--;
            alphaValue += (alphaValue + opacityChange > 250) ? 0 : opacityChange;
            //sprite changes
            //any other stuff
            //removal of block
        }
    }
}
