using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using _321_Lab05_3;


namespace Ghost_blade
{
    public class MapBoss01 : Room
    {
        public MapBoss01(Texture2D bg, Texture2D layer2, AnimatedTexture DoorOpenTexture, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, layer2, DoorOpenTexture, new Rectangle(36 * 48, 46 * 48, 6 * 48, 2 * 48), new Vector2(72 * 48, 17 * 48), 
                  new Vector2(40 * 48, 40 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2)) //
        {
            NextRooms = new List<int> { 4, 5 };
            int tileSize = 48;

            void AddObstacle(float xTile, float yTile, float widthTile, float heightTile)
            {
                float x = xTile * tileSize;
                float y = (yTile) * tileSize;
                float w = widthTile * tileSize;
                float h = (heightTile) * tileSize;

                Obstacles.Add(new Rectangle((int)MathF.Round(x), (int)MathF.Round(y), (int)MathF.Round(w), (int)MathF.Round(h)));
            }

        }
    }
}
