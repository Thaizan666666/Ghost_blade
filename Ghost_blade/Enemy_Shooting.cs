using System;
using System.Collections.Generic;
using System.Diagnostics;
using _321_Lab05_3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ghost_blade
{
    public class Enemy_Shooting : Enemy
    {
        private enum EnemyState
        {
            Idle,
            Walking
        }

        private EnemyState currentState;
        private float stateTimer;

        private const float COOLDOWN_DURATION = 2.0f;

        private const int BURST_COUNT = 3;
        private const float BURST_DELAY = 0.15f;

        private int bulletsShotInBurst;
        private float burstTimer;

        private AnimatedTexture EnemyShooting_Idle;
        private AnimatedTexture EnemyShooting_Walk;
        public Texture2D bulletTexture;
        private float fireTimer;
        private const float FIRE_RATE = 1.0f;
        private float fleeRadius = 500f;

        public Action<Bullet> OnShoot;

        public Enemy_Shooting(AnimatedTexture EnemyShooting_Idle, AnimatedTexture EnemyShooting_Walk, Texture2D texture, Vector2 startPosition, float speed, float detectionRadius, Texture2D bulletTexture)
            : base(texture, startPosition, speed, detectionRadius)
        {
            this.Health = 140;
            this.currentState = EnemyState.Idle;
            this.stateTimer = 0f;
            this.EnemyShooting_Idle = EnemyShooting_Idle;
            this.EnemyShooting_Walk = EnemyShooting_Walk;
            this.bulletTexture = bulletTexture;
            this.fireTimer = FIRE_RATE;
            this.IsActive = true;
            this.Speed = speed;
        }

        public override void Update(Player player, List<Rectangle> obstacles, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            EnemyShooting_Idle.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            EnemyShooting_Walk.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            // Priority 1: Check if the enemy is being knocked back
            if (knockbackTimer > 0)
            {
                Position += knockbackDirection * knockbackSpeed * deltaTime * 5f;
                knockbackTimer -= deltaTime;
                return;
            }

            // --- การคำนวณพื้นฐาน ---
            float distance = Vector2.Distance(Position, player.position);
            Vector2 desiredMovement = Vector2.Zero;
            Vector2 directionToPlayer = player.position - Position;

            // =======================================================
            // *** ลอจิกการยิงและการคูลดาวน์ (Fire Logic) ***
            // (ใช้ stateTimer/burstTimer เดิมในการควบคุมการยิง)
            // =======================================================

            // 1. ตรวจสอบคูลดาวน์หลัก
            if (stateTimer > 0)
            {
                stateTimer -= deltaTime;
            }
            // 2. ถ้าไม่คูลดาวน์ และอยู่ในระยะยิง ให้เริ่มยิง
            else if (CanSeePlayer(player, obstacles) && distance <= detectionRadius)
            {
                burstTimer -= deltaTime;

                if (burstTimer <= 0 && bulletsShotInBurst < BURST_COUNT)
                {
                    Shoot(player.position);
                    bulletsShotInBurst++;
                    burstTimer = BURST_DELAY;
                }

                // เมื่อยิงครบ Burst แล้ว: เข้าสู่ Cooldown
                if (bulletsShotInBurst >= BURST_COUNT)
                {
                    stateTimer = COOLDOWN_DURATION;
                    bulletsShotInBurst = 0;
                }
            }
            else
            {
                // ถ้ายิงไม่สำเร็จ (หลุดระยะ/LoS) ให้รีเซ็ต Burst
                bulletsShotInBurst = 0;
            }


            // =======================================================
            // *** ลอจิกการเคลื่อนที่และการกำหนด Animation State ***
            // =======================================================

            // สมมติว่า IsPlayerTooClose (ที่ใช้ fleeRadius) คือเงื่อนไขการถอย
            if (IsPlayerTooClose(player, obstacles))
            {
                // 1. กำหนด Animation State
                currentState = EnemyState.Walking;

                // 2. กำหนดการเคลื่อนที่
                if (directionToPlayer != Vector2.Zero)
                {
                    directionToPlayer.Normalize();
                }
                desiredMovement = -directionToPlayer; // ถอยหลัง
            }
            else
            {
                // 1. กำหนด Animation State
                currentState = EnemyState.Idle;

                // 2. กำหนดการเคลื่อนที่
                desiredMovement = Vector2.Zero; // หยุดนิ่ง
            }

            // =======================================================
            // *** Switch/Case สำหรับ Animation ***
            // (ใช้ currentState ที่กำหนดจากลอจิกการเคลื่อนที่)
            // =======================================================

            switch (currentState)
            {
                case EnemyState.Idle:
                    // เรียกใช้ Animation ยืนนิ่ง (เช่น: sprite.PlayAnimation(idleAnim))
                    // Console.WriteLine("Playing Idle Animation"); // ตัวอย่าง
                    break;

                case EnemyState.Walking:
                    // เรียกใช้ Animation เดิน (เช่น: sprite.PlayAnimation(walkAnim))
                    // Console.WriteLine("Playing Walk Animation"); // ตัวอย่าง
                    break;

                    // คุณสามารถเพิ่ม case อื่น ๆ หรือสถานะ Cooldown ได้ที่นี่ หาก animation แตกต่าง
            }

            // =======================================================
            // *** การอัปเดตตำแหน่งและการตรวจสอบความตาย ***
            // =======================================================

            // ใช้ deltaTime เพื่อให้การเคลื่อนที่คงที่และถูกต้อง
            Position += desiredMovement * Speed;

            // อัปเดต Animation (ถ้ามี)
            // currentSprite.Update(gameTime);

            if (Health <= 0)
            {
                this.Die();
                IsActive = false;
            }
        }

        // Override the TakeDamage method to handle knockback logic
        public override void TakeDamage(int damage, Vector2 damageSourcePosition, bool issword)
        {
            Health -= damage;

            // Calculate knockback direction and start the timer immediately
            knockbackDirection = Position - damageSourcePosition;
            if (knockbackDirection != Vector2.Zero)
            {
                knockbackDirection.Normalize();
            }
            if (issword)
            {
                knockbackTimer = KnockbackDuration;
            }
            else
            {
                knockbackTimer = 0.05f;
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
        public bool IsPlayerTooClose(Player player, List<Rectangle> obstacles)
        {
            // 1. ตรวจสอบระยะห่าง (ใช้ fleeRadius แทน detectionRadius)
            if (Vector2.Distance(this.Position, player.position) > fleeRadius)
            {
                return false; // ไกลเกินกว่าจะถอย
            }

            // 2. ตรวจสอบ Line of Sight (ลอจิกเดียวกับ CanSeePlayer)
            Vector2 lineOfSight = player.position - this.Position;
            Vector2 normalizedDirection = lineOfSight;
            if (normalizedDirection != Vector2.Zero)
            {
                normalizedDirection.Normalize();
            }
            float distance = lineOfSight.Length();
            float stepSize = 5.0f;

            for (float i = 0; i < distance; i += stepSize)
            {
                Vector2 currentPoint = this.Position + normalizedDirection * i;
                Rectangle pointRect = new Rectangle((int)currentPoint.X, (int)currentPoint.Y, 1, 1);

                foreach (var obs in obstacles)
                {
                    if (obs.Intersects(pointRect))
                    {
                        return false; // มีสิ่งกีดขวาง
                    }
                }
            }
            return true; // ใกล้พอและมองเห็น
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                EnemyShooting_Walk.DrawFrame(spriteBatch, Position);
            }
        }
    }
}