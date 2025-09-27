using System;
using System.Collections.Generic;
using Ghost_blade;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Enemy
{
    public Vector2 Position;
    public Texture2D Texture;
    public float Speed;
    protected Vector2 direction;
    protected float detectionRadius;
    private readonly int hitboxWidth = 32;
    private readonly int hitboxHeight = 32;
    public bool IsActive { get; set; }
    public int Health { get; set; } = 200;

    // Knockback variables
    protected float knockbackTimer = 0f;
    protected const float KnockbackDuration = 0.2f; // Change this to protected as well for consistency
    protected float knockbackSpeed = 150f;
    protected Vector2 knockbackDirection;

    // Attack variable (only cooldown, attack logic in child class)
    protected float attackCooldown = 1.5f;
    protected float attackTimer;

    public Rectangle boundingBox
    {
        get
        {
            return new Rectangle(
                (int)(Position.X - hitboxWidth / 2),
                (int)(Position.Y - hitboxHeight / 2),
                hitboxWidth,
                hitboxHeight
            );
        }
    }
    public Rectangle bulletammo
    {
        get
        {
            return new Rectangle(
                (int)(Position.X - hitboxWidth / 2),
                (int)(Position.Y - hitboxHeight / 2),
                hitboxWidth,
                hitboxHeight
            );
        }
    }

    public Enemy(Texture2D texture, Vector2 startPosition, float speed, float detectionRadius)
    {
        this.Texture = texture;
        this.Position = startPosition;
        this.Speed = speed;
        this.detectionRadius = detectionRadius;
        this.IsActive = true;
        this.attackTimer = attackCooldown;
    }

    public virtual void Update(Player player, List<Rectangle> obstacles, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (knockbackTimer > 0)
        {
            Position += knockbackDirection * knockbackSpeed * deltaTime;
            knockbackTimer -= deltaTime;
        }
        else
        {
            // Default behavior: Chase the player if they can see them.
            Vector2 desiredMovement = Vector2.Zero;
            if (Vector2.Distance(Position, player.position) <= detectionRadius && CanSeePlayer(player, obstacles))
            {
                desiredMovement = player.position - Position;
                if (desiredMovement != Vector2.Zero)
                {
                    desiredMovement.Normalize();
                }
            }

            Vector2 newPosition = Position + desiredMovement * Speed;
            Position = newPosition;
        }
    }

    public virtual void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        Health -= damage;
        knockbackDirection = Position - damageSourcePosition;
        if (knockbackDirection != Vector2.Zero)
        {
            knockbackDirection.Normalize();
        }
        knockbackTimer = KnockbackDuration;

        if (Health <= 0)
        {
            this.Die();
        }
    }

    public void ClampPosition(Rectangle bounds, List<Rectangle> obstacles)
    {
        Position = Vector2.Clamp(Position,
            new Vector2(bounds.Left + Texture.Width / 2, bounds.Top + Texture.Height / 2),
            new Vector2(bounds.Right - Texture.Width / 2, bounds.Bottom - Texture.Height / 2));

        foreach (var obs in obstacles)
        {
            while (boundingBox.Intersects(obs))
            {
                if (knockbackTimer > 0)
                {
                    knockbackTimer = 0;
                }
                Rectangle intersection = Rectangle.Intersect(boundingBox, obs);
                Vector2 separationVector = Vector2.Zero;
                if (intersection.Width < intersection.Height)
                {
                    if (boundingBox.Center.X < obs.Center.X)
                    {
                        separationVector.X = -intersection.Width;
                    }
                    else
                    {
                        separationVector.X = intersection.Width;
                    }
                }
                else
                {
                    if (boundingBox.Center.Y < obs.Center.Y)
                    {
                        separationVector.Y = -intersection.Height;
                    }
                    else
                    {
                        separationVector.Y = intersection.Height;
                    }
                }
                Position += separationVector;
            }
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (IsActive)
        {
            spriteBatch.Draw(Texture, Position, null, Color.Red, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }

    public bool CanSeePlayer(Player player, List<Rectangle> obstacles)
    {
        if (Vector2.Distance(this.Position, player.position) > detectionRadius)
        {
            return false;
        }
        Vector2 lineOfSight = player.position - this.Position;
        Vector2 normalizedDirection = lineOfSight;
        if (normalizedDirection != Vector2.Zero)
        {
            normalizedDirection.Normalize();
        }
        float distance = lineOfSight.Length();
        float stepSize = 5.0f;
        for (float i = 0; i < distance; i += stepSize)
        {
            Vector2 currentPoint = this.Position + normalizedDirection * i;
            Rectangle pointRect = new Rectangle(
                (int)currentPoint.X,
                (int)currentPoint.Y,
                1,
                1
            );
            foreach (var obs in obstacles)
            {
                if (obs.Intersects(pointRect))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Reset()
    {
        IsActive = true;
        Health = 200;
        knockbackTimer = 0f;
    }
    public Rectangle Die()
    {
        IsActive = false;
        return bulletammo;
    }
}