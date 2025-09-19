using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class Room1 : Room
    {
        public Room1(Texture2D bg, Texture2D door, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, door, new Rectangle(109 * 24, 47 * 24, 4 * 24, 3 * 24), new Vector2(55 * 24, 98 * 24), new Rectangle(0, 0, 3285, 2970))
        {
            NextRooms = new List<int> { 1, 2 };
            int tileSize = 24;

            void AddObstacle(int xTile, int yTile, int widthTile, int heightTile)
            {
                Obstacles.Add(new Rectangle(xTile * tileSize, yTile * tileSize, widthTile * tileSize, heightTile * tileSize));
            }

            // ... (Your obstacle code remains the same) ...
            AddObstacle(4, 7, 18, 1);
            AddObstacle(4, 26, 18, 1);
            AddObstacle(3, 5, 1, 22);

            AddObstacle(20, 2, 24, 12);
            AddObstacle(20, 20, 24, 12);

            AddObstacle(36, 5, 50, 1);
            AddObstacle(86, 5, 1, 26);
            AddObstacle(36, 30, 18, 1);
            AddObstacle(60, 30, 26, 1);

            AddObstacle(53, 30, 1, 24);
            AddObstacle(60, 30, 1, 24);

            AddObstacle(36, 53, 18, 1);
            AddObstacle(61, 54, 15, 1);
            AddObstacle(48, 60, 16, 4);
            AddObstacle(35, 53, 1, 26);
            AddObstacle(74, 53, 1, 11);
            AddObstacle(36, 78, 16, 1);
            AddObstacle(58, 78, 16, 1);
            AddObstacle(74, 70, 1, 11);
            AddObstacle(61, 53, 1, 1);

            AddObstacle(45, 108, 20, 1);
            AddObstacle(45, 89, 1, 19);
            AddObstacle(64, 89, 1, 19);
            AddObstacle(45, 92, 7, 1);
            AddObstacle(58, 92, 4, 1);
            AddObstacle(61, 91, 4, 1);
            AddObstacle(51, 79, 1, 13);
            AddObstacle(58, 79, 1, 13);

            AddObstacle(101, 49, 8, 1);
            AddObstacle(113, 49, 8, 1);
            AddObstacle(120, 49, 1, 4);
            AddObstacle(120, 53, 3, 1);
            AddObstacle(123, 53, 1, 1);
            AddObstacle(123, 54, 1, 1);
            AddObstacle(124, 54, 1, 4);
            AddObstacle(124, 58, 4, 1);
            AddObstacle(128, 58, 1, 19);
            AddObstacle(124, 76, 4, 2);
            AddObstacle(124, 76, 1, 5);
            AddObstacle(120, 80, 4, 2);
            AddObstacle(120, 80, 1, 5);
            AddObstacle(100, 84, 20, 1);
            AddObstacle(101, 80, 1, 4);
            AddObstacle(97, 80, 4, 1);
            AddObstacle(97, 70, 1, 10);
            AddObstacle(97, 54, 1, 10);
            AddObstacle(96, 54, 4, 1);
            AddObstacle(98, 53, 4, 1);
            AddObstacle(101, 49, 1, 4);

            AddObstacle(75, 63, 22, 1);
            AddObstacle(75, 70, 22, 1);
            AddObstacle(7, 8, 6, 1);
            AddObstacle(14, 8, 2, 1);
            AddObstacle(17, 8, 2, 1);
            AddObstacle(5, 11, 2, 2);
            AddObstacle(5, 14, 2, 2);
            AddObstacle(5, 18, 2, 2);
            AddObstacle(19, 12, 1, 2);
            AddObstacle(19, 20, 1, 2);
            AddObstacle(50, 10, 4, 4);
            AddObstacle(70, 10, 4, 4);
            AddObstacle(50, 22, 4, 4);
            AddObstacle(70, 22, 4, 4);
            AddObstacle(44, 6, 1, 3);
            AddObstacle(74, 6, 1, 1);
            AddObstacle(83, 12, 2, 2);
            AddObstacle(82, 16, 3, 2);
            AddObstacle(82, 19, 3, 2);
            AddObstacle(36, 54, 9, 2);
            AddObstacle(42, 70, 4, 4);
            AddObstacle(64, 70, 4, 4);
            AddObstacle(48, 104, 2, 2);
            AddObstacle(54, 104, 2, 2);
            AddObstacle(60, 104, 2, 2);
            AddObstacle(110, 62, 4, 8);

            // Add enemies specific to this room
            AddEnemy(new Enemy(enemyTexture, new Vector2(43* tileSize, 60* tileSize), 1.0f, 500f));
            AddEnemy(new Enemy(enemyTexture, new Vector2(44 * tileSize, 70 * tileSize), 1.0f, 500f));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(70 * tileSize, 56 * tileSize), 1.5f, 500f, bulletTexture));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(68 * tileSize, 70 * tileSize), 1.5f, 500f, bulletTexture));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(78 * tileSize, 10 * tileSize), 1.5f, 500f, bulletTexture));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(12 * tileSize, 15 * tileSize), 1.5f, 500f, bulletTexture));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(120 * tileSize, 62 * tileSize), 1.5f, 500f, bulletTexture));
        }
    }
}