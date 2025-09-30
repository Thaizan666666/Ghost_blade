using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _321_Lab05_3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class MapCity02 : Room
    {
        public MapCity02(Texture2D bg, Texture2D layer2, AnimatedTexture DoorOpenTexture, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, layer2, DoorOpenTexture, new Rectangle(36 * 48, 30 * 48, 6 * 48, 2 * 48), new Vector2(36 * 48, 27 * 48),
                  new Vector2(109 * 48, 101 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2))
        {
            NextRooms = new List<int> { 1, 3 };
            int tileSize = 48;

            void AddObstacle(float xTile, float yTile, float widthTile, float heightTile)
            {
                float x = xTile * tileSize;
                float y = (yTile) * tileSize;
                float w = widthTile * tileSize;
                float h = (heightTile) * tileSize;

                Obstacles.Add(new Rectangle((int)MathF.Round(x), (int)MathF.Round(y), (int)MathF.Round(w), (int)MathF.Round(h)));
            }

            // ... (Your obstacle code remains the same) ...
            AddObstacle(110, 69, 14, 23);
            AddObstacle(116, 91, 4, 24);
            AddObstacle(98, 111, 20, 4);
            AddObstacle(88, 69, 16, 23);
            AddObstacle(98, 92, 2, 24);
            AddObstacle(62, 59, 26, 33);

            AddObstacle(108, 17, 12, 25);
            AddObstacle(116, 4, 10, 38);
            AddObstacle(122, 42, 2, 27);
            AddObstacle(88, 17, 14, 25);
            AddObstacle(62, 30, 26, 22);
            AddObstacle(90, 4, 2, 16);
            AddObstacle(90, 4, 26, 2);

            AddObstacle(98, 49, 12, 1);
            AddObstacle(98, 61, 12, 1);

            AddObstacle(42, 30, 22, 2);
            AddObstacle(28, 30, 8, 2);
            AddObstacle(28, 83, 34, 2);
            AddObstacle(28, 30, 2, 54);
            AddObstacle(42, 41, 2, 2);
            AddObstacle(42.6f, 45, 1, 1);
            AddObstacle(42, 46, 8, 1.5f);
            AddObstacle(50, 46.5f, 1.5f, 2);

            AddObstacle(50, 48, 2, 4);
            AddObstacle(42, 66.5f, 2, 5.5f);
            AddObstacle(44, 64.5f, 8, 1);
            AddObstacle(50, 60.7f, 2, 4);
            AddObstacle(45.5f, 64, 4, 1);

            AddObstacle(100, 108, 4, 4);
            AddObstacle(104.5f, 105.9f, 2, 1);
            AddObstacle(110.5f, 105.9f, 1.5f, 1);
            AddObstacle(106.5f, 106.5f, 5, 1);
            AddObstacle(105.5f, 107, 5, 5);
            AddObstacle(112.5f, 107, 5, 5);
            AddObstacle(48, 32, 6, 1);
            AddObstacle(30, 46, 6, 1.5f);
            AddObstacle(58, 78.8f, 4, 2);
            AddObstacle(59.8f, 77.8f, 4, 2);
            AddObstacle(30, 78.5f, 2.3f, 2);
            AddObstacle(32, 79.5f, 2, 5);
            AddObstacle(92, 12.3f, 3.8f, 1.3f);
            AddObstacle(92, 13, 2.5f, 2);
            AddObstacle(110, 14, 5, 1.5f);
            AddObstacle(120.3f, 61f, 1, 1);
            AddObstacle(93.8f, 49, 1, 1);
        }
    }
}