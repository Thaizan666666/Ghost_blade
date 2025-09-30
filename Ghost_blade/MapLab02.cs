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
    public class MapLab02 : Room
    {
        public MapLab02(Texture2D bg, Texture2D layer2, AnimatedTexture DoorOpenTexture, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, layer2, DoorOpenTexture, new Rectangle(39 * 48, 17 * 48, 3 * 48, 1 * 48), new Vector2(39 * 48, 14 * 48),
                  new Vector2(95 * 48, 95 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2)) //
        {
            NextRooms = new List<int> { 4, 6 };
            int tileSize = 48;

            void AddObstacle(float xTile, float yTile, float widthTile, float heightTile)
            {
                float x = xTile * tileSize;
                float y = (yTile) * tileSize;
                float w = widthTile * tileSize;
                float h = (heightTile) * tileSize;

                Obstacles.Add(new Rectangle((int)MathF.Round(x), (int)MathF.Round(y), (int)MathF.Round(w), (int)MathF.Round(h)));
            }

            AddObstacle(89, 101, 17, 2);
            AddObstacle(106, 81, 2, 25);
            AddObstacle(88, 81, 2, 25);
            AddObstacle(100, 71, 17, 11);
            AddObstacle(79, 71, 15, 11);
            AddObstacle(66, 69, 14, 13);
            AddObstacle(44, 81, 24, 11);
            AddObstacle(19, 81, 19, 11);
            AddObstacle(50, 91, 2, 14);
            AddObstacle(30, 91, 2, 14);
            AddObstacle(31, 103, 20, 2);
            AddObstacle(112, 26, 4, 12);
            AddObstacle(106, 26, 6, 3);
            AddObstacle(103, 26, 3, 12);
            AddObstacle(80, 38, 24, 20);
            AddObstacle(116, 37, 2, 35);
            AddObstacle(66, 40, 14, 20);
            AddObstacle(46, 37, 20, 11);
            AddObstacle(19, 37, 21, 11);
            AddObstacle(19, 47, 1, 35);
            AddObstacle(25, 17, 1, 21);
            AddObstacle(58, 17, 1, 21);
            AddObstacle(26, 16, 13, 2);
            AddObstacle(42, 16, 17, 2);
            AddObstacle(90, 82, 2, 0.5f);
            AddObstacle(39, 16, 3, 1);
            AddObstacle(110.3f, 42.8f, 3.5f, 1.2f);
            AddObstacle(108.5f, 55.2f, 3.3f, 2.6f);
            AddObstacle(94.3f, 62.9f, 3.3f, 2.7f);
            AddObstacle(102, 95.8f, 5, 5);
            AddObstacle(101, 97, 1, 5);
            AddObstacle(38, 99.5f, 1, 1.5f);
            AddObstacle(40, 99.5f, 2, 1.5f);
            AddObstacle(43, 99.5f, 1, 1.5f);
            AddObstacle(48, 93.5f, 1, 3.5f);
            AddObstacle(32, 94.9f, 3.5f, 1.1f);
            AddObstacle(32, 97.5f, 2.5f, 1.5f);
            AddObstacle(52, 72.5f, 6, 1.5f);
            AddObstacle(56, 68.6f, 2, 5.4f);
            AddObstacle(28, 72.5f, 6, 1.5f);
            AddObstacle(28, 68.6f, 2, 5.4f);
            AddObstacle(28, 54.5f, 6, 1.5f);
            AddObstacle(28, 54.6f, 2, 5.4f);
            AddObstacle(52, 54.5f, 6, 1.5f);
            AddObstacle(56, 54.6f, 2, 5.4f);
            AddObstacle(34, 25.8f, 4, 2.2f);
            AddObstacle(46, 25.8f, 4, 2.2f);
            AddObstacle(26, 18, 4, 2);
            AddObstacle(55, 18, 3, 1);
            AddObstacle(55, 19, 3, 1);
            AddObstacle(5, 19, 3, 1);
            AddObstacle(26, 33, 2, 5);
            AddObstacle(80, 58, 1.5f, 2);
            AddObstacle(28, 33.5f, 1.5f, 5);
            AddObstacle(54.5f, 33.8f, 5, 5);

            AddEnemy(new Enemy_Melee(enemyTexture, new Vector2(41 * tileSize, 63 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(enemyTexture, new Vector2(42 * tileSize, 26 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(64 * tileSize, 64 * tileSize), 1.0f, 1000f, bulletTexture));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(104 * tileSize, 90 * tileSize), 1.0f, 1000f, bulletTexture));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(96 * tileSize, 90 * tileSize), 1.0f, 1000f, bulletTexture));
        }
    }
}