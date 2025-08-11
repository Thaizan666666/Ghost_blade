using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ghost_blade
{
    public class Player
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 velocity;
        private float speed;
        private SpriteEffects currentSpriteEffect;
        private float fireDelay = 0.2f;
        private float timer = 0f;
        private Texture2D bulletTexture;
        public byte currentAmmo;
        bool switch_sword = true;
        private KeyboardState previousKState;

        private bool isDashing = false;
        private float dashTimer = 0f;
        private float dashDuration = 0.2f;
        private float dashSpeedMultiplier = 5.0f;
        private Vector2 dashDirection;

        private float dashCooldown = 2f;
        private float dashCooldownTimer = 0f;
        public Rectangle drect
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); }
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

        public void Update(GameTime gameTime)
        {
            KeyboardState kState = Keyboard.GetState();

            if (dashCooldownTimer > 0)
            {
                Debug.WriteLine($"before = {dashCooldownTimer}");
                dashCooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                Debug.WriteLine($"After = {dashCooldownTimer}");
            }

            if (kState.IsKeyDown(Keys.Space) && !previousKState.IsKeyDown(Keys.Space) && !isDashing && dashCooldownTimer <= 0)
            {
                {
                    isDashing = true;
                    dashTimer = dashDuration;
                    dashCooldownTimer = dashCooldown;

                    if (velocity != Vector2.Zero)
                    {
                        dashDirection = velocity;
                    }
                    else
                    {
                        if (currentSpriteEffect == SpriteEffects.FlipHorizontally)
                        {
                            dashDirection = new Vector2(-1, 0);
                        }
                        else
                        {
                            dashDirection = new Vector2(1, 0);
                        }
                    }
                    if (dashDirection != Vector2.Zero)
                    {
                        dashDirection.Normalize();
                    }

                    Debug.WriteLine("Dash Activated!");
                }
            }

            if (!isDashing)
            {
                velocity = Vector2.Zero;

                if (kState.IsKeyDown(Keys.W)) { velocity.Y -= 1; }
                else if (kState.IsKeyDown(Keys.S)) { velocity.Y += 1; }
                if (kState.IsKeyDown(Keys.D)) { velocity.X += 1; }
                else if (kState.IsKeyDown(Keys.A)) { velocity.X -= 1; }

                if (velocity != Vector2.Zero)
                {
                    velocity.Normalize();
                }
            }

            if (isDashing)
            {
                position += dashDirection * speed * dashSpeedMultiplier;
                // Corrected dash timer update: remove the multiplier to ensure a consistent dash duration
                dashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (dashTimer <= 0)
                {
                    isDashing = false; // End Dash
                    Debug.WriteLine("Dash Ended.");
                }
            }
            else
            {
                position += velocity * speed;
            }

            if (kState.IsKeyDown(Keys.R))
            {
                this.currentAmmo = 10;
                Debug.WriteLine("Ammo = 10");
            }
            if (kState.IsKeyDown(Keys.E) && !previousKState.IsKeyDown(Keys.E))
            {
                switch_sword = !switch_sword;
                if (switch_sword)
                {
                    Debug.WriteLine("Weapon: Sword");
                }
                else
                {
                    Debug.WriteLine("Weapon: Gun");
                }
            }

            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);

            if (mousePosition.X < position.X)
            {
                currentSpriteEffect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                currentSpriteEffect = SpriteEffects.None;
            }

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            previousKState = kState;
        }

        public Bullet Shoot(Vector2 mousePosition)
        {
            if (currentAmmo <= 0 || switch_sword == true)
            {
                Debug.WriteLine("Ammo = 0");
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

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            spriteBatch.Draw(
                texture,
                position,
                null,
                Color.White,
                0f,
                origin,
                1.0f,
                currentSpriteEffect,
                0f
            );
        }
    }
}