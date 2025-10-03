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
    public class MapCity01 : Room
    {
        public MapCity01(Texture2D bg, Texture2D layer2, AnimatedTexture DoorOpenTexture,
            AnimatedTexture Enemymelee_Idle, AnimatedTexture Enemymelee_Walk, AnimatedTexture Enemymelee_Attack,
            AnimatedTexture EnemyShooting_Idle, AnimatedTexture EnemyShooting_Walk,
            Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, layer2, DoorOpenTexture, new Rectangle(72 * 48, 17 * 48, 6 * 48, 1 * 48), new Vector2(72 * 48, 14 * 48),
                  new Vector2(113 * 48, 17 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2))
        {
            NextRooms = new List<int> { 2, 3 };
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
            AddObstacle(80, 29, 30, 27);
            AddObstacle(104, 10, 1, 1);
            AddObstacle(54, 47, 26, 19);
            AddObstacle(116, 29, 4, 69);
            AddObstacle(103, 5, 17, 5);
            AddObstacle(103, 9, 1, 21);
            AddObstacle(120, 9, 1, 21);
            AddObstacle(100, 64, 6, 1.5f);
            AddObstacle(102, 67, 2, 1);
            AddObstacle(70, 66, 24, 14);
            AddObstacle(93, 56, 1, 10);
            AddObstacle(93, 97, 25, 2);
            AddObstacle(70, 87, 24, 18);
            AddObstacle(33, 105, 38, 2);
            AddObstacle(27, 47, 21, 19);
            AddObstacle(33, 66, 1, 39);
            AddObstacle(66, 66, 1, 0.5f);
            AddObstacle(38, 43, 6, 4);
            AddObstacle(64, 43, 6, 4);
            AddObstacle(47, 42, 1, 1.3f);
            AddObstacle(47, 22.5f, 1, 1.5f);
            AddObstacle(44, 29, 4, 9);
            AddObstacle(60, 29, 4, 9);
            AddObstacle(27, 15, 45, 3);
            AddObstacle(72, 16, 6, 1);
            AddObstacle(78, 17, 3, 1);
            AddObstacle(80, 17, 1, 13);
            AddObstacle(27, 17, 1, 31);
            AddObstacle(38, 18, 6, 6);
            AddObstacle(64, 18, 6, 6);
            AddObstacle(94, 86.5f, 8, 1);
            AddObstacle(94, 79, 8, 1);
            AddObstacle(98.5f, 76, 1, 1);
            AddObstacle(110.5f, 76, 1, 1);
            AddObstacle(102, 88.8f, 6, 1.2f);
            AddObstacle(106, 88.5f, 1, 1);
            AddObstacle(39, 72.5f, 6, 3);
            AddObstacle(39, 83, 6, 2);
            AddObstacle(38, 82.2f, 2, 0.9f);
            AddObstacle(57, 72.5f, 6, 3);
            AddObstacle(63, 74.5f, 1, 1);
            AddObstacle(57.5f, 83, 6, 2);
            AddObstacle(39, 94.5f, 6, 3);
            AddObstacle(57.5f, 95, 6, 2);

        }
    }
}