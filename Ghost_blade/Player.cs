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


            velocity = Vector2.Zero;


            if (kState.IsKeyDown(Keys.W))
            {
                velocity.Y -= 1;
            }
            if (kState.IsKeyDown(Keys.S))
            {
                velocity.Y += 1;
            }
            if (kState.IsKeyDown(Keys.D))
            {
                velocity.X += 1;
            }
            if (kState.IsKeyDown(Keys.A))
            {
                velocity.X -= 1;
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

            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
            }

            position += velocity * speed;

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
