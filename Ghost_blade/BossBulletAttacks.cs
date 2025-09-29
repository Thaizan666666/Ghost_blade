using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Ghost_blade
{
    public class BossBulletAttacks : BossAttack
    {
        private List<EnemyBullet> bullets;
        private bool isFiring = false;

        private const int BURST_COUNT = 50; // Number of bullets to fire in one burst
        private const float BURST_DELAY = 0.15f; // Delay between each bullet shot in the burst
        private const float ATTACK_DURATION = 1.0f; // Total duration of the attack state
        private Vector2 position;

        private int bulletsShotInBurst;
        private float burstTimer;
        private float attackTimer;

        public BossBulletAttacks(Boss owner, Texture2D bulletTexture) : base(owner, bulletTexture)
        {
            bullets = new List<EnemyBullet>();
        }

        public override void Start(Player player)
        {
            // Reset all state variables for the new attack
            isFiring = true;
            bulletsShotInBurst = 0;
            burstTimer = BURST_DELAY;
            attackTimer = ATTACK_DURATION;
            IsFinished = false;
            position = boss.Position+ new Vector2 (0,+1055);
        }

        public override void Update(GameTime gameTime, Player player,List<Rectangle> Obstacles)
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
                bullets.Add(new EnemyBullet(pixelTexture, position, directionToPlayer, 8f, 0f, 3f));

                bulletsShotInBurst++;
                burstTimer = BURST_DELAY; // Reset the timer for the next shot
            }

            // Update all active bullets and check for collision
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                // Pass the obstacles list to the bullet's update method
                bullets[i].Update(gameTime, Obstacles, player); 

                if (!bullets[i].IsActive)
                {
                    bullets.RemoveAt(i);
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
            // Draw all active bullets
            foreach (var bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
    }
}