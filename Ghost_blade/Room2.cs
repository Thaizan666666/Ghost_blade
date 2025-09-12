using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Ghost_blade
{
    public class Room2 : Room
    {
        public Room2(Texture2D bg, Texture2D door)
            : base(bg, door, new Rectangle(800, 200, 50, 100), new Vector2(500, 300), new Rectangle(0, 0, 1920, 1080))
        {
            NextRooms = new List<int> { 0, 2 };
        }
    }
}
