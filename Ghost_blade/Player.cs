using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ghost_blade
{
    public enum PlayerState
    {
        Idle,
        Running,
        Attacking,
        Dashing
    }
    public class Player
    {
        public Texture2D texture { get; set; }
        public Vector2 position { get; set; }
        public Vector2 velocity { get; private set; }

        private float speed;
        private float rotation;
        private SpriteEffects currentSpriteEffect;
        private float fireDelay = 0.2f;
        private float timer = 0f;
        private Texture2D bulletTexture;
        public byte currentAmmo;
        private bool isSwordEquipped = true;
        private KeyboardState previousKState;
        private MouseState previousMState;
        private Vector2 lastMovementDirection = new Vector2(1, 0);
        public MeleeWeapon meleeWeapon { get; private set; }
        public int Health { get; set; } = 10;

        private bool isDashing = false;
        private float dashTimer = 0f;
        private const float DashDuration = 0.2f;
        private const float DashSpeedMultiplier = 5.0f;
        private Vector2 dashDirection;
        private const float DashCooldown = 2f;
        private float dashCooldownTimer = 0f;

        public bool _isInvincible = false;
        private float _invincibilityTimer = 0f;
        private const float InvincibilityDuration = 0.5f;

        public bool IsAlive { get; private set; }
        public bool _isSlash { get; set; } = true;
        private int currentFrame;
        private float frameTimer;
        private float frameRate = 0.1f;
        private int frameWidth = 48;
        private int frameHeight = 48;
        public PlayerState currentState { get; private set; }
        private readonly int[] idleFrames = { 0, 1, 2, 3 };
        private readonly float idleFrameRate = 0.15f;
        private readonly int[] runningFrames = { 4, 5, 6, 7 };
        private readonly float runningFrameRate = 0.1f;


        // NEW: Time management for attacking state
        private float attackTimer = 0f;
        private const float AttackDuration = 0.2f; // ระยะเวลาการโจมตี (ตามที่คุณใช้ใน MeleeWeapon)

        public Rectangle drect
        {
            get
            {
                {
                    return new Rectangle(
                        (int)(position.X - texture.Width / 4 + 24),
                        (int)(position.Y - texture.Height / 2 + 24),
                        24,
                        texture.Height - 24
                    );
                }
            }
        }
        public Rectangle HitboxgetDamage
        {
            get
            {
                return new Rectangle(
                            (int)(position.X - texture.Width / 4 + 24),
                            (int)(position.Y - 24),
                            24,
                            texture.Height
                            );
            }
        }

        public Player(Texture2D playerTexture, Texture2D bulletTexture, Texture2D meleeWeaponTexture, Vector2 initialPosition)
        {
            this.texture = playerTexture;
            this.bulletTexture = bulletTexture;
            this.position = initialPosition;
            this.speed = 4f;
            this.velocity = Vector2.Zero;
            this.currentSpriteEffect = SpriteEffects.None;
            this.timer = 0f;
            this.currentAmmo = 10;
            this.previousKState = Keyboard.GetState();
            this.previousMState = Mouse.GetState();
            this.IsAlive = true;
            this.meleeWeapon = new MeleeWeapon(meleeWeaponTexture);
        }

        public void Update(GameTime gameTime, Vector2 cameraPosition)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleDash(kState, deltaTime);
            HandleWeaponSwitching(kState);
            HandleReload(kState);
            HandleAttacks(mState, gameTime, cameraPosition); // Pass cameraPosition here

            // NEW: Updated state management logic
            if (isDashing)
            {
                currentState = PlayerState.Dashing;
            }
            else if (currentState == PlayerState.Attacking)
            {
                // Update melee weapon while attacking
                meleeWeapon.Update(gameTime, position, cameraPosition, true);
                // Handle attack duration
                attackTimer += deltaTime;
                if (attackTimer >= AttackDuration)
                {
                    currentState = PlayerState.Idle; // End attacking state
                    attackTimer = 0f;
                    // Reset _isSlash or any other state-specific flags
                }
            }
            else // Not Dashing or Attacking
            {
                HandleMovement(kState);
                if (velocity != Vector2.Zero)
                {
                    currentState = PlayerState.Running;
                    // Update weapon rotation while running
                    meleeWeapon.Update(gameTime, position, cameraPosition, false);
                }
                else
                {
                    currentState = PlayerState.Idle;
                    // Update weapon rotation while idle
                    meleeWeapon.Update(gameTime, position, cameraPosition, false);
                }
            }

            switch (currentState)
            {
                case PlayerState.Idle:
                    UpdateAnimation(gameTime, idleFrames, idleFrameRate);
                    break;
                case PlayerState.Running:
                    UpdateAnimation(gameTime, runningFrames, runningFrameRate);
                    break;
                // Add more cases for Attacking and Dashing animation if you have them
                case PlayerState.Attacking:
                    // Use a specific animation for attacking
                    break;
            }

            if (_isInvincible)
            {
                _invincibilityTimer -= deltaTime;
                if (_invincibilityTimer <= 0)
                {
                    _isInvincible = false;
                }
            }

            timer += deltaTime;
            previousKState = kState;
            previousMState = mState;
        }

        private void HandleAttacks(MouseState mState, GameTime gameTime, Vector2 cameraPosition)
        {
            if (mState.LeftButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released)
            {
                if (isSwordEquipped)
                {
                    // If not currently attacking, start the attack
                    if (currentState != PlayerState.Attacking)
                    {
                        _isSlash = true;
                        currentState = PlayerState.Attacking;
                        attackTimer = 0f; // Reset attack timer
                        Debug.WriteLine("Player is Attacking!");
                    }
                }
            }
        }

        // ** (ส่วนอื่นๆ ของคลาสที่ไม่ได้เปลี่ยนแปลง) **
        private void HandleDash(KeyboardState kState, float deltaTime)
        {
            if (dashCooldownTimer > 0)
            {
                dashCooldownTimer -= deltaTime;
            }

            if (kState.IsKeyDown(Keys.Space) && !previousKState.IsKeyDown(Keys.Space) && !isDashing && dashCooldownTimer <= 0)
            {
                isDashing = true;
                dashTimer = DashDuration;
                dashCooldownTimer = DashCooldown;

                _isInvincible = true;
                _invincibilityTimer = DashDuration;

                if (velocity != Vector2.Zero)
                {
                    dashDirection = velocity;
                }
                else
                {
                    dashDirection = lastMovementDirection;
                }

                if (dashDirection != Vector2.Zero)
                {
                    dashDirection.Normalize();
                }
                Debug.WriteLine("Dash Activated!");
            }

            if (isDashing)
            {
                Vector2 newPosition = position + dashDirection * speed * DashSpeedMultiplier;
                position = newPosition;

                dashTimer -= deltaTime;
                if (dashTimer <= 0)
                {
                    isDashing = false;
                    Debug.WriteLine("Dash Ended.");
                }
            }
        }

        private void HandleMovement(KeyboardState kState)
        {
            if (!isDashing && currentState != PlayerState.Attacking)
            {
                Vector2 newVelocity = Vector2.Zero;

                if (kState.IsKeyDown(Keys.W)) { newVelocity.Y -= 1; }
                else if (kState.IsKeyDown(Keys.S)) { newVelocity.Y += 1; }
                if (kState.IsKeyDown(Keys.D)) { newVelocity.X += 1; currentSpriteEffect = SpriteEffects.None; }
                else if (kState.IsKeyDown(Keys.A)) { newVelocity.X -= 1; currentSpriteEffect = SpriteEffects.FlipHorizontally; }

                velocity = newVelocity;

                if (velocity != Vector2.Zero)
                {
                    velocity.Normalize();
                    lastMovementDirection = velocity;
                }

                Vector2 newPosition = position + velocity * speed;
                position = newPosition;
            }
            else
            {
                velocity = Vector2.Zero; // Stop movement when attacking or dashing
            }
        }

        private void HandleWeaponSwitching(KeyboardState kState)
        {
            if (kState.IsKeyDown(Keys.E) && !previousKState.IsKeyDown(Keys.E))
            {
                isSwordEquipped = !isSwordEquipped;
                if (isSwordEquipped)
                {
                    Debug.WriteLine("Weapon: Sword");
                }
                else
                {
                    Debug.WriteLine("Weapon: Gun");
                }
            }
        }

        private void HandleReload(KeyboardState kState)
        {
            if (kState.IsKeyDown(Keys.R))
            {
                this.currentAmmo = 10;
                Debug.WriteLine("Ammo = 10");
            }
        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public void ClampPosition(Rectangle bounds, List<Rectangle> obstacles)
        {

            // 1. Apply normal movement if not dashing

            if (!isDashing)

            {

                position += velocity; // Apply normal movement here

            }



            // 2. Clamp to world bounds first

            position = Vector2.Clamp(position,

                  new Vector2(bounds.Left + texture.Width / 2, bounds.Top + texture.Height / 2),

                  new Vector2(bounds.Right - texture.Width / 2, bounds.Bottom - texture.Height / 2));



            // 3. Handle obstacle collisions

            foreach (var obs in obstacles)

            {

                // If the player's bounding box intersects an obstacle

                while (drect.Intersects(obs)) // Use a while loop to ensure player is fully out

                {

                    if (isDashing)

                    {

                        isDashing = false; // Stop dashing immediately upon collision

                        Debug.WriteLine("Dash interrupted by obstacle.");

                    }



                    // Determine the direction to push the player out

                    Vector2 separationVector = Vector2.Zero;

                    Rectangle intersection = Rectangle.Intersect(drect, obs);



                    // Find the smallest axis of overlap to push along

                    if (intersection.Width < intersection.Height)

                    {

                        // Push horizontally

                        if (drect.Center.X < obs.Center.X) // Player is to the left of obstacle

                        {

                            separationVector.X = -intersection.Width;

                        }

                        else // Player is to the right of obstacle

                        {

                            separationVector.X = intersection.Width;

                        }

                    }

                    else

                    {

                        // Push vertically

                        if (drect.Center.Y < obs.Center.Y) // Player is above obstacle

                        {

                            separationVector.Y = -intersection.Height;

                        }

                        else // Player is below obstacle

                        {

                            separationVector.Y = intersection.Height;

                        }

                    }

                    position += separationVector;

                    Debug.WriteLine($"Collision! Pushing player by {separationVector}");

                }

            }

        }

        public Bullet Shoot(Vector2 mousePosition)
        {
            if (currentAmmo <= 0 || isSwordEquipped)
            {
                Debug.WriteLine("Ammo = 0 or Sword is Equipped");
                return null;
            }
            if (timer >= fireDelay)
            {
                timer = 0f;
                Vector2 direction = mousePosition - position;
                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                }
                else
                {
                    direction = Vector2.UnitX;
                }
                float bulletRotation = MathF.Atan2(direction.Y, direction.X);
                Vector2 bulletStartPosition = position;
                currentAmmo--;
                return new Bullet(bulletTexture, bulletStartPosition, direction, 10f, bulletRotation, 2f);
            }
            return null;
        }

        public void Die()
        {
            IsAlive = false;
        }

        public void TakeDamage(int damage)
        {
            if (_isInvincible)
            {
                return; // Do nothing if invincible
            }

            Health -= damage;
            Debug.WriteLine($"Player took {damage} damage. Health is now {Health}");

            _isInvincible = true;
            _invincibilityTimer = InvincibilityDuration;

            if (Health <= 0)
            {
                Die();
            }
        }

        private void UpdateAnimation(GameTime gameTime, int[] frames, float rate)
        {
            frameRate = rate;
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (frameTimer >= frameRate)
            {
                frameTimer = 0f;
                currentFrame++;
                if (currentFrame >= frames.Length)
                {
                    currentFrame = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
                Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
                if (currentSpriteEffect == SpriteEffects.None)
                {
                    spriteBatch.Draw(texture, position, sourceRect, Color.White, rotation, origin, 1f, currentSpriteEffect, 0f);
                }
                else
                {
                    spriteBatch.Draw(texture, position - new Vector2(24,0), sourceRect, Color.White, rotation, origin, 1f, currentSpriteEffect, 0f);
                }
            }
        }
        public void Reset()
        {
            IsAlive = true;
            Health = 10;

        }
    }
}