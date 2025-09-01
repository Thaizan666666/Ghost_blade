using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Enemy
{
    public Vector2 Position;
    public Texture2D Texture;
    public float Speed;
    private Vector2 direction;
    private float detectionRadius;

    public Enemy(Texture2D texture, Vector2 startPosition, float speed, float detectionRadius)
    {
        this.Texture = texture;
        this.Position = startPosition;
        this.Speed = speed;
        this.detectionRadius = detectionRadius;
    }

    public void Update(Vector2 playerPosition)
    {
        float distance = Vector2.Distance(Position, playerPosition);

        if (distance <= detectionRadius)
        {
            direction = playerPosition - Position;
            direction.Normalize();

            Position += direction * Speed;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color.Red);
    }
}