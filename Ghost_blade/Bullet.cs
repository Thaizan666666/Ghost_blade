using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ghost_blade
{
    public class Bullet
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private float rotation;
        private float speed;
        private float lifeTime;
        private float currentLifeTime;
        private readonly int hitboxSize = 10;

        public bool IsActive { get; private set; }

        public Rectangle boundingBox
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, hitboxSize, hitboxSize);
            }
        }

        public Bullet(Texture2D texture, Vector2 startPosition, Vector2 direction, float bulletSpeed, float bulletRotation, float lifeDuration)
        {
            this.texture = texture;
            this.position = startPosition;
            this.velocity = direction;
            this.speed = bulletSpeed;
            this.rotation = bulletRotation;
            this.lifeTime = lifeDuration;
            this.currentLifeTime = 0f;
            this.IsActive = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            // Update position based on velocity and speed, using delta time for smooth movement.
            position += velocity * speed * (float)gameTime.ElapsedGameTime.TotalSeconds * 60f;

            // Update lifetime.
            currentLifeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Deactivate the bullet if its lifetime has expired.
            if (currentLifeTime >= lifeTime)
            {
                IsActive = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;

            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            spriteBatch.Draw(
                texture,
                position,
                null,
                Color.White,
                rotation,
                origin,
                1.0f,
                SpriteEffects.None,
                0f
            );
        }
    }
}