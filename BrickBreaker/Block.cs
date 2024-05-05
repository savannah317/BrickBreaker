using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Reflection.Emit;
using System.Resources;
using BrickBreaker.Properties;

namespace BrickBreaker
{
    public class Block
    {
        //tool id order: neutral, pickaxe, axe, shovel (0-3)

        public int x, y, width, height, hp, id, heldPowerID, lastCollisionTimeStamp;
        public Image image;
        string toolWeakness;

        public int alphaValue = 0;
        public Rectangle overlay;


        PointF[] rectanglePoints;

        public List<Powerup> powerupList = new List<Powerup>();

        int opacityChange = 30;

        public Color colour;

        public Block(int _x, int _y, int width_, int height_, int id_, int randomOneHundred)
        {
            #region Block Data
            string[][] blockData = new string[][]
        {
        new string [] {"Hp", "Weak To", "Png", "Chance Of Powerup (0 - 10)", "ID of Powerup"},

        new string [] {"1", "Shovel", "grass_block", "10", "2"},        //Grass Block      |   //Seeds
        new string [] {"3", "Axe", "oak_log", "1", "1"},                //Oak Wood Log     |
        new string [] {"1", "Shears", "oak_leaves", "20", "1"},         //Oak Leaves       |   //Apple
        new string [] {"3", "Axe", "oak_planks", "1", "1"},             //Oak Planks       |
        new string [] {"2", "Pick", "stone", "1", "3"},                 //Stone            |   //Stone Pick
        
        new string [] {"2", "Pick", "iron_ore", "1", "1"},              //Iron Ore         |
        new string [] {"3", "Pick", "gold_ore", "1", "1"},              //Gold Ore         |
        new string [] {"2", "Pick", "diamond_ore", "1", "1"},           //Diamond Ore      |
        new string [] {"5", "Pick", "obsidian", "1", "1"},              //Obsidian         |
        new string [] {"2", "Pick", "netherrack", "1", "1"},            //Netherack        |
        
        new string [] {"3", "Pick", "quartz_ore", "1", "1"},            //Quartz Ore       |
        new string [] {"4", "Pick", "netherite", "1", "1"},             //Netherite        |
        new string [] {"10", "Sword", "endframe_top", "4", "1"},        //End Portal Block |
        new string [] {"4", "Pick", "stonebrick", "2", "1"},            //Stone Bricks     |
        new string [] {"4", "Pick", "end_stone", "1", "1"},             //Endstone         |
        
        new string [] {"4", "Pick", "end_bricks", "1", "1"},            //Endstone Bricks  |
        new string [] {"2", "Shovel", "sand", "1", "1"},                //Sand             |
        new string [] {"2", "Shovel", "gravel", "1", "1"},              //Gravel           |
        new string [] {"4", "Pick", "coal_ore", "1", "1"},              //Coal Ore         |
        new string [] {"2", "Sword", "water", "1", "1"},                //Water            |
        
        new string [] {"2", "Sword", "lava", "1", "1"},                 //Lava             |
        new string [] {"1", "Sword", "portal", "0", "1"},               //Nether Portal    |
        new string [] {"2", "Sword", "bedrock", "1", "1"},              //Bedrock          |
        new string [] {"4", "Sword", "dragon_egg", "1", "1"},           //Dragon Egg       |
        new string [] {"3", "Pick", "cobblestone", "1", "1"},           //Cobblestone      |

        };
            #endregion

            toolWeakness = blockData[id][1];
            x = _x;
            y = _y;
            width = width_;
            height = height_;
            id = id_;

            hp = Convert.ToInt16(blockData[id][0]);

            //Get the correct image
            ResourceManager rm = Resources.ResourceManager;
            image = (Image)rm.GetObject(blockData[id][2]);

            //Find if the block should contain powerups
            if (randomOneHundred <= Convert.ToInt16(blockData[id][3]))
            {
                int powerupID = Convert.ToInt16(blockData[id][4]);
                Powerup newPowerup = new Powerup(powerupID, x + (width / 2), y + (height / 2));

                powerupList.Add(newPowerup);
            }


            overlay = new Rectangle(x, y, width, height);

            rectanglePoints = new PointF[]
             {
                new PointF(x, y), //Left Up
                new PointF(x, y + height), //Left Down
                new PointF(x + width, y + height), //Right Down
                new PointF(x + width, y), //Right Up
             };
        }

        //   public int runCollision(List<Block> b) {
        //         hp--; //may need to change if we want some blocks to only be vulnerable to certain tools or smthn
        //          if (hp > 0) { return 0; }
        //          b.Remove(this); //remove the block if it's hp is <= 0
        //          return heldPowerID;
        // }

        #region Creating the shadows from the block
        double getTheta(PointF one, PointF two)
        {
            double deltaX = (two.X - one.X);
            double deltaY = ((two.Y - one.Y) == 0) ? 0.001 : (two.Y - one.Y);
            double theta = Math.Atan(deltaX / deltaY);
            return Math.Abs(theta);
        }

        int positiveOrNegative(float one, float two)
        {
            int delta = (two >= one) ? -1 : 1;
            return delta;
        }
        PointF getShadowPoint(PointF p, PointF lightSource, double lightStrength)
        {
            double specificPointLS = lightStrength;
            double theta = getTheta(p, lightSource);

            double deltaX = specificPointLS * Math.Sin(theta);
            deltaX *= positiveOrNegative(p.X, lightSource.X);

            double deltaY = specificPointLS * Math.Cos(theta);
            deltaY *= positiveOrNegative(p.Y, lightSource.Y);

            PointF shadowedPoint = new PointF(p.X + (float)deltaX, p.Y + (float)deltaY);

            return shadowedPoint;
        }

        double GetRadius(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public PointF[] shadowPoints(PointF lightSource, double lightStrength)
        {

            PointF[] shadowPoints = new PointF[]
            {
                    getShadowPoint(rectanglePoints[0], lightSource, lightStrength),
                    getShadowPoint(rectanglePoints[1], lightSource, lightStrength),
                    getShadowPoint(rectanglePoints[2], lightSource, lightStrength),
                    getShadowPoint(rectanglePoints[3], lightSource, lightStrength),
            };

            PointF[] checkPoints = new PointF[]
        {
                getShadowPoint(rectanglePoints[0], lightSource, 1),
                    getShadowPoint(rectanglePoints[1], lightSource, 1),
                    getShadowPoint(rectanglePoints[2], lightSource, 1),
                    getShadowPoint(rectanglePoints[3], lightSource, 1),
        };

            //Find if i can simplify the shape into 1 polygon using checkPoints
            int check = 5;
            for (int i = 0; i < 4; i++)
            {
                Rectangle box = new Rectangle(x, y, width, height);
                if (box.Contains((int)checkPoints[i].X, (int)checkPoints[i].Y))
                {
                    check = i;
                }
            }
            if (check != 5)
            {
                int i = check;
                PointF[] pointFs = new PointF[6]
                {
                    rectanglePoints[i],
                    rectanglePoints[i + ((i > 2) ? (-3) : (1))],
                    shadowPoints[i + ((i > 2) ? (-3) : (1))],
                    shadowPoints[i + ((i < 2) ? (2) : (-2))],
                    shadowPoints[i + ((i < 1) ? (3) : (-1))],
                    rectanglePoints[i + ((i < 1) ? (3) : (-1))]
                };

                return pointFs;
            }
            else
            {
                double[] sideLengthChecks = new double[]
                 {
                    GetRadius(shadowPoints[1],shadowPoints[2]),
                    GetRadius(shadowPoints[2],shadowPoints[3]),
                    GetRadius(shadowPoints[3],shadowPoints[0]),
                    GetRadius(shadowPoints[0],shadowPoints[1]),
                 };

                double longestSide = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (sideLengthChecks[i] > longestSide)
                    {
                        longestSide = sideLengthChecks[i];
                        check = i;
                    }
                }

                PointF[] pointFs = new PointF[6]
                {
                    rectanglePoints[check + ((check + 1 > 3) ? -3 : 1)],
                    rectanglePoints[check + ((check + 2 > 3) ? -2 : 2)],
                    shadowPoints[check + ((check + 2 > 3) ? -2 : 2)],
                    shadowPoints[check + ((check + 3 > 3) ? -1 : 3)],
                    shadowPoints[check],
                    shadowPoints[check + ((check + 1 > 3) ? -3 : 1)]
                };

                return pointFs;
            }
        }
        #endregion
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
