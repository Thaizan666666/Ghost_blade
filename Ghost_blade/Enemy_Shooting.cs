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

        // ระยะเวลาที่ใช้ในการเปลี่ยนสถานะ
        private const float MOVE_AWAY_DURATION = 1.0f;
        private const float COOLDOWN_DURATION = 2.0f;
        private const float SHOOTING_DURATION = 0.5f;

        // ตัวแปรสำหรับระบบยิงแบบเบิร์สต์ (burst fire)
        private const int BURST_COUNT = 3;
        private const float BURST_DELAY = 0.15f;

        private int bulletsShotInBurst;
        private float burstTimer;

        public Texture2D bulletTexture;
        private float fireTimer;
        private const float FIRE_RATE = 1.0f;

        // An event that fires when a bullet is shot.
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
        }


        public override void Update(Player player, List<Rectangle> obstacles)
        {
            // ใช้ gameTime.ElapsedGameTime.TotalSeconds เพื่อค่า deltaTime ที่แม่นยำขึ้น
            float deltaTime = 1f / 60f;
            float distance = Vector2.Distance(Position, player.position);

            Vector2 desiredMovement = Vector2.Zero;

            switch (currentState)
            {
                case EnemyState.Idle:
                    if (distance <= 200f && CanSeePlayer(player, obstacles)) // ตั้งค่า 200f เป็นระยะที่ต้องการให้ถอยหลัง
                    {
                        currentState = EnemyState.MovingAway;
                        stateTimer = MOVE_AWAY_DURATION;
                    }
                    else if (distance <= detectionRadius && CanSeePlayer(player, obstacles))
                    {
                        currentState = EnemyState.Shooting;
                        stateTimer = SHOOTING_DURATION;
                        bulletsShotInBurst = 0; // รีเซ็ตตัวนับกระสุนเมื่อเริ่มสถานะยิง
                        burstTimer = BURST_DELAY; // เริ่มต้น timer สำหรับนัดแรก
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
                        bulletsShotInBurst = 0;
                        burstTimer = BURST_DELAY;
                    }
                    break;

                case EnemyState.Shooting:
                    burstTimer -= deltaTime;
                    stateTimer -= deltaTime;

                    // ยิงกระสุนถ้านับเวลาได้และยังยิงไม่ครบ 3 นัด
                    if (burstTimer <= 0 && bulletsShotInBurst < BURST_COUNT)
                    {
                        Shoot(player.position);
                        bulletsShotInBurst++;
                        burstTimer = BURST_DELAY; // รีเซ็ต timer สำหรับนัดถัดไป
                    }

                    // ถ้าเวลาในสถานะ Shooting หมด หรือยิงครบ 3 นัดแล้ว ให้เปลี่ยนไปสถานะ Cooldown
                    if (stateTimer <= 0 || bulletsShotInBurst >= BURST_COUNT)
                    {
                        currentState = EnemyState.Cooldown;
                        stateTimer = COOLDOWN_DURATION;
                        bulletsShotInBurst = 0; // รีเซ็ตตัวนับกระสุนสำหรับรอบถัดไป
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

            float offsetDistance = 20f;
            Vector2 startPosition = Position + direction * offsetDistance;

            // สร้าง EnemyBullet
            var bullet = new EnemyBullet(bulletTexture, startPosition, direction, 8f, 0f, 2f);

            OnShoot?.Invoke(bullet);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                spriteBatch.Draw(Texture, Position, null, Color.Green, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
            }
        }
        public override void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                this.IsActive = false;
            }
        }
    }
}