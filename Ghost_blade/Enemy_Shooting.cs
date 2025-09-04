// ในไฟล์ Enemy_Shooting.cs
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

        public Enemy_Shooting(Texture2D texture, Vector2 startPosition, float speed, float detectionRadius, Texture2D bulletTexture)
            : base(texture, startPosition, speed, detectionRadius)
        {
            this.currentState = EnemyState.Idle;
            this.stateTimer = 0f;
            this.bulletTexture = bulletTexture;
            this.fireTimer = FIRE_RATE;
        }

        // *** แก้ไข: เมธอด Update นี้จะเรียกใช้เมธอด Update ของคลาสแม่ที่ถูกต้อง
        public override void Update(Vector2 playerPosition, List<Rectangle> obstacles)
        {
            float distance = Vector2.Distance(Position, playerPosition);

            direction = playerPosition - Position;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            Vector2 desiredMovement = Vector2.Zero; // ต้องประกาศและกำหนดค่าที่นี่

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
                    desiredMovement = -direction;
                    stateTimer -= 1 / 60f;
                    if (stateTimer <= 0)
                    {
                        currentState = EnemyState.Shooting;
                        stateTimer = SHOOTING_DURATION;
                    }
                    break;

                case EnemyState.Shooting:
                    fireTimer -= 1 / 60f;
                    if (fireTimer <= 0)
                    {
                        // โค้ดสำหรับสร้างกระสุน
                        fireTimer = FIRE_RATE;
                    }

                    stateTimer -= 1 / 60f;
                    if (stateTimer <= 0)
                    {
                        currentState = EnemyState.Cooldown;
                        stateTimer = COOLDOWN_DURATION;
                    }
                    break;

                case EnemyState.Cooldown:
                    stateTimer -= 1 / 60f;
                    if (stateTimer <= 0)
                    {
                        currentState = EnemyState.Idle;
                    }
                    break;
            }

            // *** แก้ไข: เรียกใช้เมธอด Update ของคลาสแม่ด้วย desiredMovement ที่คำนวณได้
            Vector2 newPosition = Position + desiredMovement * Speed;
            Position = HandleCollision(newPosition, obstacles);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.Green, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}