using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class Enemy_Shooting : Enemy
    {
        private enum EnemyState
        {
            Idle,
            MovingAway,
            Cooldown,
            Shooting
        }

        private EnemyState currentState;
        private float stateTimer;

        private const float MOVE_AWAY_DURATION = 1.0f;
        private const float COOLDOWN_DURATION = 2.0f;
        private const float SHOOTING_DURATION = 0.5f;

        public Texture2D bulletTexture;
        private float fireTimer;
        private const float FIRE_RATE = 1.0f;

        // An event that fires when a bullet is shot.
        public Action<Bullet> OnShoot;

        public Enemy_Shooting(Texture2D texture, Vector2 startPosition, float speed, float detectionRadius, Texture2D bulletTexture)
            : base(texture, startPosition, speed, detectionRadius)
        {
            this.currentState = EnemyState.Idle;
            this.stateTimer = 0f;
            this.bulletTexture = bulletTexture;
            this.fireTimer = FIRE_RATE;
        }

        public override void Update(Player player, List<Rectangle> obstacles)
        {
            float deltaTime = 1f / 60f;
            float distance = Vector2.Distance(Position, player.position);

            Vector2 desiredMovement = Vector2.Zero;

            switch (currentState)
            {
                case EnemyState.Idle:
                    if (distance <= detectionRadius)
                    {
                        currentState = EnemyState.MovingAway;
                        stateTimer = MOVE_AWAY_DURATION;
                    }
                    break;

                case EnemyState.MovingAway:
                    Vector2 directionToPlayer = player.position - Position;
                    if (directionToPlayer != Vector2.Zero)
                    {
                        directionToPlayer.Normalize();
                    }
                    desiredMovement = -directionToPlayer; // Move away from the player

                    stateTimer -= deltaTime;
                    if (stateTimer <= 0)
                    {
                        currentState = EnemyState.Shooting;
                        stateTimer = SHOOTING_DURATION;
                    }
                    break;

                case EnemyState.Shooting:
                    fireTimer -= deltaTime;
                    if (fireTimer <= 0)
                    {
                        Shoot(player.position);
                        fireTimer = FIRE_RATE;
                    }

                    stateTimer -= deltaTime;
                    if (stateTimer <= 0)
                    {
                        currentState = EnemyState.Cooldown;
                        stateTimer = COOLDOWN_DURATION;
                    }
                    break;

                case EnemyState.Cooldown:
                    stateTimer -= deltaTime;
                    if (stateTimer <= 0)
                    {
                        currentState = EnemyState.Idle;
                    }
                    break;
            }

            // Apply the desired movement and handle collision.
            Vector2 newPosition = Position + desiredMovement * Speed;
            Position = HandleCollision(newPosition, obstacles);
        }

        private void Shoot(Vector2 targetPosition)
        {
            Vector2 direction = targetPosition - Position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            // Create a new bullet
            var bullet = new Bullet(bulletTexture, Position, direction, 8f, 0f, 2f);
            // Invoke the OnShoot event to let the game know a bullet was created.
            OnShoot?.Invoke(bullet);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw enemy as a green sprite
            spriteBatch.Draw(Texture, Position, null, Color.Green, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}