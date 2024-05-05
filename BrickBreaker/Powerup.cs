using BrickBreaker.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Automation;

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
            #region Powerup Data
            string[][] powerupData = new string[][]
            {
                new string[]{"Fallspeed", "Activetime", "Png", "Radius" },
                new string[]{"3", "400", "apple", "10"},
                new string[]{"3", "400", "seeds", "10"},
                new string[]{"3", "700", "stone_pickaxe", "10"},
            };
            #endregion

            id = _id;
            x = _x;
            y = _y;
            fallSpeed = Convert.ToInt16(powerupData[id][0]);
            activeTime = lifeSpan = Convert.ToDouble(powerupData[id][1]);
            radius = Convert.ToInt16(powerupData[id][3]);
            ResourceManager rm = Resources.ResourceManager;
            image = (Image)rm.GetObject(powerupData[id][2]);
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

        public void OnPickup()
        {
            switch (id)
            {
                case 1: //Apple
                    if (GameScreen.lives < GameScreen.MAX_LIVES)
                    {
                        GameScreen.lives++;
                    }
                    break;
                case 2: //Seeds
                    break;
                case 3: //Stone Pick
                    GameScreen.ball.image = Resources.stone_pickaxe;
                    GameScreen.ball.tools.Add("Pick");
                    GameScreen.ball.strength = strength + 1;
                    break;


            }
        }
        public void WhileActive()
        {
            switch (id)
            {
                case 1: //Apple
                    if (activeTime % (57 - (3 * strength)) == 0)
                    {
                        string[] tools = new string[]
                     {
                            "Shears", "Axe", "Shovel"
                    };
                        Projectile p = new Projectile(0, -4 - (3 * strength), Resources.heart, 10 + (3 * strength), tools, strength);
                        GameScreen.projectiles.Add(p);
                    }
                    break;
                case 2: //Seeds
                    if (activeTime % (14 - (3 * strength)) == 0)
                    {
                        string[] tools = new string[]
                        {
                            "Shears", "Shovel"
                        };
                        Projectile p = new Projectile((int)(10.1 * Math.Sin(activeTime)), -12 - (3 * strength), Resources.wheat, 7 + (3 * strength), tools, strength);
                        GameScreen.projectiles.Add(p);
                    }
                    break;


            }
        }
        public void OnDeath()
        {
            switch (id)
            {
                case 1: //Apple
                    break;
                case 2: //Seeds
                    break;
                case 3: //Stone Pick
                    GameScreen.ball.tools.Remove("Pick");
                    GameScreen.ball.image = Resources.snowball;
                    break;
            }
        }
    }
}
