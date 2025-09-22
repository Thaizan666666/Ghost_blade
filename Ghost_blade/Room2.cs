using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Ghost_blade
{
    public class Room2 : Room
    {
        public Room2(Texture2D bg, Texture2D door, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, door, new Rectangle(39 * 24, 15 * 24, 3 * 24, 3 * 24), new Vector2(40 * 24, 96 * 24), new Rectangle(0, 0, 3285, 2970)) //
        {
            NextRooms = new List<int> { 0, 2 };
            int tileSize = 24;

            void AddObstacle(int xTile, int yTile, int widthTile, int heightTile)
            {
                Obstacles.Add(new Rectangle(xTile * tileSize, yTile * tileSize, widthTile * tileSize, heightTile * tileSize));
            }

            AddObstacle(19, 80, 19, 12);
            AddObstacle(31, 92, 1, 10);
            AddObstacle(31, 102, 20, 1);
            AddObstacle(44, 80, 23, 12);
            AddObstacle(50, 92, 1, 12);
            AddObstacle(66, 68, 14, 12);
            AddObstacle(80, 70, 14, 13);
            AddObstacle(19, 44, 1, 50);
            AddObstacle(19, 36, 21, 12);
            AddObstacle(46, 36, 21, 12);
            AddObstacle(25, 15, 5, 5);
            AddObstacle(25, 32, 5, 5);
            AddObstacle(56, 15, 5, 5);
            AddObstacle(30, 15, 9, 3);
            AddObstacle(42, 15, 15, 3);
            AddObstacle(56, 18, 1, 1);
            AddObstacle(54, 34, 4, 2);
            AddObstacle(58, 20, 1, 20);
            AddObstacle(56, 32, 2, 2);
            AddObstacle(34, 24, 4, 4);
            AddObstacle(46, 24, 4, 4);
            AddObstacle(25, 20, 1, 16);
            AddObstacle(66, 48, 16, 12);
            AddObstacle(82, 35, 22, 23);
            AddObstacle(104, 25, 2, 14);
            AddObstacle(106, 28, 7, 1);
            AddObstacle(112, 29, 5, 10);
            AddObstacle(116, 38, 1, 50);
            AddObstacle(100, 70, 20, 13);
            AddObstacle(89, 83, 1, 18);
            AddObstacle(106, 83, 1, 18);
            AddObstacle(90, 100, 20, 1);
            AddObstacle(102, 94, 4, 6);
            AddObstacle(101, 97, 1, 6);
            AddObstacle(110, 42, 4, 2);
            AddObstacle(108, 54, 4, 4);
            AddObstacle(94, 62, 4, 4);
            AddObstacle(28, 54, 6, 2);
            AddObstacle(28, 54, 2, 6);
            AddObstacle(28, 68, 2, 6);
            AddObstacle(28, 72, 6, 2);
            AddObstacle(52, 54, 6, 2);
            AddObstacle(56, 54, 2, 6);
            AddObstacle(52, 72, 6, 2);
            AddObstacle(56, 67, 2, 6);
            AddObstacle(38, 99, 1, 2);
            AddObstacle(40, 99, 2, 2);
            AddObstacle(43, 99, 1, 2);
            AddObstacle(32, 94, 3, 2);
            AddObstacle(32, 97, 3, 2);

            AddEnemy(new Enemy_Melee(enemyTexture, new Vector2(41 * tileSize, 63 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(enemyTexture, new Vector2(42 * tileSize, 26 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(64 * tileSize, 64 * tileSize), 1.0f, 1000f, bulletTexture));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(104 * tileSize, 90 * tileSize), 1.0f, 1000f, bulletTexture));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(96 * tileSize, 90 * tileSize), 1.0f, 1000f, bulletTexture));
        }
    }
}
