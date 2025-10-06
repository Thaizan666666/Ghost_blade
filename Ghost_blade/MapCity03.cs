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
    public class MapCity03 : Room
    {
        public MapCity03(Texture2D bg, Texture2D layer2, AnimatedTexture DoorOpenTexture,
            AnimatedTexture Enemymelee_Idle, AnimatedTexture Enemymelee_Walk, AnimatedTexture Enemymelee_Attack, AnimatedTexture Enemymelee_Death,
            AnimatedTexture EnemyShooting_Idle, AnimatedTexture EnemyShooting_Walk, AnimatedTexture EnemyShooting_Death,
            Texture2D enemyTexture, Texture2D bulletTexture, Texture2D parry, Texture2D Laser)
            : base(bg, layer2, DoorOpenTexture, new Rectangle(85 * 48, 25 * 48, 4 * 48, 1 * 48), new Vector2(85 * 48, 22 * 48),
                  new Vector2(22 * 48, 20 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2))
        {
            NextRooms = new List<int> { 1, 2 };
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
            AddObstacle(4, 33, 16, 19);
            AddObstacle(26, 33, 4, 19);
            AddObstacle(12, 12, 2, 22);
            AddObstacle(12, 12, 18, 2);
            AddObstacle(4, 73, 82, 9);
            AddObstacle(4, 52, 2, 23);
            AddObstacle(42, 67, 24, 6);
            AddObstacle(90, 73, 16, 9);
            AddObstacle(102, 54, 2, 21);
            AddObstacle(22, 58.5f, 4, 6);
            AddObstacle(76, 105, 28, 2);
            AddObstacle(90, 43, 12, 13);
            AddObstacle(64, 43, 20, 13);
            AddObstacle(20, 60, 8, 2);
            AddObstacle(72, 58.8f, 4, 3);
            AddObstacle(90.5f, 65, 3, 3);
            AddObstacle(86, 34.3f, 3, 1.7f);
            AddObstacle(30, 12, 44, 40);
            AddObstacle(42, 52, 24, 8);
            AddObstacle(76, 82, 2, 26);
            AddObstacle(100, 80, 2, 28);
            AddObstacle(74, 24, 10, 2);
            AddObstacle(90, 24, 14, 2);
            AddObstacle(100, 24, 6, 32);
            AddObstacle(84, 24, 6, 1);
            AddObstacle(98.5f, 70.2f, 5, 1.1f);
            AddObstacle(78.8f, 96, 1.5f, 1.5f);
            AddObstacle(78, 100.5f, 4, 4);
            AddObstacle(82, 102, 1, 5);
            AddObstacle(97, 100.8f, 3, 5);
            AddObstacle(95.3f, 101.5f, 3, 5);
            AddObstacle(93.5f, 102, 3, 5);
            AddObstacle(74, 26, 2, 1);
            AddObstacle(74, 28.5f, 2, 2);
            AddObstacle(84, 25, 1, 1);
            AddObstacle(89, 25, 1, 1);

            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(14 * tileSize, 57 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(35 * tileSize, 57 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Shooting(EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, enemyTexture, new Vector2(27 * tileSize, 68 * tileSize), 1.0f, 1000f, bulletTexture, parry));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(82 * tileSize, 60 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(80 * tileSize, 68 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Shooting(EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, enemyTexture, new Vector2(96 * tileSize, 63 * tileSize), 1.0f, 1000f, bulletTexture, parry));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(85 * tileSize, 90 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(94 * tileSize, 90 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Shooting(EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, enemyTexture, new Vector2(90 * tileSize, 98 * tileSize), 1.0f, 1000f, bulletTexture, parry));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(96 * tileSize, 35 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(79 * tileSize, 35 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(85 * tileSize, 30 * tileSize), 1.5f, 1000f));
        }
    }
}