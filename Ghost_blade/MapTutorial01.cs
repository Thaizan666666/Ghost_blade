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
    public class MapTutorial01 : Room
    {
        public MapTutorial01(Texture2D bg, Texture2D layer2, AnimatedTexture DoorOpenTexture,
            AnimatedTexture Enemymelee_Idle, AnimatedTexture Enemymelee_Walk, AnimatedTexture Enemymelee_Attack, AnimatedTexture Enemymelee_Death,
            AnimatedTexture EnemyShooting_Idle, AnimatedTexture EnemyShooting_Walk, AnimatedTexture EnemyShooting_Death,
            Texture2D enemyTexture, Texture2D bulletTexture, Texture2D parry)
            : base(bg, layer2, DoorOpenTexture, new Rectangle(90 * 48, 71 * 48, 6 * 48, 1 * 48), new Vector2(0 * 48, 0 * 48),
                  new Vector2(22 * 48, 24 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2))
        {
            NextRooms = new List<int> { };
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
            AddObstacle(12, 37, 80, 11);
            AddObstacle(12, 35, 34, 1);
            AddObstacle(32, 29, 14, 8);
            AddObstacle(68, 29, 14, 8);
            AddObstacle(12, 12, 34, 6);
            AddObstacle(12, 18, 2, 18);
            AddObstacle(32, 18, 14, 4);
            AddObstacle(46, 12, 60, 2);
            AddObstacle(68, 14, 14, 8);
            AddObstacle(104, 14, 2, 58);
            AddObstacle(98, 37, 6, 11);
            AddObstacle(82, 48, 6, 1);
            AddObstacle(96, 71, 10, 2);
            AddObstacle(80, 71, 10, 2);
            AddObstacle(89, 72, 8, 2);
            AddObstacle(80, 48, 2, 30);
            AddObstacle(30, 21.4f, 1, 1);
            AddObstacle(67, 21.3f, 1, 1);
            AddObstacle(67, 30.7f, 1, 1);
        }
    }
}