using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _321_Lab05_3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class MapLab01 : Room
    {
        int tileSize = 48;

        public MapLab01(Texture2D bg, Texture2D layer2, AnimatedTexture DoorOpenTexture,
            AnimatedTexture Enemymelee_Idle, AnimatedTexture Enemymelee_Walk, AnimatedTexture Enemymelee_Attack, AnimatedTexture Enemymelee_Death,
            AnimatedTexture EnemyShooting_Idle, AnimatedTexture EnemyShooting_Walk, AnimatedTexture EnemyShooting_Death,
            Texture2D enemyTexture, Texture2D bulletTexture, Texture2D parry, Texture2D Laser)
            : base(bg, layer2, DoorOpenTexture, new Rectangle(109 * 48, 49 * 48, 4 * 48, 1 * 48), new Vector2(109 * 48, 46 * 48),
                  new Vector2(15 * 48, 15 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2))
        {
            NextRooms = new List<int> { 5, 6 };

            void AddObstacle(float xTile, float yTile, float widthTile, float heightTile)
            {
                float x = xTile * tileSize;
                float y = (yTile) * tileSize;
                float w = widthTile * tileSize;
                float h = (heightTile) * tileSize;

                Obstacles.Add(new Rectangle((int)MathF.Round(x), (int)MathF.Round(y), (int)MathF.Round(w), (int)MathF.Round(h)));
            }

            // ... (Your obstacle code remains the same) ...
            AddObstacle(20, 7, 24, 7);
            AddObstacle(3, 0, 41, 8);
            AddObstacle(3, 0, 1, 30);
            AddObstacle(6, 8, 6, 1);
            AddObstacle(13, 8, 2, 1);
            AddObstacle(16, 8, 2, 1);
            AddObstacle(19, 13, 1, 1);
            AddObstacle(19, 21, 1, 1);
            AddObstacle(5, 12, 2, 1);
            AddObstacle(5, 15, 2, 1);
            AddObstacle(5, 19, 2, 1);
            AddObstacle(44, 4, 44, 2);
            AddObstacle(86, 5, 2, 28);
            AddObstacle(74, 55, 24, 9);
            AddObstacle(98, 46, 4, 8);
            AddObstacle(98, 54, 2, 1);
            AddObstacle(44, 6, 1, 3);
            AddObstacle(74, 6, 1, 1);
            AddObstacle(50, 11.8f, 4, 2.2f);
            AddObstacle(70, 10.7f, 4, 1.3f);
            AddObstacle(71, 12, 2, 1);
            AddObstacle(70, 23.8f, 4, 2.2f);
            AddObstacle(53, 22, 1, 1);
            AddObstacle(50, 23, 4, 1);
            AddObstacle(51, 24, 2, 1);
            AddObstacle(82, 13, 3, 1);
            AddObstacle(82, 16, 3, 1);
            AddObstacle(82, 20, 3, 1);
            AddObstacle(74, 54, 3, 1);
            AddObstacle(35, 45, 10, 10);
            AddObstacle(48, 61, 16, 3);
            AddObstacle(42, 71.8f, 4, 2.2f);
            AddObstacle(35, 55, 1, 30);
            AddObstacle(3, 27, 20, 2);
            AddObstacle(20, 21, 24, 10);
            AddObstacle(43, 31, 11, 23);
            AddObstacle(60, 31, 27, 23);
            AddObstacle(35, 79, 17, 13);
            AddObstacle(45, 92, 1, 19);
            AddObstacle(46, 109, 20, 2);
            AddObstacle(74, 71, 24, 10);
            AddObstacle(74, 70, 2, 1);
            AddObstacle(58, 79, 17, 13);
            AddObstacle(64, 92, 2, 20);
            AddObstacle(48, 104.8f, 2, 1.2f);
            AddObstacle(48, 104.8f, 2, 1.2f);
            AddObstacle(54, 104.8f, 2, 1.2f);
            AddObstacle(60, 104.8f, 2, 1.2f);
            AddObstacle(64, 70, 4, 4);
            AddObstacle(97, 54, 1, 1);
            AddObstacle(97, 81, 5, 5);
            AddObstacle(120, 81, 5, 5);
            AddObstacle(124, 77, 5, 5);
            AddObstacle(100, 85, 20, 2);
            AddObstacle(102, 48, 7, 2);
            AddObstacle(113, 48, 7, 2);
            AddObstacle(120, 49, 4, 5);
            AddObstacle(109, 48, 4, 1);
            AddObstacle(124, 53, 4, 5);
            AddObstacle(128, 57, 1, 20);
            AddObstacle(111, 62, 2, 8);
            AddObstacle(113, 63.5f, 1, 3);

            // Add enemies specific to this room
        }

    }
}