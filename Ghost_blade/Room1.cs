using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class Room1 : Room
    {
        public Room1(Texture2D bg, Texture2D door)
            : base(bg, door, new Rectangle(550, 300, 50, 100), new Vector2(100, 300), new Rectangle(0, 0, 1920, 1080)) //
        {
            NextRooms = new List<int> { 1, 2 };

            Obstacles.Add(new Rectangle(400, 400, 200, 50));
            Obstacles.Add(new Rectangle(800, 200, 100, 300));

        }
    }
}
