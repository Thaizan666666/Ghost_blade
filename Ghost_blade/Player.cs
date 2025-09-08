using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ghost_blade
{
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
        private Vector2 lastMovementDirection = new Vector2(1, 0); // Stores the last direction the player moved

        private bool isDashing = false;
        private float dashTimer = 0f;
        private const float DashDuration = 0.2f;
        private const float DashSpeedMultiplier = 5.0f;
        private Vector2 dashDirection;

        private const float DashCooldown = 2f;
        private float dashCooldownTimer = 0f;

        public Rectangle drect
        {
            get
            {
                return new Rectangle(
                    (int)(position.X - texture.Width / 2),
                    (int)(position.Y - texture.Height / 2),
                    texture.Width,
                    texture.Height
                );
            }
        }

        public Player(Texture2D playerTexture, Texture2D bulletTexture, Vector2 initialPosition)
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
        }

        public void Update(GameTime gameTime, Vector2 cameraPosition)
        {
            KeyboardState kState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleDash(kState, deltaTime);
            HandleMovement(kState);
            HandleRotation(cameraPosition);
            HandleWeaponSwitching(kState);
            HandleReload(kState);

            timer += deltaTime;
            previousKState = kState;
        }

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

                // Use the current velocity for the dash direction if the player is moving
                if (velocity != Vector2.Zero)
                {
                    dashDirection = velocity;
                }
                else
                {
                    // Use the last known movement direction for the dash if the player is stationary
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
                // Corrected position update for dashing
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
            if (!isDashing)
            {
                Vector2 newVelocity = Vector2.Zero;

                if (kState.IsKeyDown(Keys.W)) { newVelocity.Y -= 1; }
                if (kState.IsKeyDown(Keys.S)) { newVelocity.Y += 1; }
                if (kState.IsKeyDown(Keys.D)) { newVelocity.X += 1; }
                if (kState.IsKeyDown(Keys.A)) { newVelocity.X -= 1; }

                // Update the velocity property
                velocity = newVelocity;

                if (velocity != Vector2.Zero)
                {
                    velocity.Normalize();
                    // Store the current movement direction for stationary dashes
                    lastMovementDirection = velocity;
                }

                // Update the position property
                Vector2 newPosition = position + velocity * speed;
                position = newPosition;
            }
        }

        private void HandleRotation(Vector2 cameraPosition)
        {
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosWorld = new Vector2(mouseState.X, mouseState.Y) - cameraPosition;
            Vector2 dirToMouse = mousePosWorld - position;

            float angle = MathF.Atan2(dirToMouse.Y, dirToMouse.X);
            float snapAngle = MathF.PI / 4f;
            angle = MathF.Round(angle / snapAngle) * snapAngle;
            rotation = angle;
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
            // Fix: Create a local variable to modify and then reassign
            Vector2 newPosition = position;
            newPosition.X = MathHelper.Clamp(newPosition.X, bounds.Left + texture.Width / 2, bounds.Right - texture.Width / 2);
            newPosition.Y = MathHelper.Clamp(newPosition.Y, bounds.Top + texture.Height / 2, bounds.Bottom - texture.Height / 2);
            position = newPosition;

            foreach (var obs in obstacles)
            {
                if (drect.Intersects(obs))
                {
                    if (isDashing)
                    {
                        isDashing = false;
                        Debug.WriteLine("Dash interrupted by obstacle.");
                    }
                    
                    Vector2 movementDirection = isDashing ? dashDirection : velocity;

                    if (movementDirection != Vector2.Zero)
                    {
                        // Fix: Create a local variable to modify and then reassign
                        Vector2 pushedPosition = position - movementDirection * speed;
                        position = pushedPosition;
                    }
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
                
                // Bullet starts at the player's position
                Vector2 bulletStartPosition = position; 
                currentAmmo--;

                return new Bullet(bulletTexture, bulletStartPosition, direction, 10f, bulletRotation, 2f);
            }
            return null;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}