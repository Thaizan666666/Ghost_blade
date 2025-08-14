using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Ghost_blade
{
    public abstract class Room
    {
        public Texture2D Background { get; protected set; }
        public Texture2D DoorTexture { get; protected set; }
        public Rectangle Door { get; protected set; }
        public Vector2 StartPosition { get; protected set; }
        public List<int> NextRooms { get; protected set; }
        public Rectangle Bounds { get; protected set; }

        // เพิ่ม list สำหรับ obstacles
        public List<Rectangle> Obstacles { get; protected set; }

        public Room(Texture2D background, Texture2D doorTexture, Rectangle door, Vector2 startPosition, Rectangle bounds)
        {
            Background = background;
            DoorTexture = doorTexture;
            Door = door;
            StartPosition = startPosition;
            Bounds = bounds;
            NextRooms = new List<int>();
            Obstacles = new List<Rectangle>(); // สร้าง list ว่างสำหรับ obstacle
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, new Rectangle(0, 0, 1920, 1080), Color.White);
            if (DoorTexture != null) spriteBatch.Draw(DoorTexture, Door, Color.White);

            // วาดสิ่งกีดขวางสีเทา
            foreach (var rect in Obstacles)
            {
                Texture2D tex = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                tex.SetData(new[] { Color.Gray });
                spriteBatch.Draw(tex, rect, Color.White);
            }
        }
    }

}
