using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class MapCity01 : Room
    {
        public MapCity01(Texture2D bg, Texture2D door, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, door, new Rectangle(72 * 48, 16 * 48, 6 * 48, 2 * 48), new Vector2(113 * 48, 17 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2))
        {
            NextRooms = new List<int> { 2, 3 };
            int tileSize = 48;

            void AddObstacle(int xTile, int yTile, int widthTile, int heightTile)
            {
                Obstacles.Add(new Rectangle(xTile * tileSize, yTile * tileSize, widthTile * tileSize, heightTile * tileSize));
            }

            // ... (Your obstacle code remains the same) ...
            AddObstacle(80, 28, 30, 28);
            AddObstacle(54, 46, 26, 20);
            AddObstacle(70, 56, 24, 24);
            AddObstacle(70, 86, 24, 24);
            AddObstacle(94, 96, 24, 5);
            AddObstacle(116, 28, 4, 70);
            AddObstacle(27, 104, 45, 4);
            AddObstacle(120, 5, 4, 25);
            AddObstacle(26, 64, 8, 40);
            AddObstacle(26, 46, 22, 20);
            AddObstacle(38, 72, 8, 4);
            AddObstacle(38, 82, 8, 4);
            AddObstacle(38, 94, 8, 4);
            AddObstacle(56, 72, 8, 4);
            AddObstacle(56, 82, 8, 4);
            AddObstacle(56, 94, 8, 4);
            AddObstacle(101, 4, 25, 6);
            AddObstacle(101, 4, 3, 25);
            AddObstacle(100, 64, 6, 2);
            AddObstacle(102, 66, 2, 2);
            AddObstacle(98, 74, 2, 4);
            AddObstacle(110, 74, 2, 4);
            AddObstacle(102, 88, 6, 2);
            AddObstacle(104, 86, 2, 2);
            AddObstacle(38, 42, 6, 4);
            AddObstacle(64, 42, 6, 4);
            AddObstacle(26, 16, 2, 30);
            AddObstacle(26, 16, 46, 2);
            AddObstacle(38, 18, 6, 6);
            AddObstacle(64, 18, 6, 6);
            AddObstacle(44, 28, 4, 10);
            AddObstacle(60, 28, 4, 10);
            AddObstacle(78, 16, 6, 2);
            AddObstacle(80, 16, 6, 15);
        }
    }
}