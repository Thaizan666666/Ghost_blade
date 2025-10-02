using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using _321_Lab05_3;
namespace Ghost_blade
{
    public class Enemy_Melee : Enemy
    {
        public int Damage { get; set; } = 2;
        public Rectangle AttackHitbox { get; private set; } = Rectangle.Empty;

        private float attackAnimationTimer = 0f;
        private const float AttackAnimationDuration = 0.2f;
        private Vector2 lastPosition = Vector2.Zero;
        private float distanceToPlayer;
        // New state variable
        private bool isAttacking = false;

        private Vector2 TexturePosition;
        private bool flip = false;
        private AnimatedTexture Enemymelee_Idle;
        private AnimatedTexture Enemymelee_Walk;
        private AnimatedTexture Enemymelee_Attack;

        public Enemy_Melee(AnimatedTexture Enemymelee_Idle, AnimatedTexture Enemymelee_Walk, AnimatedTexture Enemymelee_Attack,
            Texture2D texture, Vector2 startPosition, float speed, float detectionRadius)
          : base(texture, startPosition, speed, detectionRadius)
        {
            this.Enemymelee_Idle = Enemymelee_Idle;
            this.Enemymelee_Walk = Enemymelee_Walk;
            this.Enemymelee_Attack = Enemymelee_Attack;
        }

        public override void Update(Player player, List<Rectangle> obstacles, GameTime gameTime)
        {
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Enemymelee_Idle.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            Enemymelee_Walk.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            Enemymelee_Attack.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            Vector2 desiredMovement = Vector2.Zero;

            if (knockbackTimer > 0)
            {
                Position += knockbackDirection * knockbackSpeed * deltaTime * 5f;
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
                distanceToPlayer = Vector2.Distance(Position, player.position);

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
                if (Health <= 0)
                {
                    this.Die();
                    IsActive = false;
                }

                if (desiredMovement.X < 0) { flip = true; }
                else if (desiredMovement.X > 0) { flip = false; }
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            TexturePosition = new Vector2 (Position.X - 32,Position.Y - 32);

            if (IsActive)
            {
                if (knockbackTimer > 0)
                {
                    Enemymelee_Idle.DrawFrame(spriteBatch, TexturePosition, flip);
                }
                else if (distanceToPlayer <= 70)
                {
                    Enemymelee_Attack.DrawFrame(spriteBatch, TexturePosition, flip);
                }
                else
                {
                    if (Vector2.Distance(TexturePosition, lastPosition) > 0.1f)
                        Enemymelee_Walk.DrawFrame(spriteBatch, TexturePosition, flip);
                    else
                        Enemymelee_Idle.DrawFrame(spriteBatch, TexturePosition  , flip);
                }
                lastPosition = TexturePosition;
            }
        }
    }
}