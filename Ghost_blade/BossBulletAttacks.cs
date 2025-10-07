using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Ghost_blade
{
    public class BossBulletAttacks : BossAttack
    {
        private List<EnemyBullet> bullets;
        private List<Bullet> _parriedBullets;
        private bool isFiring = false;
        private Texture2D parry;

        private const int BURST_COUNT = 50; // Number of bullets to fire in one burst
        private const float BURST_DELAY = 0.15f; // Delay between each bullet shot in the burst
        private const float ATTACK_DURATION = 0.6f; // Total duration of the attack state
        private Vector2 position;

        private int bulletsShotInBurst;
        private float burstTimer;
        private float attackTimer;

        public BossBulletAttacks(Boss owner, Texture2D bulletTexture,Texture2D parry) : base(owner, bulletTexture)
        {
            bullets = new List<EnemyBullet>();
            _parriedBullets = new List<Bullet>();
            this.parry = parry;
        }

        public override void Start(Player player)
        {
            // Reset all state variables for the new attack
            isFiring = true;
            bulletsShotInBurst = 0;
            burstTimer = BURST_DELAY;
            attackTimer = ATTACK_DURATION;
            IsFinished = false;
            position = boss.Position + new Vector2(144, boss.Boss_height-48);
        }

        public override void Update(GameTime gameTime, Player player, List<Rectangle> Obstacles)
        {
            if (!isFiring)
                return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            attackTimer -= deltaTime;
            burstTimer -= deltaTime;

            // Fire a bullet if the timer is ready and we haven't fired all bullets yet
            if (burstTimer <= 0 && bulletsShotInBurst < BURST_COUNT)
            {
                // Calculate bullet direction towards the player
                Vector2 directionToPlayer = player.position - position;
                directionToPlayer.Normalize();

                // Create and add the new bullet
                bullets.Add(new EnemyBullet(pixelTexture, position, directionToPlayer, 8f, 0f, 3f,parry));

                bulletsShotInBurst++;
                burstTimer = BURST_DELAY; // Reset the timer for the next shot
            }

            // Update all active bullets and check for collision
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                EnemyBullet currentEnemyBullet = bullets[i];

                // **เรียกใช้ Update และรับค่ากระสุน Parry คืนมา**
                // (ต้องแน่ใจว่าได้แก้ไข EnemyBullet.Update เป็น public Bullet Update(...) แล้ว)
                Bullet newParriedBullet = currentEnemyBullet.Update(gameTime, Obstacles, player,boss.Position + new Vector2(boss.Position.X/2,boss.Position.Y/2));

                // **ถ้ามีการ Parry เกิดขึ้น**
                if (newParriedBullet != null)
                {
                    _parriedBullets.Add(newParriedBullet); // เพิ่มกระสุนใหม่เข้า List Parry
                }

                if (!currentEnemyBullet.IsActive)
                {
                    bullets.RemoveAt(i);
                }
                for (int j = _parriedBullets.Count - 1; j >= 0; j--)
                {
                    Bullet parriedBullet = _parriedBullets[j];

                    // อัปเดตการเคลื่อนที่และชนกับกำแพง (เรียก Update ของคลาส Bullet Base)
                    parriedBullet.Update(gameTime, Obstacles);

                    // *[TODO: เพิ่มโค้ดตรวจสอบการชนกับ Boss ที่นี่]*
                    if (parriedBullet.boundingBox.Intersects(boss.HitboxgetDamage))
                    {
                        boss.TakeDamage(40);
                        parriedBullet.IsActive = false;
                    }

                    if (!parriedBullet.IsActive)
                    {
                        _parriedBullets.RemoveAt(j);
                    }
                }
        }

            // End the attack when all bullets are fired or the attack duration is over
            if (bulletsShotInBurst >= BURST_COUNT && bullets.Count == 0)
            {
                isFiring = false;
                IsFinished = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw all active enemy bullets
            foreach (var bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }

            // **[เพิ่ม] วาดกระสุนที่ถูก Parry**
            foreach (var bullet in _parriedBullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
    }
}