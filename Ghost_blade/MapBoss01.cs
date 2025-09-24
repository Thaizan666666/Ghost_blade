using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Ghost_blade
{
    public class MapBoss01 : Room
    {
        public MapBoss01(Texture2D bg, Texture2D door, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, door, new Rectangle(36 * 48, 46 * 48, 6 * 48, 2 * 48), new Vector2(40 * 48, 40 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2)) //
        {
            NextRooms = new List<int> { 4, 5 };
            int tileSize = 48;

            void AddObstacle(int xTile, int yTile, int widthTile, int heightTile)
            {
                Obstacles.Add(new Rectangle(xTile * tileSize, yTile * tileSize, widthTile * tileSize, heightTile * tileSize));
            }
            AddObstacle(8, 2, 2, 46);
            AddObstacle(8, 2, 64, 2);
            AddObstacle(70, 2, 2, 46);
            AddObstacle(8, 46, 28, 2);
            AddObstacle(42, 46, 30, 2);
        }
    }
}
