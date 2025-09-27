using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class MapLab01 : Room
    {
        int tileSize = 48;
        private Texture2D overlayTexture;

        public MapLab01(Texture2D bg, Texture2D door, Texture2D enemyTexture, Texture2D bulletTexture)
            : base(bg, door, new Rectangle(109 * 48, 47 * 48, 4 * 48, 3 * 48), new Vector2(15 * 48, 15 * 48), new Rectangle(0, 0, 3285*2, 2970*2))
        {
            NextRooms = new List<int> { 5,6 };

            void AddObstacle(int xTile, int yTile, int widthTile, int heightTile)
            {
                Obstacles.Add(new Rectangle(xTile * tileSize, yTile * tileSize, widthTile * tileSize, heightTile * tileSize));
            }

            // ... (Your obstacle code remains the same) ...
            AddObstacle(20, 7, 24, 7);
            AddObstacle(3, 0, 41, 8);
            AddObstacle(3, 0, 1, 30);
            AddObstacle(6, 8, 6, 1);
            AddObstacle(13, 8, 2, 1);
            AddObstacle(16, 8, 2, 1);
            AddObstacle(19, 13, 1, 1);
            AddObstacle(19, 21, 1, 1);
            AddObstacle(5, 12, 2, 1);
            AddObstacle(5, 15, 2, 1);
            AddObstacle(5, 19, 2, 1);
            AddObstacle(44, 4, 44, 2);
            AddObstacle(86, 5, 2, 28);
            
            AddObstacle(74, 55, 24, 9);
            AddObstacle(98, 46, 4, 8);
            AddObstacle(98, 54, 2, 1);
            AddObstacle(44, 6, 1, 3);
            AddObstacle(74, 6, 1, 1);
            AddObstacle(50, 12, 4, 2);
            AddObstacle(70, 11, 4, 1);
            AddObstacle(71, 12, 2, 1);
            AddObstacle(70, 24, 4, 2);
            AddObstacle(53, 22, 1, 1);
            AddObstacle(50, 23, 4, 1);
            AddObstacle(51, 24, 2, 1);
            AddObstacle(82, 13, 3, 1);
            AddObstacle(82, 16, 3, 1);
            AddObstacle(82, 20, 3, 1);
            AddObstacle(74, 54, 3, 1);
            
            AddObstacle(35, 45, 10, 10);
            AddObstacle(48, 61, 16, 3);
            AddObstacle(42, 72, 4, 2);
            AddObstacle(35, 55, 1, 30);


            // Add enemies specific to this room
            AddEnemy(new Enemy_Melee(enemyTexture, new Vector2(60 * tileSize, 10 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(enemyTexture, new Vector2(60 * tileSize, 23 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(80 * tileSize, 17 * tileSize), 1.0f, 1000f, bulletTexture));
            AddEnemy(new Enemy_Melee(enemyTexture, new Vector2(44 * tileSize, 60 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(enemyTexture, new Vector2(70 * tileSize, 60 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(55 * tileSize, 70 * tileSize), 1.0f, 1000f, bulletTexture));
            AddEnemy(new Enemy_Melee(enemyTexture, new Vector2(105* tileSize, 55 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Melee(enemyTexture, new Vector2(105 * tileSize, 75 * tileSize), 1.5f, 1000f));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(120 * tileSize, 75 * tileSize), 1.0f, 1000f, bulletTexture));
            AddEnemy(new Enemy_Shooting(enemyTexture, new Vector2(120 * tileSize, 55 * tileSize), 1.0f, 1000f, bulletTexture));
        }
        public override void LoadContent(ContentManager content)
        {
            // โหลด asset พิเศษเฉพาะ MapLab01
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (overlayTexture != null)
            {
                spriteBatch.Draw(overlayTexture, new Vector2(20 * tileSize, 7 * tileSize), new Rectangle(0, 0, 100, 100), Color.White);
            }
        }

    }
}