using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Ghost_blade
{
    public abstract class Room
    {
        protected Texture2D background;
        public Rectangle Bounds { get; private set; }
        public List<Rectangle> Obstacles { get; private set; }
        public Rectangle Door { get; private set; }
        public Vector2 StartPosition { get; private set; }
        public List<int> NextRooms { get; protected set; }
        public List<Enemy> Enemies { get; private set; }
        private Texture2D doorTexture;

        public Room(Texture2D bg, Texture2D door, Rectangle doorRectangle, Vector2 startPosition, Rectangle bounds)
        {
            this.background = bg;
            this.doorTexture = door;
            this.Door = doorRectangle;
            this.StartPosition = startPosition;
            this.Bounds = bounds;
            Obstacles = new List<Rectangle>();
            Enemies = new List<Enemy>();
        }

        public void AddEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }
        public void ResetRoom()
        {
            foreach (var enemy in Enemies)
            {
                enemy.Reset();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, Vector2.Zero, null, Color.White, 0f, Vector2.Zero,2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(doorTexture, Door, Color.Red);

            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            foreach (var rect in Obstacles)
            {
                //spriteBatch.Draw(pixel, rect, Color.Gray * 0.6f); // สีเทาเพื่อให้เห็นชัด
            }
        }
    }
}