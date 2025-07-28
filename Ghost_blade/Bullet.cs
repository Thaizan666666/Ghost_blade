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

        public bool IsActive { get; private set; }

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


            position += velocity * speed;


            currentLifeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;


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