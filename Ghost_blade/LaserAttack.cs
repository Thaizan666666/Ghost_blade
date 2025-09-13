using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ghost_blade
{
    public class LaserAttack : BossAttack
    {
        private enum LaserState { Charging, Firing, CoolingDown };
        private LaserState currentState;

        private float chargeTimer;
        private float fireTimer;
        private float coolDownTimer;

        private const float CHARGE_DURATION = 1.5f;
        private const float FIRE_DURATION = 5.0f;
        private const float COOL_DOWN_DURATION = 1.0f;

        // The angle of the laser beam
        private float laserAngle;

        // Variables for the sweeping motion
        private float currentRotationSpeed;
        private const float MAX_ROTATION_SPEED = 2f;
        private float rotationDirection; // 1 for clockwise, -1 for counter-clockwise
        private const float OVERSHOOT_TOLERANCE = 0.5f; // The amount the laser will overshoot before changing direction

        private Random random = new Random();

        public LaserAttack(Boss owner, Texture2D pixelTexture) : base(owner, pixelTexture)
        {
        }

        public override void Start(Player player)
        {
            currentState = LaserState.Charging;
            chargeTimer = 0f;
            fireTimer = 0f;
            coolDownTimer = 0f;

            // Randomly start the laser pointing either straight down or straight up
            if (random.Next(2) == 0)
            {
                laserAngle = MathHelper.PiOver2; // 90 degrees (down)
            }
            else
            {
                laserAngle = -MathHelper.PiOver2; // -90 degrees, same as 270 (up)
            }

            // Randomly set the initial rotation direction
            rotationDirection = (random.Next(2) == 0) ? 1 : -1;

            // Start the rotation speed at 0
            currentRotationSpeed = 0f;

            IsFinished = false;
        }

        public override void Update(GameTime gameTime, Player player)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (currentState)
            {
                case LaserState.Charging:
                    chargeTimer += delta;

                    if (chargeTimer >= CHARGE_DURATION)
                    {
                        currentState = LaserState.Firing;
                        fireTimer = 0f;
                    }
                    break;

                case LaserState.Firing:
                    fireTimer += delta;

                    // Gradually increase the rotation speed over the firing duration
                    currentRotationSpeed = MathHelper.Lerp(0, MAX_ROTATION_SPEED, fireTimer / FIRE_DURATION);

                    // Calculate the angle to the player
                    float desiredAngle = (float)Math.Atan2(player.position.Y - boss.Position.Y, player.position.X - boss.Position.X);

                    // Use a slightly modified version of the previous logic to handle the overshoot
                    float angleDifference = MathHelper.WrapAngle(desiredAngle - laserAngle);

                    // Check if the laser has passed the player's position AND the overshoot tolerance
                    // This logic is now more robust against angle wrapping issues.
                    if (rotationDirection == 1 && angleDifference < -OVERSHOOT_TOLERANCE)
                    {
                        rotationDirection = -1; // Passed while moving clockwise, now reverse
                        // Reset the timer to restart the acceleration
                        fireTimer = 0f;
                    }
                    else if (rotationDirection == -1 && angleDifference > OVERSHOOT_TOLERANCE)
                    {
                        rotationDirection = 1; // Passed while moving counter-clockwise, now reverse
                        // Reset the timer to restart the acceleration
                        fireTimer = 0f;
                    }

                    // Update the laser angle based on the determined rotation direction and the accelerating speed
                    laserAngle += currentRotationSpeed * rotationDirection * delta;

                    if (fireTimer >= FIRE_DURATION)
                    {
                        currentState = LaserState.CoolingDown;
                        coolDownTimer = 0f;
                    }
                    break;

                case LaserState.CoolingDown:
                    coolDownTimer += delta;
                    if (coolDownTimer >= COOL_DOWN_DURATION)
                    {
                        IsFinished = true;
                    }
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (currentState == LaserState.Charging)
            {
                // Make the laser blink during the charging phase
                if ((int)(chargeTimer * 5) % 2 == 0)
                {
                    DrawLaser(spriteBatch, 2f, Color.Red * 0.5f);
                }
            }
            else if (currentState == LaserState.Firing)
            {
                // Draw the full-strength laser while firing
                DrawLaser(spriteBatch, 4f, Color.Red);
            }
        }

        // Helper method to draw the laser
        private void DrawLaser(SpriteBatch spriteBatch, float thickness, Color color)
        {
            float laserLength = 800f;
            Vector2 origin = new Vector2(0, thickness / 2);
            spriteBatch.Draw(
                pixelTexture,
                boss.Position,
                null,
                color,
                laserAngle,
                origin,
                new Vector2(laserLength, thickness),
                SpriteEffects.None,
                0f
            );
        }
    }
}
