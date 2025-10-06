using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
// สมมติว่า Player, Boss, BossAttack, Enemy อยู่ใน namespace ที่เหมาะสม
// ต้องแน่ใจว่า class Player มีคุณสมบัติ .position และ .HitboxgetDamage
// และมีเมธอด TakeDamage(int damage)

namespace Ghost_blade
{
    // คลาสที่สืบทอดมาจาก Enemy
    public class EnemyLaser : Enemy
    {
        // === ตัวแปรสำหรับสถานะการโจมตี ===
        private enum LaserState { Charging, Firing, CoolingDown };
        private LaserState currentAttackState;

        // === ตัวแปรจับเวลา ===
        private float chargeTimer;
        private float fireTimer;
        private float damageCooldownTimer = 0f;

        // === ค่าคงที่ของ Laser Attack Logic ===
        private const float CHARGE_DURATION = 1.0f; // เวลาชาร์จ
        private const float FIRE_DURATION = 3.0f;    // เวลาที่ยิงเลเซอร์
        private const float ATTACK_COOLDOWN_DURATION = 2.0f; // หน่วงเวลาระหว่างรอบการโจมตี

        // === ค่าคงที่ของ Damage/Collision ===
        private const float DAMAGE_COOLDOWN_DURATION = 0.2f; // โจมตีผู้เล่นซ้ำทุก 0.2 วิ
        private const int LASER_DAMAGE = 1;

        // === ตัวแปรเลเซอร์ ===
        private float laserAngle;
        private const float LASER_THICKNESS_CHARGE = 4f;
        private const float LASER_THICKNESS_FIRE = 8f;
        private const float LASER_LENGTH = 1500f;

        // === ค่าคงที่ควบคุมการเล็ง (Aiming) ===
        private const float AIMING_SPEED_CHARGING = 10f; // เล็งเร็ว (ตามผู้เล่นขณะชาร์จ)
        private const float AIMING_SPEED_FIRING = 0.5f;   // เล็งช้ามาก (ขณะยิง)

        private readonly Texture2D pixelTexture;

        public EnemyLaser(Texture2D texture, Vector2 startPosition, float speed, float detectionRadius, Texture2D pixelTexture)
            : base(texture, startPosition, speed, detectionRadius)
        {
            this.pixelTexture = pixelTexture;
            this.currentAttackState = LaserState.CoolingDown;
            this.attackTimer = ATTACK_COOLDOWN_DURATION;
            this.Health = 50;
        }

        public override void Update(Player player, List<Rectangle> obstacles, GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // 1. อัปเดต Damage Cooldown
            if (damageCooldownTimer > 0)
            {
                damageCooldownTimer -= delta;
            }

            // 2. จัดการ Knockback Timer (ไม่ต้องย้ายตำแหน่ง แต่ต้องให้ Timer นับถอยหลัง)
            if (knockbackTimer > 0)
            {
                // **ไม่ใส่โค้ด Position += ...** ทำให้ไม่เคลื่อนที่
                knockbackTimer -= delta;
            }

            // **[สำคัญ]: ไม่เรียก base.Update() ทำให้ไม่เคลื่อนที่และไม่ไล่ล่า**

            // 3. ตรวจสอบว่าสามารถเห็นผู้เล่นได้
            bool canAttack = CanSeePlayer(player, obstacles);

            // 4. สถานะการโจมตีด้วยเลเซอร์
            switch (currentAttackState)
            {
                case LaserState.CoolingDown:
                    attackTimer -= delta;
                    if (attackTimer <= 0 && canAttack)
                    {
                        StartLaserAttack(player);
                    }
                    break;

                case LaserState.Charging:
                    chargeTimer += delta;
                    AimLaser(player, AIMING_SPEED_CHARGING, delta);

                    if (chargeTimer >= CHARGE_DURATION)
                    {
                        currentAttackState = LaserState.Firing;
                        fireTimer = 0f;
                    }
                    break;

                case LaserState.Firing:
                    fireTimer += delta;
                    AimLaser(player, AIMING_SPEED_FIRING, delta);
                    CheckLaserCollision(player);

                    if (fireTimer >= FIRE_DURATION)
                    {
                        currentAttackState = LaserState.CoolingDown;
                        attackTimer = ATTACK_COOLDOWN_DURATION;
                    }
                    break;
            }
        }

        // เขียนทับ TakeDamage เพื่อให้แน่ใจว่า Knockback จะไม่เกิดขึ้น
        public override void TakeDamage(int damage, Vector2 damageSourcePosition, bool issword)
        {
            // 1. ลดพลังชีวิต
            Health -= damage;

            // 2. ป้องกัน Knockback โดยสมบูรณ์
            knockbackTimer = 0f;

            // 3. ตรวจสอบสถานะการตาย
            if (Health <= 0)
            {
                IsActive = false;
                // เมธอด Die() ควรจัดการ Drop Item และ/หรือตั้งค่า IsActive = false
                Die();
            }
        }

        private void StartLaserAttack(Player player)
        {
            currentAttackState = LaserState.Charging;
            chargeTimer = 0f;
            Vector2 laserCenter = Position;
            laserAngle = (float)Math.Atan2(player.position.Y - laserCenter.Y, player.position.X - laserCenter.X);
        }

        private void AimLaser(Player player, float aimingSpeed, float delta)
        {
            Vector2 laserCenter = Position; // จุดกำเนิดเลเซอร์คือตำแหน่ง Enemy
            float desiredAngle = (float)Math.Atan2(player.position.Y - laserCenter.Y, player.position.X - laserCenter.X);

            // การหมุนแบบนุ่มนวล
            float angleDifference = MathHelper.WrapAngle(desiredAngle - laserAngle);
            laserAngle += angleDifference * aimingSpeed * delta;
        }

        private void CheckLaserCollision(Player player)
        {
            float thickness = LASER_THICKNESS_FIRE;
            Vector2 laserStart = Position;

            Vector2 direction = new Vector2((float)Math.Cos(laserAngle), (float)Math.Sin(laserAngle));
            Vector2 laserEnd = laserStart + direction * LASER_LENGTH;

            // 1. เตรียมข้อมูลผู้เล่น
            Rectangle playerBox = player.HitboxgetDamage;
            Vector2 playerCenter = playerBox.Center.ToVector2();
            float playerRadius = Math.Max(playerBox.Width, playerBox.Height) / 2f;

            // 2. ตรวจสอบจุดที่ใกล้ที่สุดบนเส้นเลเซอร์ (Line-to-Point Projection)
            Vector2 closestPoint = Vector2.Zero;
            float dx = laserEnd.X - laserStart.X;
            float dy = laserEnd.Y - laserStart.Y;
            float lineLengthSquared = dx * dx + dy * dy;

            if (lineLengthSquared != 0)
            {
                float t = ((playerCenter.X - laserStart.X) * dx + (playerCenter.Y - laserStart.Y) * dy) / lineLengthSquared;
                t = MathHelper.Clamp(t, 0, 1);
                closestPoint = new Vector2(laserStart.X + t * dx, laserStart.Y + t * dy);
            }
            else
            {
                closestPoint = laserStart;
            }

            // 3. ตรวจสอบระยะห่างการชน
            float distanceSquared = Vector2.DistanceSquared(playerCenter, closestPoint);
            float hitDistance = thickness / 2f + playerRadius;

            if (distanceSquared <= hitDistance * hitDistance)
            {
                // Collision detected! ตรวจสอบ Damage Cooldown ก่อนโจมตี
                if (damageCooldownTimer <= 0)
                {
                    player.TakeDamage(LASER_DAMAGE);
                    damageCooldownTimer = DAMAGE_COOLDOWN_DURATION;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch); // วาด Enemy Texture

            if (currentAttackState == LaserState.Charging)
            {
                // เลเซอร์กระพริบขณะชาร์จ
                if ((int)(chargeTimer * 8) % 2 == 0)
                {
                    DrawLaser(spriteBatch, LASER_THICKNESS_CHARGE, Color.Red * 0.5f);
                }
            }
            else if (currentAttackState == LaserState.Firing)
            {
                // วาดเลเซอร์เต็มกำลัง
                DrawLaser(spriteBatch, LASER_THICKNESS_FIRE, Color.Red);
            }
        }

        private void DrawLaser(SpriteBatch spriteBatch, float thickness, Color color)
        {
            // ตั้งค่า Origin ให้อยู่ตรงกลางของความหนา (แกน Y)
            Vector2 origin = new Vector2(0, 0);

            spriteBatch.Draw(
                pixelTexture,
                Position,
                null,
                color,
                laserAngle,
                origin,
                new Vector2(LASER_LENGTH, thickness),
                SpriteEffects.None,
                0f
            );
        }
    }
}