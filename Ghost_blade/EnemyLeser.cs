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
    public class EnemyLeser : Enemy
    {
        public Vector2 Direction { get; private set; }
        public int Damage { get; private set; }

        // === Lifetime Management ===
        private float lifeSpan;   // Maximum time the laser can exist (e.g., 2.0 seconds)
        private float lifeTimer;  // Current time left

        public new Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)(Position.X - Texture.Width / 2),
                    (int)(Position.Y - Texture.Height / 2),
                    Texture.Width,
                    Texture.Height
                );
            }
        }

        public EnemyLeser(Texture2D texture, Vector2 position, Vector2 direction, float speed, int damage, float lifeSpan)

            : base(texture, position, speed, 0f)
        {
            this.Damage = damage;
            this.lifeSpan = lifeSpan;
            this.lifeTimer = lifeSpan;

            this.Direction = direction;
            if (direction != Vector2.Zero)
            {
                this.Direction.Normalize();
            }

            this.Health = 1;
        }
        public override void Update(Player player, List<Rectangle> obstacles, GameTime gameTime)
        {
            if (!IsActive) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Direction * Speed * deltaTime;

            lifeTimer -= deltaTime;
            if (lifeTimer <= 0)
            {
                IsActive = false;
            }

            // NOTE: We do NOT call base.Update() to skip inherited enemy movement/chase logic.
        }

        public override void TakeDamage(int damage, Vector2 damageSourcePosition, bool issword)
        {
            // Lasers should not take damage. They are simply destroyed when they hit something.
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                // Calculate the rotation angle in radians based on the direction vector
                float rotation = (float)Math.Atan2(Direction.Y, Direction.X);

                spriteBatch.Draw(
                    Texture,
                    Position,
                    null,
                    Color.White,
                    rotation, // Rotation calculated above
                    new Vector2(Texture.Width / 2, Texture.Height / 2), // Origin (center the sprite)
                    1.0f, // Scale
                    SpriteEffects.None,
                    0f
                );
            }
        }
        public void OnHit()
        {
            IsActive = false;
        }
    }
}
