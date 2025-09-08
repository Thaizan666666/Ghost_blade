using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

public class Bullet
{
    private Texture2D texture;
    private Vector2 position;
    private Vector2 velocity;
    private float rotation;
    private float speed;
    private float lifeTime;
    private float currentLifeTime;
    private readonly int hitboxSize = 10;

    public bool IsActive { get; set; }

    public Rectangle boundingBox
    {
        get
        {
            return new Rectangle((int)position.X, (int)position.Y, hitboxSize, hitboxSize);
        }
    }

    public Bullet(Texture2D texture, Vector2 startPosition, Vector2 direction, float bulletSpeed, float bulletRotation, float lifeDuration)
    {
        this.texture = texture;
        this.position = startPosition;
        this.velocity = direction;
        this.speed = bulletSpeed;
        this.rotation = bulletRotation;
        this.lifeTime = lifeDuration;
        this.currentLifeTime = 0f;
        this.IsActive = true;
    }

    // เพิ่ม List<Rectangle> obstacles เป็นพารามิเตอร์ เพื่อให้กระสุนตรวจสอบการชนกับกำแพงได้เอง
    public void Update(GameTime gameTime, List<Rectangle> obstacles)
    {
        if (!IsActive) return;

        // ตรวจสอบการชนกับกำแพง
        foreach (var obstacle in obstacles)
        {
            if (boundingBox.Intersects(obstacle))
            {
                IsActive = false;
                return;
            }
        }

        // อัปเดตตำแหน่งตามความเร็ว โดยใช้ delta time เพื่อการเคลื่อนที่ที่ราบรื่น
        position += velocity * speed * (float)gameTime.ElapsedGameTime.TotalSeconds * 60f;

        // อัปเดตระยะเวลา
        currentLifeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // ปิดการใช้งานกระสุนเมื่อหมดอายุ
        if (currentLifeTime >= lifeTime)
        {
            IsActive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;

        Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

        spriteBatch.Draw(
            texture,
            position,
            null,
            Color.White,
            rotation,
            origin,
            1.0f,
            SpriteEffects.None,
            0f
        );
    }
}