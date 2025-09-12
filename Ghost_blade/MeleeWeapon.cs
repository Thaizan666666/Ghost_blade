using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ghost_blade
{
    /// <summary>
    /// Represents the player's melee weapon, handling its state and attack logic.
    /// </summary>
    public class MeleeWeapon
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 origin;
        private float rotation;

        // Melee attack state
        private bool isSwinging = false;
        private float swingTimer = 0f;
        private const float swingDuration = 0.2f; // The duration of the swing in seconds

        public MeleeWeapon(Texture2D weaponTexture)
        {
            this.texture = weaponTexture;
            // The origin is at the base of the weapon to rotate correctly from the player
            this.origin = new Vector2(0, texture.Height / 2);
        }

        public void Update(GameTime gameTime, Vector2 playerPosition, float playerRotation)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update the weapon's position and rotation relative to the player.
            // We rotate the position offset to make the weapon circle around the player.
            Vector2 offset = new Vector2(texture.Width / 2, 0);
            Matrix rotationMatrix = Matrix.CreateRotationZ(playerRotation);
            Vector2 rotatedOffset = Vector2.Transform(offset, rotationMatrix);
            this.position = playerPosition + rotatedOffset;
            this.rotation = playerRotation;

            // Handle the swing timer
            if (isSwinging)
            {
                swingTimer += deltaTime;
                if (swingTimer >= swingDuration)
                {
                    isSwinging = false;
                    swingTimer = 0f;
                }
            }
        }

        /// <summary>
        /// Starts the melee swing animation.
        /// </summary>
        public void Swing()
        {
            // Only swing if not already in the middle of a swing
            if (!isSwinging)
            {
                isSwinging = true;
                swingTimer = 0f;
                System.Diagnostics.Debug.WriteLine("Sword swing activated!");
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the weapon when it is swinging
            if (isSwinging)
            {
                // The weapon is drawn at its calculated position, rotated and scaled
                spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        /// <summary>
        /// Gets the collision rectangle for the active swing.
        /// </summary>
        public Rectangle AttackRectangle
        {
            get
            {
                if (!isSwinging)
                {
                    return Rectangle.Empty; // Return an empty rectangle if not swinging
                }

                // Calculate the rotated rectangle for collision detection.
                // This is a simplified AABB (Axis-Aligned Bounding Box) for the sword's current position.
                return new Rectangle(
                    (int)(position.X - origin.X),
                    (int)(position.Y - origin.Y),
                    (int)texture.Width,
                    (int)texture.Height
                );
            }
        }
    }
}
