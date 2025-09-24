using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class MapCity03 : Room
    {
        public MapCity03(Texture2D bg, Texture2D door, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, door, new Rectangle(84 * 48, 24 * 48, 6 * 48, 2 * 48), new Vector2(22 * 48, 20 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2))
        {
            NextRooms = new List<int> { 1, 2 };
            int tileSize = 48;

            void AddObstacle(int xTile, int yTile, int widthTile, int heightTile)
            {
                Obstacles.Add(new Rectangle(xTile * tileSize, yTile * tileSize, widthTile * tileSize, heightTile * tileSize));
            }

            // ... (Your obstacle code remains the same) ...
            AddObstacle(4, 32, 16, 20);
            AddObstacle(4, 52, 2, 22);
            AddObstacle(30, 12, 44, 40);
            AddObstacle(26, 32, 4, 20);
            AddObstacle(42, 52, 24, 8);
            AddObstacle(64, 42, 20, 14);
            AddObstacle(12, 12, 18, 2);
            AddObstacle(12, 12, 2, 20);
            AddObstacle(4, 72, 82, 10);
            AddObstacle(42, 66, 24, 6);
            AddObstacle(76, 82, 2, 26);
            AddObstacle(76, 104, 28, 2);
            AddObstacle(90, 72, 16, 10);
            AddObstacle(100, 80, 2, 28);
            AddObstacle(74, 24, 10, 2);
            AddObstacle(90, 24, 14, 2);
            AddObstacle(100, 24, 6, 32);
            AddObstacle(102, 54, 2, 20);
            AddObstacle(90, 42, 12, 14);
            AddObstacle(22, 58, 4, 6);
            AddObstacle(20, 60, 8, 2);
            AddObstacle(72, 58, 4, 4);
            AddObstacle(90, 64, 4, 4);
            AddObstacle(86, 34, 4, 2);
        }
    }
}