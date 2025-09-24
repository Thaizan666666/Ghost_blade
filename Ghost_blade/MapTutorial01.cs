using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class MapTutorial01 : Room
    {
        public MapTutorial01(Texture2D bg, Texture2D door, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, door, new Rectangle(90 * 48, 70 * 48, 6 * 48, 2 * 48), new Vector2(22 * 48, 24 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2))
        {
            NextRooms = new List<int> { 1, 2, 3};
            int tileSize = 48;

            void AddObstacle(int xTile, int yTile, int widthTile, int heightTile)
            {
                Obstacles.Add(new Rectangle(xTile * tileSize, yTile * tileSize, widthTile * tileSize, heightTile * tileSize));
            }

            // ... (Your obstacle code remains the same) ...
            AddObstacle(12, 36, 80, 12);
            AddObstacle(12, 34, 34, 2);
            AddObstacle(32, 28, 14, 8);
            AddObstacle(68, 28, 14, 8);
            AddObstacle(12, 12, 34, 6);
            AddObstacle(12, 18, 2, 18);
            AddObstacle(32, 18, 14, 4);
            AddObstacle(46, 12, 60, 2);
            AddObstacle(68, 14, 14, 8);
            AddObstacle(104, 14, 2, 58);
            AddObstacle(98, 36, 6, 12);
            AddObstacle(96, 70, 10, 2);
            AddObstacle(80, 70, 10, 2);
            AddObstacle(80, 48, 2, 30);
        }
    }
}