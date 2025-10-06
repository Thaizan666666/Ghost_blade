using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ghost_blade
{
    public class LaserAttack : BossAttack
    {
        private enum LaserState { Charging, Firing, CoolingDown };
        private LaserState currentState;

        // Hitbox ถูกเก็บไว้เพื่อวัตถุประสงค์ในการประกาศ แต่เราจะใช้การคำนวณ Line/Point Collision แทน
        public Rectangle Hitbox { get; private set; }

        private float chargeTimer;
        private float fireTimer;
        private float coolDownTimer;
        private float coolDowmRealTimer;

        private const float CHARGE_DURATION = 1.5f;
        private const float FIRE_DURATION = 5.0f;
        private const float COOL_DOWN_DURATION = 1.0f;

        private float laserAngle;
        private float currentRotationSpeed;
        private const float MAX_ROTATION_SPEED = 2f;
        private float rotationDirection;
        private const float OVERSHOOT_TOLERANCE = 0.5f;

        private Random random = new Random();

        public LaserAttack(Boss owner, Texture2D pixelTexture) : base(owner, pixelTexture)
        {
        }

        public override void Start(Player player)
        {
            currentState = LaserState.Charging;
            chargeTimer = 0f;
            fireTimer = 0f;
            coolDownTimer = 0f;
            coolDowmRealTimer = 0f;

            // Set the laser to always start at 0 degrees (pointing right)
            laserAngle = (float)Math.Atan2(player.position.Y - (boss.Position.Y + boss.Boss_height), player.position.X - (boss.Position.X + boss.Boss_width));

            // Set a fixed rotation direction (e.g., clockwise)
            rotationDirection = 1f;

            currentRotationSpeed = 0f;
            IsFinished = false;
        }

        public override void Update(GameTime gameTime, Player player, List<Rectangle> obstacles)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // ตรวจสอบความปลอดภัยของพารามิเตอร์ 'player'
            if (player == null)
            {
                IsFinished = true;
                return;
            }

            switch (currentState)
            {
                case LaserState.Charging:
                    chargeTimer += delta;
                    if (chargeTimer >= CHARGE_DURATION)
                    {
                        currentState = LaserState.Firing;
                        fireTimer = 0f;
                        coolDowmRealTimer = 0f;
                    }
                    break;

                case LaserState.Firing:
                    coolDowmRealTimer += delta;
                    fireTimer += delta;

                    // Gradually increase the rotation speed over the firing duration
                    currentRotationSpeed = MathHelper.Lerp(0, MAX_ROTATION_SPEED, fireTimer / FIRE_DURATION);

                    // Calculate the angle to the player (ใช้ 'player' parameter โดยตรง)
                    float desiredAngle = (float)Math.Atan2(player.position.Y - (boss.Position.Y + 1056 / 2), player.position.X - (boss.Position.X + boss.Boss_width));

                    // Use a slightly modified version of the previous logic to handle the overshoot
                    float angleDifference = MathHelper.WrapAngle(desiredAngle - laserAngle);

                    if (rotationDirection == 1 && angleDifference < -OVERSHOOT_TOLERANCE)
                    {
                        rotationDirection = -1;
                        fireTimer = 0f;
                    }
                    else if (rotationDirection == -1 && angleDifference > OVERSHOOT_TOLERANCE)
                    {
                        rotationDirection = 1;
                        fireTimer = 0f;
                    }

                    // Update the laser angle based on the determined rotation direction and the accelerating speed
                    laserAngle += currentRotationSpeed * rotationDirection * delta;

                    // *** การตรวจสอบการชนที่ถูกต้องสำหรับเลเซอร์ที่หมุนได้ ***

                    float laserLength = 2000f;
                    float thickness = 10f; // ความหนาของเลเซอร์ในสถานะ Firing
                    Vector2 laserStart = boss.Position + new Vector2(boss.Boss_width, boss.Boss_height);

                    // 1. กำหนดจุดปลายของเส้นเลเซอร์
                    Vector2 direction = new Vector2((float)Math.Cos(laserAngle), (float)Math.Sin(laserAngle));
                    Vector2 laserEnd = laserStart + direction * laserLength;

                    // 2. กำหนดจุดศูนย์กลางของผู้เล่นและรัศมี (แบบประมาณ)
                    Rectangle playerBox = player.HitboxgetDamage;
                    Vector2 playerCenter = playerBox.Center.ToVector2();
                    // รัศมีผู้เล่น (ใช้ขนาดที่ใหญ่ที่สุดของความกว้าง/สูง)
                    float playerRadius = Math.Max(playerBox.Width, playerBox.Height) / 2f;

                    // 3. ตรวจสอบว่าจุดใดบนเส้นเลเซอร์ (L1-L2) อยู่ใกล้ผู้เล่นที่สุด
                    Vector2 closestPoint = Vector2.Zero;
                    float dx = laserEnd.X - laserStart.X;
                    float dy = laserEnd.Y - laserStart.Y;
                    float lineLengthSquared = dx * dx + dy * dy;

                    if (lineLengthSquared == 0) // เลเซอร์เป็นแค่จุด
                    {
                        closestPoint = laserStart;
                    }
                    else
                    {
                        // คำนวณ t (ตัวประกอบการฉายภาพ)
                        float t = ((playerCenter.X - laserStart.X) * dx + (playerCenter.Y - laserStart.Y) * dy) / lineLengthSquared;

                        // จำกัดค่า t ให้อยู่ระหว่าง 0 ถึง 1 (เพื่อให้จุดใกล้สุดอยู่บนส่วนของเส้นตรง)
                        t = MathHelper.Clamp(t, 0, 1);

                        // คำนวณจุดที่ใกล้ที่สุดบนส่วนของเส้นตรง
                        closestPoint = new Vector2(laserStart.X + t * dx, laserStart.Y + t * dy);
                    }

                    // 4. ตรวจสอบระยะห่างการชน
                    float distanceSquared = Vector2.DistanceSquared(playerCenter, closestPoint);
                    float hitDistance = thickness / 2f + playerRadius; // รวมรัศมีการชนของเลเซอร์และผู้เล่น

                    if (distanceSquared <= hitDistance * hitDistance)
                    {
                        // Collision detected!
                        player.TakeDamage(2);
                    }
                    // **********************************

                    if (coolDowmRealTimer >= FIRE_DURATION)
                    {
                        currentState = LaserState.CoolingDown;
                        coolDownTimer = 0f;
                    }
                    break;

                case LaserState.CoolingDown:
                    coolDownTimer += delta;
                    if (coolDownTimer >= COOL_DOWN_DURATION)
                    {
                        IsFinished = true;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (currentState == LaserState.Charging)
            {
                // Make the laser blink during the charging phase
                if ((int)(chargeTimer * 5) % 2 == 0)
                {
                    DrawLaser(spriteBatch, 5f, Color.Red * 1f);
                }
            }
            else if (currentState == LaserState.Firing)
            {
                // Draw the full-strength laser while firing
                DrawLaser(spriteBatch, 10f, Color.Red);
            }
        }

        // Helper method to draw the laser
        private void DrawLaser(SpriteBatch spriteBatch, float thickness, Color color)
        {
            float laserLength = 2000f;
            Vector2 origin = new Vector2(0,0);
            Vector2 laserStart = boss.Position + new Vector2(boss.Boss_width, boss.Boss_height);

            // Hitbox calculation is now in Update()

            spriteBatch.Draw(
                pixelTexture,
                laserStart,
                null,
                color,
                laserAngle,
                origin,
                new Vector2(laserLength, thickness),
                SpriteEffects.None,
                0f
            );
        }
    }
}