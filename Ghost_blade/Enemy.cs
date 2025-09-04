// ในไฟล์ Enemy.cs
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
    private const int HitboxWidth = 32;
    private const int HitboxHeight = 32;

    public Rectangle BoundingBox
    {
        get
        {
            return new Rectangle(
                (int)(Position.X - HitboxWidth / 2),
                (int)(Position.Y - HitboxHeight / 2),
                HitboxWidth,
                HitboxHeight
            );
        }
    }

    public Enemy(Texture2D texture, Vector2 startPosition, float speed, float detectionRadius)
    {
        this.Texture = texture;
        this.Position = startPosition;
        this.Speed = speed;
        this.detectionRadius = detectionRadius;
    }

    // แก้ไข: เมธอด Update หลักที่ใช้สำหรับศัตรูทั่วไป
    public virtual void Update(Vector2 playerPosition, List<Rectangle> obstacles)
    {
        oldPosition = Position;
        float distance = Vector2.Distance(Position, playerPosition);
        Vector2 desiredMovement = Vector2.Zero;

        if (distance <= detectionRadius)
        {
            desiredMovement = playerPosition - Position;
            if (desiredMovement != Vector2.Zero)
            {
                desiredMovement.Normalize();
            }
        }

        Vector2 newPosition = Position + desiredMovement * Speed;
        Position = HandleCollision(newPosition, obstacles);
    }

    // *** แก้ไข: เพิ่มเมธอด Update ที่รับ desiredMovement เพื่อให้คลาสลูกเรียกใช้ ***
    public virtual void Update(Vector2 playerPosition, List<Rectangle> obstacles, Vector2 desiredMovement)
    {
        oldPosition = Position;
        Vector2 newPosition = Position + desiredMovement * Speed;
        Position = HandleCollision(newPosition, obstacles);
    }

    protected Vector2 HandleCollision(Vector2 newPosition, List<Rectangle> obstacles)
    {
        Vector2 resultPosition = newPosition;

        Rectangle rectX = new Rectangle(
            (int)(newPosition.X - HitboxWidth / 2),
            (int)(Position.Y - HitboxHeight / 2),
            HitboxWidth,
            HitboxHeight
        );

        foreach (var obs in obstacles)
        {
            if (rectX.Intersects(obs))
            {
                resultPosition.X = Position.X;
                break;
            }
        }

        Rectangle rectY = new Rectangle(
            (int)(Position.X - HitboxWidth / 2),
            (int)(newPosition.Y - HitboxHeight / 2),
            HitboxWidth,
            HitboxHeight
        );

        foreach (var obs in obstacles)
        {
            if (rectY.Intersects(obs))
            {
                resultPosition.Y = Position.Y;
                break;
            }
        }

        return resultPosition;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, Color.Red, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0f);
    }
}