using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Ghost_blade
{
    public class Room3 : Room
    {
        public Room3(Texture2D bg, Texture2D door)
            : base(bg, door, new Rectangle(89 * 24, 37 * 24, 4 * 24, 3 * 24), new Vector2(23 * 24, 96 * 24), new Rectangle(0, 0, 3285, 2970)) //
        {
            NextRooms = new List<int> { 0, 1};
            int tileSize = 24;

            void AddObstacle(int xTile, int yTile, int widthTile, int heightTile)
            {
                Obstacles.Add(new Rectangle(xTile * tileSize, yTile * tileSize, widthTile * tileSize, heightTile * tileSize));
            }

            AddObstacle(40, 26, 28, 33);
            AddObstacle(26, 36, 14, 16);
            AddObstacle(74, 16, 30, 21);
            AddObstacle(74, 37, 15, 3);
            AddObstacle(93, 37, 15, 3);
            AddObstacle(74, 40, 12, 2);
            AddObstacle(77, 42, 3, 12);
            AddObstacle(80, 52, 22, 2);
            AddObstacle(103, 40, 6, 2);
            AddObstacle(108, 39, 1, 30);
            AddObstacle(42, 59, 26, 1);
            AddObstacle(42, 60, 70, 13);
            AddObstacle(26, 72, 30, 16);
            AddObstacle(40, 4, 34, 16);
            AddObstacle(11, 4, 9, 21);
            AddObstacle(20, 7, 8, 2);
            AddObstacle(20, 7, 20, 1);
            AddObstacle(29, 8, 2, 1);
            AddObstacle(34, 8, 2, 1);
            AddObstacle(28, 12, 6, 7);
            AddObstacle(11, 25, 1, 11);
            AddObstacle(11, 36, 9, 16);
            AddObstacle(22, 28, 8, 2);
            AddObstacle(22, 30, 4, 2);
            AddObstacle(11, 52, 1, 50);
            AddObstacle(12, 52, 2, 7);
            AddObstacle(12, 72, 8, 17);
            AddObstacle(12, 89, 4, 20);
            AddObstacle(30, 88, 3, 1);
            AddObstacle(32, 89, 1, 20);
            AddObstacle(16, 106, 20, 1);
            AddObstacle(16, 92, 2, 9);
            AddObstacle(30, 92, 2, 9);
            AddObstacle(18, 56, 8, 2);
            AddObstacle(18, 56, 2, 4);
            AddObstacle(22, 66, 10, 2);
            AddObstacle(30, 60, 2, 8);
        }
    }
}
