using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class MapCity02 : Room
    {
        public MapCity02(Texture2D bg, Texture2D door, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, door, new Rectangle(36 * 48, 30 * 48, 6 * 48, 2 * 48), new Vector2(109 * 48, 101 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2))
        {
            NextRooms = new List<int> { 1, 3 };
            int tileSize = 48;

            void AddObstacle(int xTile, int yTile, int widthTile, int heightTile)
            {
                Obstacles.Add(new Rectangle(xTile * tileSize, yTile * tileSize, widthTile * tileSize, heightTile * tileSize));
            }

            // ... (Your obstacle code remains the same) ...
            AddObstacle(110, 68, 14, 24);
            AddObstacle(116, 92, 4, 24);
            AddObstacle(98, 110, 20, 4);
            AddObstacle(88, 68, 16, 24);
            AddObstacle(98, 92, 2, 24);
            AddObstacle(62, 58, 26, 34);
            AddObstacle(108, 16, 12, 26);
            AddObstacle(116, 4, 10, 38);
            AddObstacle(122, 42, 2, 26);
            AddObstacle(88, 16, 14, 26);
            AddObstacle(62, 30, 26, 22);
            AddObstacle(90, 4, 2, 16);
            AddObstacle(90, 4, 26, 2);
            AddObstacle(98, 48, 12, 2);
            AddObstacle(98, 60, 12, 2);
            AddObstacle(42, 30, 22, 2);
            AddObstacle(28, 30, 8, 2);
            AddObstacle(28, 82, 34, 2);
            AddObstacle(28, 30, 2, 54);
            AddObstacle(42, 40, 2, 6);
            AddObstacle(42, 46, 10, 2);
            AddObstacle(50, 48, 2, 4);
            AddObstacle(42, 66, 2, 6);
            AddObstacle(42, 64, 10, 2);
            AddObstacle(50, 60, 2, 4);
        }
    }
}