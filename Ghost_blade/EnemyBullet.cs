using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ghost_blade
{
    // EnemyBullet inherits all the properties and methods from the base Bullet class.
    public class EnemyBullet : Bullet
    {
        private Texture2D parry;
        // The constructor calls the base class constructor to set up common properties.
        public EnemyBullet(Texture2D texture, Vector2 startPosition, Vector2 direction, float bulletSpeed, float bulletRotation, float lifeDuration, Texture2D parry)
            : base(texture, startPosition, direction, bulletSpeed, bulletRotation, lifeDuration)
        {
            this.parry = parry;
        }

        // The override keyword lets you specialize the behavior for this class.
        // We add the player as a parameter to check for collision.
        public Bullet Update(GameTime gameTime, List<Rectangle> obstacles, Player player)
        {
            // First, call the base class's Update method to handle generic bullet logic.
            base.Update(gameTime, obstacles);
            Bullet parriedBullet = null;
            if (!IsActive) return null;
            // Then, add the specialized collision check for the player.
            if (IsActive && boundingBox.Intersects(player.HitboxgetDamage) && player._isInvincible == false)
            {
                player.TakeDamage(1);
                IsActive = false;
                Debug.WriteLine($"Hp = {player.Health}");
            }

            if (IsActive && boundingBox.Intersects(player.meleeWeapon.ParryHitbox))
            {
                IsActive = false;
                Vector2 parryDirection = -this.velocity;
                parriedBullet = new Bullet(
                    this.parry,
                    this.position,
                    parryDirection,
                    this.speed * 1.5f, // ให้กระสุน Parry เร็วขึ้น
                    this.rotation,
                    this.lifeTime
                );
            }
            return parriedBullet;
        }
        public Bullet Update(GameTime gameTime, List<Rectangle> obstacles, Player player,Vector2 enemyposition)
        {
            // First, call the base class's Update method to handle generic bullet logic.
            base.Update(gameTime, obstacles);
            Bullet parriedBullet = null;
            if (!IsActive) return null;
            // Then, add the specialized collision check for the player.
            if (IsActive && boundingBox.Intersects(player.HitboxgetDamage) && player._isInvincible == false)
            {
                player.TakeDamage(1);
                IsActive = false;
                Debug.WriteLine($"Hp = {player.Health}");
            }

            if (IsActive && boundingBox.Intersects(player.meleeWeapon.ParryHitbox))
            {
                IsActive = false;
                Vector2 parryDirection = player.position - enemyposition;
                parriedBullet = new Bullet(
                    this.parry,
                    this.position,
                    parryDirection,
                    this.speed * 1.5f, // ให้กระสุน Parry เร็วขึ้น
                    this.rotation,
                    this.lifeTime
                );
            }
            return parriedBullet;
        }
    }
}