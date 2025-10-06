using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Reflection.Metadata;
using _321_Lab05_3;


namespace Ghost_blade
{
    public class MapLab03 : Room
    {
        int tileSize = 48;
        public MapLab03(Texture2D bg, Texture2D layer2, AnimatedTexture DoorOpenTexture,
            AnimatedTexture Enemymelee_Idle, AnimatedTexture Enemymelee_Walk, AnimatedTexture Enemymelee_Attack, AnimatedTexture Enemymelee_Death,
            AnimatedTexture EnemyShooting_Idle, AnimatedTexture EnemyShooting_Walk, AnimatedTexture EnemyShooting_Death,Texture2D enemyTexture1,
            AnimatedTexture ChargingLaser,AnimatedTexture DeathLaser, Texture2D enemyTexture, Texture2D bulletTexture,Texture2D parry, Texture2D Laser)
            : base(bg, layer2, DoorOpenTexture, new Rectangle(89 * 48, 39 * 48, 4 * 48, 1 * 48), new Vector2(89 * 48, 36 * 48),
                  new Vector2(23 * 48, 96 * 48), new Rectangle(0, 0, 3285 * 2, 2970 * 2)) 
        {
            NextRooms = new List<int> { 4, 5 };

            void AddObstacle(float xTile, float yTile, float widthTile, float heightTile)
            {
                float x = xTile * tileSize;
                float y = (yTile) * tileSize;
                float w = widthTile * tileSize;
                float h = (heightTile) * tileSize;

                Obstacles.Add(new Rectangle((int)MathF.Round(x), (int)MathF.Round(y), (int)MathF.Round(w), (int)MathF.Round(h)));
            }

            AddObstacle(15, 107, 18, 1);
            AddObstacle(11, 73, 9, 15);
            AddObstacle(15, 88, 1, 19);
            AddObstacle(16, 99.5f, 1.5f, 1.5f);
            AddObstacle(16, 97, 1.5f, 1);
            AddObstacle(16, 95, 1.5f, 1);
            AddObstacle(16, 93, 1.5f, 1);
            AddObstacle(30.5f, 99.5f, 1.5f, 1.5f);
            AddObstacle(30.5f, 97, 1.5f, 1);
            AddObstacle(30.5f, 95, 1.5f, 1);
            AddObstacle(30.5f, 93, 1.5f, 1);
            AddObstacle(32, 84, 1, 23);
            AddObstacle(26, 73, 24, 15);
            AddObstacle(22, 66.5f, 10, 1.5f);
            AddObstacle(30, 61, 2, 7);
            AddObstacle(18, 56.5f, 8, 1.5f);
            AddObstacle(18, 58, 2, 2);
            AddObstacle(11, 58, 1, 16);
            AddObstacle(12, 50, 2, 9);
            AddObstacle(11, 37, 9, 15);
            AddObstacle(26, 37, 14, 15);
            AddObstacle(40, 51, 2, 8);
            AddObstacle(42, 56, 1, 17);
            AddObstacle(39, 52, 1, 1);
            AddObstacle(12, 2, 8, 22);
            AddObstacle(11, 23, 1, 14);
            AddObstacle(22, 30, 4, 2);
            AddObstacle(22, 29, 8, 1);
            AddObstacle(28, 13.5f, 6, 3.5f);
            AddObstacle(20, 3, 20, 5);
            AddObstacle(20, 8, 8, 1);
            AddObstacle(40, 6, 35, 14);
            AddObstacle(40, 27, 28, 21);
            AddObstacle(67, 48, 1, 12);
            AddObstacle(74, 20, 15, 20);
            AddObstacle(93, 20, 15, 20);
            AddObstacle(74, 40, 3, 2);
            AddObstacle(77, 40, 3, 14);
            AddObstacle(80, 52.9f, 22, 1.1f);
            AddObstacle(67, 60, 1, 1);
            AddObstacle(67, 61, 41, 1);
            AddObstacle(108, 37, 1, 25);
            AddObstacle(80, 40, 5, 2);
            AddObstacle(85, 40, 1, 1);
            AddObstacle(104, 40, 5, 2);
            AddObstacle(89, 38, 4, 1);

            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(16 * tileSize, 67 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(39 * tileSize, 69 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(36 * tileSize, 58 * tileSize), 1.5f, 1000f));
            AddEnemy(new EnemyLaser(enemyTexture1, enemyTexture, ChargingLaser, DeathLaser, enemyTexture, new Vector2(26 * tileSize, 61 * tileSize), 1.0f, 1000f, Laser));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(19 * tileSize, 30 * tileSize), 1.5f, 1000f));
            AddEnemy(new EnemyLaser(enemyTexture1, enemyTexture, ChargingLaser, DeathLaser, enemyTexture, new Vector2(27 * tileSize, 24 * tileSize), 1.0f, 1000f, Laser));
            AddEnemy(new Enemy_Shooting(EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, enemyTexture, new Vector2(24 * tileSize, 16 * tileSize), 1.0f, 1000f, bulletTexture, parry));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(39 * tileSize, 23 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(76 * tileSize, 57 * tileSize), 1.5f, 1000f));
            AddEnemy(new EnemyLaser(enemyTexture1, enemyTexture, ChargingLaser, DeathLaser, enemyTexture, new Vector2(105 * tileSize, 57 * tileSize), 1.0f, 1000f, Laser));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(98 * tileSize, 44 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, enemyTexture, new Vector2(83 * tileSize, 45 * tileSize), 1.5f, 1000f));
        }
    }
}