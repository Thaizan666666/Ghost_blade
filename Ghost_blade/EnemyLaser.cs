using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ghost_blade;
using _321_Lab05_3; // สมมติว่านี่คือ Namespace ของ AnimatedTexture

namespace Ghost_blade
{
    // คลาสนี้ทำหน้าที่เป็นป้อมปืน (Turret) ที่ยิงเลเซอร์และไม่เคลื่อนที่
    public class EnemyLaser : Enemy
    {
        private enum EnemyState
        {
            Idle,
            Charging, // ชาร์จพลังงาน (หันตามและกระพริบ)
            Firing,   // ยิงเลเซอร์หนา (ล็อกเป้าและแสดง Laser.png)
            Cooldown
        }

        private EnemyState currentState;
        private float stateTimer;
        private Vector2 laserDirection; // เก็บเป้าหมายสุดท้ายที่ล็อกไว้

        // === การตั้งค่าการยิงเลเซอร์ ===
        private const float CHARGE_DURATION = 3f;
        private const float PRE_FIRE_DELAY = 0.5f; // NEW: ดีเลย์ $0.5 \text{s}$ ก่อนยิงเพื่อให้ผู้เล่นหลบได้
        private const float FIRING_DURATION = 0.15f;
        private const float ATTACK_COOLDOWN = 4f;
        private const int LASER_DAMAGE_INSTANT = 1; // ความเสียหาย 1 หน่วย ต่อเฟรม ขณะที่อยู่ในเลเซอร์

        // NEW: สำหรับการวาดเส้นเลเซอร์และการคำนวณการชน
        private const float LASER_LENGTH = 2000f; // ความยาวของเส้นเลเซอร์
        private const float LASER_THICKNESS = 10f; // ความหนาของเส้นเลเซอร์

        // (ตัวแปร Animation State และ Texture เหมือนเดิม)
        private bool flip = true;
        private bool isDying = false;
        private bool hasPlayedDeath = false;
        private Texture2D Anim_Idle;
        private Texture2D Anim_Firing;
        private AnimatedTexture Anim_Charging;
        private AnimatedTexture Anim_Death;
        private Vector2 lastMovement = Vector2.Zero;
        private Vector2 TexturePosition;

        // NEW: Texture 1x1 สำหรับวาดเส้นเลเซอร์
        private Texture2D pixelTexture;

        public Action<EnemyLaser, Vector2> OnShootLaser; // ส่งทิศทางที่ล็อกไว้ไปด้วย

        // Helper property เพื่อคำนวณจุดกึ่งกลางของ Turret (จุดเริ่มต้นของเลเซอร์)
        private Vector2 TextureCenter => new Vector2(Anim_Idle.Width / 2f, Anim_Idle.Height / 2f);


        public EnemyLaser(
            Texture2D IdleLaser, Texture2D FiringLaser, AnimatedTexture ChargingLaser,
            AnimatedTexture DeathLaser, Texture2D texture, Vector2 startPosition,
            float speed, float detectionRadius, Texture2D laserProjectileTexture)
            : base(texture, startPosition, speed, detectionRadius)
        {
            this.Health = 250;
            this.currentState = EnemyState.Idle;
            this.stateTimer = 0f;
            this.Anim_Idle = IdleLaser;
            this.Anim_Firing = FiringLaser;
            this.Anim_Charging = ChargingLaser.Clone();
            this.Anim_Death = DeathLaser.Clone();
            this.IsActive = true;
            this.Speed = speed;

            // NEW: เก็บ pixelTexture ที่ใช้ในการวาดเส้น
            this.pixelTexture = laserProjectileTexture;
        }

        public override void Update(Player player, List<Rectangle> obstacles, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Health <= 0 && !hasPlayedDeath)
            {
                this.Die();
                IsActive = false;
                isDying = true;
                Anim_Death.Reset();
            }

            if (stateTimer > 0)
            {
                stateTimer -= deltaTime;
            }

            float distance = Vector2.Distance(Position, player.position);

            // *** State Machine Logic ***
            switch (currentState)
            {
                case EnemyState.Idle:
                case EnemyState.Cooldown:
                    // 1. Idle/Cooldown: หันหน้าตามผู้เล่น
                    UpdateFlipDirection(player);
                    if (CanSeePlayer(player, obstacles) && stateTimer <= 0 && distance <= detectionRadius)
                    {
                        currentState = EnemyState.Charging;
                        stateTimer = CHARGE_DURATION;
                        Anim_Charging.Reset();
                    }
                    else
                    {
                        currentState = EnemyState.Idle;
                    }
                    break;

                case EnemyState.Charging:
                    // 2. Charging:

                    if (stateTimer > PRE_FIRE_DELAY)
                    {
                        // Active Tracking: ติดตามผู้เล่น (Tracking)
                        UpdateFlipDirection(player);
                        laserDirection = player.position - Position - TextureCenter;
                        if (laserDirection != Vector2.Zero) laserDirection.Normalize();
                    }
                    else if (stateTimer <= PRE_FIRE_DELAY && stateTimer > 0)
                    {
                        // Pre-fire Lock: ช่วงดีเลย์ $0.2 \text{s}$ สุดท้าย (ไม่มี Tracking, ล็อกเป้าหมายแล้ว)
                    }

                    Anim_Charging.UpdateFrame(deltaTime);
                    if (stateTimer <= 0)
                    {
                        // ทิศทางสุดท้ายถูกล็อกไว้ในตัวแปร laserDirection แล้ว

                        currentState = EnemyState.Firing;
                        stateTimer = FIRING_DURATION;

                        // *** เรียกเมธอดสร้างเลเซอร์หนา (Hitscan) สำหรับสัญญาณภายนอก ***
                        ShootLaser(laserDirection);
                    }
                    break;

                case EnemyState.Firing:
                    // 3. Firing: ไม่มีการหันหน้า (ล็อกเป้าแล้ว)

                    // NEW: ตรวจสอบการชนและสร้างความเสียหาย
                    ApplyContinuousLaserDamage(player);

                    if (stateTimer <= 0)
                    {
                        currentState = EnemyState.Cooldown;
                        stateTimer = ATTACK_COOLDOWN;
                    }
                    break;
            }
        }

        // NEW: เมธอดสำหรับตรวจสอบการชนและสร้างความเสียหาย (ตามรูปแบบ Hitscan จาก LaserAttack)
        private void ApplyContinuousLaserDamage(Player player)
        {
            if (laserDirection == Vector2.Zero) return;

            Vector2 laserStart = Position + TextureCenter;
            Vector2 direction = laserDirection;
            Vector2 laserEnd = laserStart + direction * LASER_LENGTH;
            float thickness = LASER_THICKNESS;

            // 1. กำหนดจุดศูนย์กลางของผู้เล่นและรัศมี (แบบประมาณ)
            Rectangle playerBox = player.HitboxgetDamage;
            Vector2 playerCenter = playerBox.Center.ToVector2();
            float playerRadius = Math.Max(playerBox.Width, playerBox.Height) / 2f;

            // 2. ตรวจสอบว่าจุดใดบนเส้นเลเซอร์ (L1-L2) อยู่ใกล้ผู้เล่นที่สุด
            Vector2 closestPoint = Vector2.Zero;
            float dx = laserEnd.X - laserStart.X;
            float dy = laserEnd.Y - laserStart.Y;
            float lineLengthSquared = dx * dx + dy * dy;

            if (lineLengthSquared != 0)
            {
                // คำนวณ t (ตัวประกอบการฉายภาพ)
                float t = ((playerCenter.X - laserStart.X) * dx + (playerCenter.Y - laserStart.Y) * dy) / lineLengthSquared;

                // จำกัดค่า t ให้อยู่ระหว่าง 0 ถึง 1 (เพื่อให้จุดใกล้สุดอยู่บนส่วนของเส้นตรง)
                t = MathHelper.Clamp(t, 0, 1);

                // คำนวณจุดที่ใกล้ที่สุดบนส่วนของเส้นตรง
                closestPoint = new Vector2(laserStart.X + t * dx, laserStart.Y + t * dy);

                // 3. ตรวจสอบระยะห่างการชน
                float distanceSquared = Vector2.DistanceSquared(playerCenter, closestPoint);
                float hitDistance = thickness / 2f + playerRadius; // รวมรัศมีการชนของเลเซอร์และผู้เล่น

                if (distanceSquared <= hitDistance * hitDistance)
                {
                    // Collision detected! ลดเลือดผู้เล่น
                    player.TakeDamage(LASER_DAMAGE_INSTANT);
                }
            }
        }

        private void UpdateFlipDirection(Player player)
        {
            // (โค้ดเหมือนเดิม)
            if (player.position.X > Position.X)
            {
                flip = false; // หันขวา
            }
            else if (player.position.X < Position.X)
            {
                flip = true; // หันซ้าย
            }
        }

        private void ShootLaser(Vector2 direction)
        {
            // ส่งสัญญาณไปยัง Game Manager ให้สร้าง Laser Line 
            // โดยใช้ Position และ Direction ที่ล็อกไว้ เพื่อจัดการ Damage และ Draw Laser Line
            // Damage handling ได้ถูกย้ายไปที่ ApplyContinuousLaserDamage แล้ว
            OnShootLaser?.Invoke(this, direction);
        }

        public override void TakeDamage(int damage, Vector2 damageSourcePosition, bool issword)
        {
            base.TakeDamage(damage, damageSourcePosition, issword);
            // เมื่อถูกดาบฟันขณะชาร์จ ให้เข้าสู่สถานะ Cooldown สั้นลง
            if (issword && currentState == EnemyState.Charging)
            {
                currentState = EnemyState.Cooldown;
                stateTimer = ATTACK_COOLDOWN * 0.5f;
            }
        }

        // === Helper method สำหรับวาดเส้นเลเซอร์ (ตามแบบ LaserAttack) ===
        private void DrawLaser(SpriteBatch spriteBatch, float thickness, Color color)
        {
            if (laserDirection == Vector2.Zero) return;

            // 1. คำนวณมุมของเลเซอร์จากทิศทางที่ล็อกไว้
            float laserAngle = (float)Math.Atan2(laserDirection.Y, laserDirection.X);

            // 2. กำหนดจุดเริ่มต้นของเลเซอร์ (จากกึ่งกลางของ Texture)
            Vector2 laserStart = Position + TextureCenter;

            // 3. Draw the line using the 1x1 pixel texture
            spriteBatch.Draw(
                pixelTexture,
                laserStart,
                null,
                color,
                laserAngle,
                // Origin (0, 0.5f) ทำให้เส้นเลเซอร์ถูกวาดโดยมีจุดเริ่มต้นอยู่กึ่งกลางความหนา
                new Vector2(0f, 0.5f),
                new Vector2(LASER_LENGTH, thickness),
                SpriteEffects.None,
                0f
            );
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            TexturePosition = Position - new Vector2(48,48);
            SpriteEffects spriteEffect = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (IsActive) { spriteBatch.Draw(Anim_Idle, TexturePosition, null, Color.White, 0f, Vector2.Zero, 2f, spriteEffect, 0f); }
            if (isDying)
            {
                if (!Anim_Death.IsEnd)
                {
                    Anim_Death.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                    Anim_Death.DrawFrame(spriteBatch, TexturePosition);

                }
                else
                {
                    hasPlayedDeath = true;
                }
            }

            if (!IsActive) return;

            // 2. กำหนดแอนิเมชันตามสถานะ
            if (currentState == EnemyState.Charging)
            {
                // วาด Animation Charging
                Anim_Charging.DrawFrame(spriteBatch, TexturePosition, flip);

                float pulse = (float)Math.Abs(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 8)); // ความเร็วการกระพริบ 8
                Color flashColor = Color.Red * (0.3f + pulse * 0.7f); // Alpha เปลี่ยนจาก 0.3 ถึง 1.0 (กระพริบ)

                DrawLaser(spriteBatch, 2f, flashColor);
            }
            else if (currentState == EnemyState.Firing)
            {
                spriteBatch.Draw(Anim_Firing, TexturePosition, null, Color.White, 0f, Vector2.Zero, 2f, spriteEffect, 0f);

                DrawLaser(spriteBatch, LASER_THICKNESS, Color.Red);
            }
        }

        public override void Reset()
        {
            base.Reset();
            isDying = false;
            hasPlayedDeath = false;
            Health = 250;
            currentState = EnemyState.Idle;
            stateTimer = 0f;
        }
    }
}
