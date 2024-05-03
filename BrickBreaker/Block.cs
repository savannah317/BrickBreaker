using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace BrickBreaker
{
    public class Block
    {
        //tool id order: neutral, pickaxe, axe, shovel (0-3)

        public int x, y, width, height, hp, id, heldPowerID, toolWeaknessID, lastCollisionTimeStamp;
        public Image image;

        public int alphaValue = 0;
        public Rectangle overlay;


        PointF[] rectanglePoints;

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
        public PointF[][] shadowPoints(PointF lightSource, double lightStrength)
        {

            PointF[] shadowPoints = new PointF[]
            {
                    getShadowPoint(rectanglePoints[0], lightSource, lightStrength),
                    getShadowPoint(rectanglePoints[1], lightSource, lightStrength),
                    getShadowPoint(rectanglePoints[2], lightSource, lightStrength),
                    getShadowPoint(rectanglePoints[3], lightSource, lightStrength),
            };

            PointF[][] shadows = new PointF[][]
            {
                    new PointF[]
                    {
                    rectanglePoints[0],
                    shadowPoints[0],
                    shadowPoints[1],
                    rectanglePoints[1],
                    },
                    new PointF[]
                    {
                    rectanglePoints[1],
                    shadowPoints[1],
                    shadowPoints[2],
                    rectanglePoints[2],
                    },
                    new PointF[]
                    {
                    rectanglePoints[2],
                    shadowPoints[2],
                    shadowPoints[3],
                    rectanglePoints[3],
                    },
                    new PointF[]
                    {
                    rectanglePoints[3],
                    shadowPoints[3],
                    shadowPoints[0],
                    rectanglePoints[0],
                    },
            };
            return shadows;
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
