using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Ghost_blade
{
    public class Enemy_Melee : Enemy
    {
        public int Damage { get; set; } = 2;
        public Rectangle AttackHitbox { get; private set; } = Rectangle.Empty;

        private float attackAnimationTimer = 0f;
        private const float AttackAnimationDuration = 0.2f;

        // New state variable
        private bool isAttacking = false;

        public Enemy_Melee(Texture2D texture, Vector2 startPosition, float speed, float detectionRadius)
          : base(texture, startPosition, speed, detectionRadius)
        {
        }

        public override void Update(Player player, List<Rectangle> obstacles, GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 desiredMovement = Vector2.Zero;

            // Priority 1: Check for knockback first
            if (knockbackTimer > 0)
            {
                Position += knockbackDirection * knockbackSpeed * deltaTime;
                knockbackTimer -= deltaTime;
                isAttacking = false; // Cancel attack if knocked back
            }
            // Priority 2: If the enemy is in the attacking state
            else if (isAttacking)
            {
                // The enemy stands still
                attackAnimationTimer -= deltaTime;

                // Check for damage only during the animation
                CheckMeleeAttack(player);

                // If the animation is over, return to normal state
                if (attackAnimationTimer <= 0)
                {
                    isAttacking = false;
                    AttackHitbox = Rectangle.Empty;
                }
            }
            // Priority 3: Normal behavior (chasing/attacking)
            else
            {
                float distanceToPlayer = Vector2.Distance(Position, player.position);

                // Check if the enemy can see the player
                if (CanSeePlayer(player, obstacles))
                {
                    // If the player is in range to start an attack
                    if (distanceToPlayer <= 70)
                    {
                        // Update attack cooldown
                        attackTimer -= deltaTime;
                        if (attackTimer <= 0)
                        {
                            // Trigger the attack state
                            isAttacking = true;
                            attackAnimationTimer = AttackAnimationDuration;
                            attackTimer = attackCooldown;
                        }
                        desiredMovement = Vector2.Zero; // Stop movement to prepare for attack
                    }
                    else
                    {
                        // Normal movement towards the player
                        desiredMovement = player.position - Position;
                        if (desiredMovement != Vector2.Zero)
                        {
                            desiredMovement.Normalize();
                        }
                    }
                }
                else // If enemy cannot see the player, remain idle
                {
                    desiredMovement = Vector2.Zero;
                }

                Vector2 newPosition = Position + desiredMovement * Speed;
                Position = newPosition;
            }
        }

        private void CheckMeleeAttack(Player player)
        {
            int attackWidth = 60;
            int attackHeight = 60;

            Vector2 directionToPlayer = player.position - this.Position;
            if (directionToPlayer != Vector2.Zero)
            {
                directionToPlayer.Normalize();
            }

            Vector2 attackPosition = this.Position + directionToPlayer * (this.Texture.Width / 2 + 10);

            // Create the hitbox for this frame
            Rectangle attackHitbox = new Rectangle(
        (int)(attackPosition.X - attackWidth / 2),
        (int)(attackPosition.Y - attackHeight / 2),
        attackWidth,
        attackHeight
      );

            // Assign the hitbox to the public property
            this.AttackHitbox = attackHitbox;

            if (attackHitbox.Intersects(player.HitboxgetDamage))
            {
                // Prevent damage from being applied multiple times per attack
                if (attackAnimationTimer > 0.1f)
                {
                    player.TakeDamage(this.Damage);
                    Console.WriteLine("Melee Enemy hit the player!");
                }
            }
        }
    }
}