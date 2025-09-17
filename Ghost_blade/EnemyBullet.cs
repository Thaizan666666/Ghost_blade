using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Ghost_blade
{
    // EnemyBullet inherits all the properties and methods from the base Bullet class.
    public class EnemyBullet : Bullet
    {
        // The constructor calls the base class constructor to set up common properties.
        public EnemyBullet(Texture2D texture, Vector2 startPosition, Vector2 direction, float bulletSpeed, float bulletRotation, float lifeDuration)
            : base(texture, startPosition, direction, bulletSpeed, bulletRotation, lifeDuration)
        {
        }

        // The override keyword lets you specialize the behavior for this class.
        // We add the player as a parameter to check for collision.
        public void Update(GameTime gameTime, List<Rectangle> obstacles, Player player)
        {
            // First, call the base class's Update method to handle generic bullet logic.
            base.Update(gameTime, obstacles);

            // Then, add the specialized collision check for the player.
            if (IsActive && boundingBox.Intersects(player.HitboxgetDamage) && !player.IsInvincible)
            {
                player.TakeDamage(1);
                IsActive = false;
            }
        }
    }
}