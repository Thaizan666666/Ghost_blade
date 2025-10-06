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
        public MapBoss01(Texture2D bg, Texture2D layer2, AnimatedTexture DoorOpenTexture,
            AnimatedTexture Enemymelee_Idle, AnimatedTexture Enemymelee_Walk, AnimatedTexture Enemymelee_Attack, AnimatedTexture Enemymelee_Death,
            AnimatedTexture EnemyShooting_Idle, AnimatedTexture EnemyShooting_Walk, AnimatedTexture EnemyShooting_Death,
            Texture2D enemyTexture, Texture2D bulletTexture, Texture2D parry,Texture2D Laser)
            : base(bg, layer2, DoorOpenTexture, new Rectangle(0 * 48, 0 * 48, 6 * 48, 2 * 48), new Vector2(0 * 48, 0 * 48),
                  new Vector2(52 * 48, 26 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2)) //
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
            AddObstacle(35, 18, 6, 2);
            AddObstacle(41, 16, 25, 2);
            AddObstacle(63, 18, 6, 2);
            AddObstacle(34, 19, 1, 18);
            AddObstacle(34, 35, 40, 1);
            AddObstacle(69, 19, 1, 18);
            AddObstacle(41, 29, 6, 1.2f);
            AddObstacle(57, 29, 6, 1.2f);
        }
    }
}