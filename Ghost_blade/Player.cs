using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using _321_Lab05_3;
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
        
        private float fireDelay = 0.2f;
        private float timer = 0f;
        private Texture2D bulletTexture;
        public int currentAmmo;
        public int maxAmmo { get; set; } = 30;
        public bool isSwordEquipped = true;

        private KeyboardState previousKState;
        private MouseState previousMState;
        private Vector2 lastMovementDirection = new Vector2(1, 0);
        public MeleeWeapon meleeWeapon { get; private set; }
        public int Health { get; set; } = 5;

        private bool isDashing = false;
        private float dashTimer = 0f;
        private const float DashDuration = 0.4f;
        private const float DashSpeedMultiplier = 3.0f;
        private Vector2 dashDirection;
        private const float DashCooldown = 0.75f;
        private float dashCooldownTimer = 0f;

        public bool isReloading = false;
        private float reloadTimer = 0f;
        private const float ReloadTime = 1f;

        public bool _isInvincible = false;
        private float _invincibilityTimer = 0f;
        private const float InvincibilityDuration = 0.4f;

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
        private const float ParryCooldown = 3f; // Set a cooldown duration, e.g., 1.0 second
        private float parryCooldownTimer = 0f;

        public AnimatedTexture change_Weapon;
        private bool isWeaponSwitching = false;
        public int currentWeaponFrame = 0;
        private bool isWeaponSwitchingBackwards = false;
        private int weaponSwitchStartFrame = 0;
        private int weaponSwitchEndFrame = 0;
        private float weaponFrameTimer = 0f;
        private float weaponFrameRate = 0.1f;
        private int framefinish;
        public bool isDamageFlickering = false;

        private bool flip = false;
        private bool attackf = false;
        public AnimatedTexture IdleBladeTexture;
        public AnimatedTexture IdleGunTexture;
        public AnimatedTexture RunningBladeTexture;
        public AnimatedTexture RunningGunTexture;
        public AnimatedTexture AttackingTexture;
        public AnimatedTexture AttackingTexture2;
        public AnimatedTexture AttackingTextureUp;
        public AnimatedTexture AttackingTextureDown;
        public AnimatedTexture GunDashingTexture;
        public AnimatedTexture BladeDashingTexture;
        public Texture2D Hand;
        public float HandRotation;
        public Vector2 HandOrigin;

        public Rectangle drect
        {
            get
            {
                {
                    return new Rectangle(
                        (int)(position.X - texture.Width / 2 + 48),
                        (int)(position.Y - texture.Height + 48),
                        48,
                        (texture.Height - 24)*2
                    );
                }
            }
        }
        public Rectangle HitboxgetDamage
        {
            get
            {
                return new Rectangle(
                            (int)(position.X - texture.Width / 2 + 48),
                            (int)(position.Y - texture.Height),
                            48,
                            texture.Height * 2
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
            this.timer = 0f;
            this.currentAmmo = 10;
            this.previousKState = Keyboard.GetState();
            this.previousMState = Mouse.GetState();
            this.IsAlive = true;
            this.meleeWeapon = new MeleeWeapon(meleeWeaponTexture);

            IdleBladeTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            IdleGunTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            RunningGunTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            RunningBladeTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            GunDashingTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            BladeDashingTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            AttackingTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            AttackingTexture2 = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            AttackingTextureUp = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            AttackingTextureDown = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
        }

        public void Update(GameTime gameTime, Vector2 cameraPosition)
        {
            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            IdleBladeTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            IdleGunTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            RunningBladeTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            RunningGunTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            GunDashingTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            BladeDashingTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            AttackingTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            AttackingTexture2.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            AttackingTextureUp.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            AttackingTextureDown.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
            //เติมเลือด
            if(kState.IsKeyDown(Keys.H) && !previousKState.IsKeyDown(Keys.H))
            {
                Health += 2;
            }
            else if(Health > 5)
            {
                Health = 5;
            }

            HandleDash(kState, deltaTime);
            HandleWeaponSwitching(kState);
            HandleReload(kState,mState, deltaTime);
            HandleAttacks(kState,mState, cameraPosition); // Pass cameraPosition here

            MouseState mouse = Mouse.GetState();
            if (mouse.X < 1920/2) { flip = true;}
            else if (mouse.X >= 1920/2) {  flip = false;}

            // NEW: Updated state management logic
            if (isDashing)
            {
                currentState = PlayerState.Dashing;
            }
            else if (currentState == PlayerState.Attacking)
            {
                // Update melee weapon while attacking
                meleeWeapon.Update(gameTime, position, cameraPosition);
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
                    meleeWeapon.Update(gameTime, position, cameraPosition);
                }
                else
                {
                    currentState = PlayerState.Idle;
                    // Update weapon rotation while idle
                    meleeWeapon.Update(gameTime, position, cameraPosition);
                }
            }
            if (parryCooldownTimer > 0)
            {
                parryCooldownTimer -= deltaTime;
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

                    isDamageFlickering = false;
                }
            }

            timer += deltaTime;
            previousKState = kState;
            previousMState = mState;

            if (isWeaponSwitching)
            {
                weaponFrameTimer += deltaTime;

                if (weaponFrameTimer >= weaponFrameRate)
                {
                    weaponFrameTimer = 0f;

                    if (!isWeaponSwitchingBackwards)
                    {
                        if (currentWeaponFrame < weaponSwitchEndFrame)
                            currentWeaponFrame++;
                        else
                            isWeaponSwitching = false;
                    }
                    else
                    {
                        if (currentWeaponFrame > weaponSwitchEndFrame)
                            currentWeaponFrame--;
                        else
                            isWeaponSwitching = false;
                    }
                }
            }
        }

        private void HandleAttacks(KeyboardState kState, MouseState mState, Vector2 cameraPosition) // ต้องเพิ่ม cameraPosition เป็น parameter
        {
            bool _iscanattack = isDashing;
            if (!_iscanattack || meleeWeapon._ultTimer > 0)
            {
                if (mState.LeftButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released && meleeWeapon._parryTimer <= 0)
                {
                    if (isSwordEquipped)
                    {
                        meleeWeapon.PerformAttack(position, cameraPosition);
                        currentState = PlayerState.Attacking;
                        _isSlash = true;
                    }
                }
                if (mState.RightButton == ButtonState.Pressed && previousMState.RightButton == ButtonState.Released)
                {
                    if (isSwordEquipped && currentState != PlayerState.Attacking && !isDashing && parryCooldownTimer <= 0)
                    {
                        meleeWeapon.PerformParry(position, cameraPosition);
                        parryCooldownTimer = ParryCooldown;
                    }
                }
            }
        }

        // ** (ส่วนอื่นๆ ของคลาสที่ไม่ได้เปลี่ยนแปลง) **
        private void HandleDash(KeyboardState kState, float deltaTime)
        {
            bool isActionActive = currentState == PlayerState.Attacking || meleeWeapon._parryTimer > 0 || meleeWeapon._ultTimer > 0 || meleeWeapon._ultTimer > 0;
            if (dashCooldownTimer > 0)
            {
                dashCooldownTimer -= deltaTime;
            }

            if (kState.IsKeyDown(Keys.Space) && !previousKState.IsKeyDown(Keys.Space) && !isDashing && dashCooldownTimer <= 0 && !isActionActive)
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
            bool isActionActive = isDashing || currentState == PlayerState.Attacking || meleeWeapon._parryTimer > 0 || meleeWeapon._ultTimer > 0;
            if (!isActionActive)
            {
                Vector2 newVelocity = Vector2.Zero;

                if (kState.IsKeyDown(Keys.W)) { newVelocity.Y -= 1; }
                else if (kState.IsKeyDown(Keys.S)) { newVelocity.Y += 1;}
                if (kState.IsKeyDown(Keys.D)) { newVelocity.X += 1; }
                else if (kState.IsKeyDown(Keys.A)) { newVelocity.X -= 1;}

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
                isWeaponSwitching = true;

                if (isSwordEquipped)
                {
                    isWeaponSwitchingBackwards = true;
                    weaponSwitchStartFrame = 3;
                    weaponSwitchEndFrame = 0;
                    currentWeaponFrame = weaponSwitchStartFrame;
                    Debug.WriteLine("Weapon: Sword");
                }
                else
                {
                    isWeaponSwitchingBackwards = false;
                    weaponSwitchStartFrame = 0;
                    weaponSwitchEndFrame = 3;
                    currentWeaponFrame = weaponSwitchStartFrame;
                    Debug.WriteLine("Weapon: Gun");
                }
            }
        }

        private void HandleReload(KeyboardState kState,MouseState mState, float deltaTime)
        {
            const byte MAG_SIZE = 10;
            if (!isSwordEquipped)
            {
                if ((!isReloading && kState.IsKeyDown(Keys.R) && currentAmmo < MAG_SIZE && maxAmmo > 0)||
                (currentAmmo == 0 && mState.LeftButton == ButtonState.Pressed && !isReloading && maxAmmo > 0))
                {
                    isReloading = true;
                    reloadTimer = ReloadTime;
                    Debug.WriteLine("Reloading...");
                }

                if (isReloading)
                {
                    reloadTimer -= deltaTime;
                    if (reloadTimer <= 0f)
                    {
                        int ammoNeeded = MAG_SIZE - currentAmmo;
                        int ammoToTake = Math.Min(ammoNeeded, maxAmmo);
                        maxAmmo -= ammoToTake;
                        currentAmmo += ammoToTake;
                        isReloading = false;
                        Debug.WriteLine("Reload complete. Ammo = 10");
                    }
                }
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
            bool _iscanShoot = isDashing;
            if (!_iscanShoot)
            {
                if (currentAmmo <= 0 || isSwordEquipped)
                {
                    Debug.WriteLine("Ammo = 0 or Sword is Equipped");
                    return null;
                }
                if (timer >= fireDelay)
                {
                    timer = 0f;
                    Vector2 direction = mousePosition - position + new Vector2(24, 24);
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
                    return new Bullet(bulletTexture, bulletStartPosition - new Vector2(24, 24), direction, 20f, bulletRotation, 2f);
                }
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

            isDamageFlickering = true;
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

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            if (IsAlive)
            {
                Color drawColor = Color.White;
                if (isDamageFlickering)
                {
                    // ใช้ค่า flickerInterval ที่กำหนด
                    const float flickerInterval = 0.05f;

                    // ใช้ _invincibilityTimer เพื่อควบคุมจังหวะกะพริบ สลับระหว่าง White (มองเห็น) และ Transparent (หายไป)
                    if ((int)(_invincibilityTimer / flickerInterval) % 2 == 0)
                    {
                        drawColor = Color.Transparent; // กะพริบหายไป
                    }
                    // ถ้าเป็นเลขคี่ จะใช้ drawColor = Color.White (ค่าเริ่มต้น)
                }
                Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
                Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);
                MouseState mouseState = Mouse.GetState();
                Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
                Vector2 screenCenter = new Vector2(1920 / 2f, 1080 / 2f);
                float HandRotation = (float)Math.Atan2(mousePos.Y - screenCenter.Y, mousePos.X - screenCenter.X);
                if (!isSwordEquipped && currentState != PlayerState.Dashing)
                {
                    if (mousePos.X < screenCenter.X)
                    {
                        spriteBatch.Draw(Hand, new Vector2(position.X - 32, position.Y - 14), null, drawColor, HandRotation, HandOrigin, 2f, SpriteEffects.FlipVertically, 0f);
                    }
                    else 
                    { 
                        spriteBatch.Draw(Hand, new Vector2(position.X - 15, position.Y - 14), null, drawColor, HandRotation, HandOrigin, 2f, SpriteEffects.None, 0f);
                    }

                }
                if (currentState == PlayerState.Idle)
                {
                    if (isSwordEquipped) { IdleBladeTexture.DrawFrame(spriteBatch, position - new Vector2(48, 48),flip); }
                    else if (!isSwordEquipped && flip == false) { IdleGunTexture.DrawFrame(spriteBatch, position - new Vector2(48, 48), flip); }
                    else if (!isSwordEquipped && flip == true) { IdleGunTexture.DrawFrame(spriteBatch, position - new Vector2(96, 48), flip); }
                }
                else if (currentState == PlayerState.Running)
                {
                    if (isSwordEquipped) { RunningBladeTexture.DrawFrame(spriteBatch, position - new Vector2(48, 48), flip); }
                    else if (!isSwordEquipped) { RunningGunTexture.DrawFrame(spriteBatch, position - new Vector2(48, 48), flip); }
                }
                else if (currentState == PlayerState.Dashing)
                {
                    if (isSwordEquipped && flip == false) { BladeDashingTexture.DrawFrame(spriteBatch, position - new Vector2(48, 48), flip); }
                    else if (isSwordEquipped && flip == true) { BladeDashingTexture.DrawFrame(spriteBatch, position - new Vector2(72, 48), flip); }
                    else if (!isSwordEquipped && flip == false) { GunDashingTexture.DrawFrame(spriteBatch, position - new Vector2(48, 48), flip); }
                    else if (!isSwordEquipped && flip == true) { GunDashingTexture.DrawFrame(spriteBatch, position - new Vector2(72, 48), flip); }
                }
                else if (currentState == PlayerState.Attacking) 
                {
                    int AttackDiraction = meleeWeapon.Attack;
                    if (isSwordEquipped)
                    {
                        if (AttackDiraction == 1) // ล่าง
                        {
                            if (flip == false)
                            { AttackingTextureDown.DrawFrame(spriteBatch, position - new Vector2(64, 64), flip); }
                            else
                            { AttackingTextureDown.DrawFrame(spriteBatch, position - new Vector2(64, 64), flip); }
                        }
                        else if (AttackDiraction == 2) // ซ้าย
                        {
                            flip = true;
                            if (attackf == false)
                            {
                                { AttackingTexture.DrawFrame(spriteBatch, position - new Vector2(96, 48), flip); }
                            }
                            else if (attackf == true)
                            {
                                { AttackingTexture2.DrawFrame(spriteBatch, position - new Vector2(48, 48), flip); }
                            }
                        }
                        else if (AttackDiraction == 3) // บน
                        {
                            if (flip == false)
                            { AttackingTextureUp.DrawFrame(spriteBatch, position - new Vector2(64, 64), flip); }
                            else
                            { AttackingTextureUp.DrawFrame(spriteBatch, position - new Vector2(64, 64), flip); }
                        }
                        else if (AttackDiraction == 4) // ขวา
                        {
                            flip = false;
                            if (attackf == false)
                            {
                                { AttackingTexture.DrawFrame(spriteBatch, position - new Vector2(48, 48), flip); }
                            }
                            else if (attackf == true)
                            {
                                { AttackingTexture2.DrawFrame(spriteBatch, position - new Vector2(48, 48) , flip); }
                            }
                        }
                    }
                }
                if (!isSwordEquipped && currentState != PlayerState.Dashing)
                {
                    if (mousePos.X < screenCenter.X)
                    {
                        spriteBatch.Draw(Hand, new Vector2(position.X - 18, position.Y - 14), null, drawColor, HandRotation, HandOrigin, 2f, SpriteEffects.FlipVertically, 0f);
                    }
                    else
                    {
                        spriteBatch.Draw(Hand, new Vector2(position.X - 32, position.Y - 14), null, drawColor, HandRotation, HandOrigin, 2f, SpriteEffects.None, 0f);
                    }

                }
            }
        }
        public void Reset()
        {
            IsAlive = true;
            Health = 5;
            currentAmmo = 10;
            isSwordEquipped = true;
            currentWeaponFrame = 0;
        }
    }
}