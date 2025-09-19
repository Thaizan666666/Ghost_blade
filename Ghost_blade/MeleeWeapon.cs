using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

public class MeleeWeapon
{
    private Texture2D texture;
    private Vector2 position; // ตำแหน่งจุดหมุนของอาวุธ (ตรงกับผู้เล่น)
    private Vector2 origin;
    private float rotation;
    private float realhitbox;
    private float snapAngle = MathF.PI / 4f; // 45 องศา

    public Rectangle AttackHitbox { get; private set; }

    public MeleeWeapon(Texture2D weaponTexture)
    {
        this.texture = weaponTexture;
        this.origin = new Vector2(texture.Width / 2, texture.Height / 2);
    }

    public void Update(GameTime gameTime, Vector2 playerPosition, Vector2 cameraPosition, bool isAttacking)
    {
        // อัปเดตตำแหน่งจุดหมุนของอาวุธให้ตรงกับผู้เล่น
        this.position = playerPosition;

        MouseState mouseState = Mouse.GetState();
        // แปลงพิกัดเมาส์จาก Screen Space เป็น World Space
        Vector2 mousePosWorld = new Vector2(mouseState.X, mouseState.Y) - cameraPosition;
        Vector2 dirToMouse = mousePosWorld - position;

        // คำนวณมุมและล็อกให้อยู่ใน 8 ทิศทางหลัก
        float angle = MathF.Atan2(dirToMouse.Y, dirToMouse.X);
        rotation = MathF.Round(angle / snapAngle) * snapAngle;

        // กำหนดขนาดและระยะห่างของ Hitbox จากผู้เล่น
        if (angle < 0)
        {
            angle += MathF.PI * 2;
        }

        // กำหนดขนาดและระยะห่างของ Hitbox จากผู้เล่น
        if (isAttacking)
        {
            // กำหนดขนาดและระยะห่างของ Hitbox จากผู้เล่น
            int hitboxSize = 50;
            int distance = 40;

            // ตรวจสอบมุมให้อยู่ในแต่ละ 90 องศา quadrant
            if (angle >= MathF.PI * 0.25f && angle < MathF.PI * 0.75f) // 45 ถึง 135 องศา (บน)
            {
                rotation = MathF.PI / 2f;
                realhitbox = 0f;
                AttackHitbox = new Rectangle((int)position.X - hitboxSize / 2, (int)position.Y + hitboxSize, hitboxSize, hitboxSize);
            }
            else if (angle >= MathF.PI * 0.75f && angle < MathF.PI * 1.25f) // 135 ถึง 225 องศา (ซ้าย)
            {
                rotation = MathF.PI;
                realhitbox = 1f;
                AttackHitbox = new Rectangle((int)position.X - distance - hitboxSize, (int)position.Y - hitboxSize / 2, hitboxSize, hitboxSize);
            }
            else if (angle >= MathF.PI * 1.25f && angle < MathF.PI * 1.75f) // 225 ถึง 315 องศา (ล่าง)
            {
                rotation = 3f * MathF.PI / 2f;
                realhitbox = 0f;
                AttackHitbox = new Rectangle((int)position.X - hitboxSize / 2, (int)position.Y - distance - hitboxSize, hitboxSize, hitboxSize);
            }
            else // ที่เหลือคือ 315 ถึง 45 องศา (ขวา)
            {
                rotation = 0f;
                realhitbox = 1f;
                AttackHitbox = new Rectangle((int)position.X + distance, (int)position.Y - hitboxSize / 2, hitboxSize, hitboxSize);
            }
        }
        else
        {
            // ถ้าไม่ได้โจมตี ให้ Hitbox ว่างเปล่าเพื่อป้องกันการชนที่ไม่ตั้งใจ
            AttackHitbox = Rectangle.Empty;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

        // วาดอาวุธโดยใช้ค่า rotation ที่ล็อกไว้
        spriteBatch.Draw(
            texture,
            position,
            null,
            Color.White,
            realhitbox,
            origin,
            1.0f,
            SpriteEffects.None,
            0f
        );
    }
}