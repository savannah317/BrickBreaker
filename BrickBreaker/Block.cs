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
        Image image;


        public Color colour;

        public static Random rand = new Random();

        public Block(int _x, int _y, int width_, int height_, int id_) {
            x = _x;
            y = _y;
            width = width_;
            height = height_;
            id = id_;

            switch (id) {
                case 1:
                    hp = 2;
                    image = Properties.Resources.dirt;
                    heldPowerID = 0;
                    toolWeaknessID = 3;
                    break;
                case 2:
                    
                    break;
                case 3:
                    
                    break;
                case 4:
                    
                    break;
                case 5:
                    
                    break;
                case 6:
                    
                    break;
                case 7:
                    
                    break;
                case 8:
                    
                    break;
                case 9:
                    
                    break;
                case 10:
                    
                    break;
                case 11:
                    
                    break;
                case 12:
                    
                    break;
                case 13:
                    
                    break;
                case 14:
                    
                    break;
                case 15:
                    
                    break;
                case 16:
                    
                    break;
                case 17:
                    
                    break;
                case 18:
                    
                    break;
                case 19:
                    
                    break;
                case 20:
                    
                    break;
                case 21:
                    
                    break;
                case 22:
                    
                    break;
                case 23:
                    
                    break;
                case 24:
                    
                    break;

            }
        }

        public int runCollision(List<Block> b) {
            hp--; //may need to change if we want some blocks to only be vulnerable to certain tools or smthn
            if (hp > 0) { return 0; }
            b.Remove(this); //remove the block if it's hp is <= 0
            return heldPowerID;
        }
    }
}
