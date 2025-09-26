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
            Chasing
        }

        private EnemyState currentState;
        private float stateTimer;

        private const float MOVE_AWAY_DURATION = 1.0f;
        private const float COOLDOWN_DURATION = 2.0f;

        private const int BURST_COUNT = 3;
        private const float BURST_DELAY = 0.15f;

        private int bulletsShotInBurst;
        private float burstTimer;

        public Texture2D bulletTexture;
        private float fireTimer;
        private const float FIRE_RATE = 1.0f;

        public Action<Bullet> OnShoot;

        public Enemy_Shooting(Texture2D texture, Vector2 startPosition, float speed, float detectionRadius, Texture2D bulletTexture)
            : base(texture, startPosition, speed, detectionRadius)
        {
            this.Health = 140;
            this.currentState = EnemyState.Idle;
            this.stateTimer = 0f;
            this.bulletTexture = bulletTexture;
            this.fireTimer = FIRE_RATE;
            this.IsActive = true;
            this.Speed = speed;
        }

        public override void Update(Player player, List<Rectangle> obstacles, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Priority 1: Check if the enemy is being knocked back
            if (knockbackTimer > 0)
            {
                Position += knockbackDirection * knockbackSpeed * deltaTime;
                knockbackTimer -= deltaTime;
                return; // Exit the update method to ignore other logic
            }

            // Priority 2: Normal enemy behavior (only runs if not knocked back)
            float distance = Vector2.Distance(Position, player.position);
            Vector2 desiredMovement = Vector2.Zero;
            Vector2 directionToPlayer = player.position - Position;

            if (CanSeePlayer(player, obstacles))
            {
                if (distance <= detectionRadius)
                {
                    if (currentState != EnemyState.MovingAway && currentState != EnemyState.Cooldown)
                    {
                        currentState = EnemyState.MovingAway;
                        stateTimer = MOVE_AWAY_DURATION;
                        bulletsShotInBurst = 0;
                        burstTimer = BURST_DELAY;
                    }
                }
                else
                {
                    if (currentState != EnemyState.Chasing)
                    {
                        currentState = EnemyState.Chasing;
                    }
                }
            }
            else
            {
                currentState = EnemyState.Idle;
            }

            switch (currentState)
            {
                case EnemyState.Idle:
                    desiredMovement = Vector2.Zero;
                    break;

                case EnemyState.Chasing:
                    if (directionToPlayer != Vector2.Zero)
                    {
                        directionToPlayer.Normalize();
                    }
                    desiredMovement = directionToPlayer;
                    break;

                case EnemyState.MovingAway:
                    if (directionToPlayer != Vector2.Zero)
                    {
                        directionToPlayer.Normalize();
                    }
                    desiredMovement = -directionToPlayer;

                    burstTimer -= deltaTime;
                    if (burstTimer <= 0 && bulletsShotInBurst < BURST_COUNT)
                    {
                        Shoot(player.position);
                        bulletsShotInBurst++;
                        burstTimer = BURST_DELAY;
                    }

                    stateTimer -= deltaTime;
                    if (stateTimer <= 0)
                    {
                        currentState = EnemyState.Cooldown;
                        stateTimer = COOLDOWN_DURATION;
                    }
                    break;

                case EnemyState.Cooldown:
                    desiredMovement = Vector2.Zero;
                    stateTimer -= deltaTime;
                    if (stateTimer <= 0)
                    {
                        currentState = EnemyState.Idle;
                    }
                    break;
            }

            Vector2 newPosition = Position + desiredMovement * Speed;
            Position = newPosition;
        }

        // Override the TakeDamage method to handle knockback logic
        public override void TakeDamage(int damage, Vector2 damageSourcePosition)
        {
            Health -= damage;

            // Calculate knockback direction and start the timer immediately
            knockbackDirection = Position - damageSourcePosition;
            if (knockbackDirection != Vector2.Zero)
            {
                knockbackDirection.Normalize();
            }
            knockbackTimer = KnockbackDuration;

            if (Health <= 0)
            {
                this.IsActive = false;
            }
        }

        private void Shoot(Vector2 targetPosition)
        {
            Vector2 direction = targetPosition - Position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            float offsetDistance = 20f;
            Vector2 startPosition = Position + direction * offsetDistance;

            var bullet = new EnemyBullet(bulletTexture, startPosition, direction, 15f, 0f, 2f);

            OnShoot?.Invoke(bullet);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(Texture, Position, null, Color.Green, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
            }
        }
    }
}