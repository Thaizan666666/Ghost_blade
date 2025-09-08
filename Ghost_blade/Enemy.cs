using Ghost_blade;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public class Enemy
{
    public Vector2 Position;
    public Texture2D Texture;
    public float Speed;
    protected Vector2 direction;
    protected float detectionRadius;
    private Vector2 oldPosition;
    private readonly int hitboxWidth = 32;
    private readonly int hitboxHeight = 32;
    public bool IsActive { get; set; }


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

    public Enemy(Texture2D texture, Vector2 startPosition, float speed, float detectionRadius)
    {
        this.Texture = texture;
        this.Position = startPosition;
        this.Speed = speed;
        this.detectionRadius = detectionRadius;
        this.IsActive = true; // Add this line
    }

    // Main Update method for the enemy's logic.
    public virtual void Update(Player player, List<Rectangle> obstacles)
    {
        oldPosition = Position;
        Vector2 desiredMovement = Vector2.Zero;

        // Check if the player is both within the detection radius AND has a clear line of sight.
        if (Vector2.Distance(Position, player.position) <= detectionRadius && CanSeePlayer(player, obstacles))
        {
            // If true, the enemy will actively pursue the player.
            desiredMovement = player.position - Position;
            if (desiredMovement != Vector2.Zero)
            {
                desiredMovement.Normalize();
            }
        }
        else
        {
            // If the player is not in range or the line of sight is blocked, the enemy will stop moving
            // or you could add logic for a patrolling behavior here.
            desiredMovement = Vector2.Zero;
        }

        // Apply movement
        Vector2 newPosition = Position + desiredMovement * Speed;

        // Handle collision after calculating the new position.
        Position = HandleCollision(newPosition, obstacles);
    }

    protected Vector2 HandleCollision(Vector2 newPosition, List<Rectangle> obstacles)
    {
        Vector2 resultPosition = newPosition;

        // Separate collision checks for X and Y axes to allow sliding along walls.

        // Check X-axis movement
        Rectangle rectX = new Rectangle(
            (int)(newPosition.X - hitboxWidth / 2),
            (int)(Position.Y - hitboxHeight / 2),
            hitboxWidth,
            hitboxHeight
        );

        foreach (var obs in obstacles)
        {
            if (rectX.Intersects(obs))
            {
                resultPosition.X = Position.X;
                break; // Stop checking once a collision is found.
            }
        }

        // Check Y-axis movement
        Rectangle rectY = new Rectangle(
            (int)(Position.X - hitboxWidth / 2),
            (int)(newPosition.Y - hitboxHeight / 2),
            hitboxWidth,
            hitboxHeight
        );

        foreach (var obs in obstacles)
        {
            if (rectY.Intersects(obs))
            {
                resultPosition.Y = Position.Y;
                break; // Stop checking once a collision is found.
            }
        }

        return resultPosition;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        // Draw enemy centered on its position
        spriteBatch.Draw(Texture, Position, null, Color.Red, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
    }
    public bool CanSeePlayer(Player player, List<Rectangle> obstacles)
    {
        // First, check if the player is within the maximum detection radius.
        // This is an optimization to avoid unnecessary calculations.
        if (Vector2.Distance(this.Position, player.position) > detectionRadius)
        {
            return false;
        }

        // Create a vector representing the line between the enemy and the player.
        Vector2 lineOfSight = player.position - this.Position;
        Vector2 normalizedDirection = lineOfSight;
        if (normalizedDirection != Vector2.Zero)
        {
            normalizedDirection.Normalize();
        }

        // Determine the number of steps to check along the line.
        // The smaller the step size, the more accurate the check, but it's more computationally expensive.
        float distance = lineOfSight.Length();
        float stepSize = 5.0f; // Adjust this value for accuracy vs. performance.

        // Step from the enemy's position towards the player.
        for (float i = 0; i < distance; i += stepSize)
        {
            // Calculate the position of the current point on the line.
            Vector2 currentPoint = this.Position + normalizedDirection * i;

            // Create a small rectangle for the collision check.
            // This is to account for the fact that a single point might not register a collision.
            Rectangle pointRect = new Rectangle(
                (int)currentPoint.X,
                (int)currentPoint.Y,
                1,
                1
            );

            // Check for intersection with any obstacle.
            foreach (var obs in obstacles)
            {
                if (obs.Intersects(pointRect))
                {
                    // Line of sight is blocked.
                    return false;
                }
            }
        }

        // If the loop completes without an intersection, the line of sight is clear.
        return true;
    }
}