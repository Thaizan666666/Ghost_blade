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
    private float snapAngle = MathF.PI / 4f; // 45 องศา

    // New variables for attack state and duration
    private bool _isAttackingActive = false;
    private float _attackTimer = 0f;
    private float _attackDuration = 0.2f; // ระยะเวลาการโจมตี

    public Rectangle AttackHitbox { get; private set; }

    public MeleeWeapon(Texture2D weaponTexture)
    {
        this.texture = weaponTexture;
        this.origin = new Vector2(texture.Width / 2, texture.Height / 2);
    }

    public void Update(GameTime gameTime, Vector2 playerPosition, Vector2 cameraPosition)
    {
        // อัปเดตตำแหน่งจุดหมุนของอาวุธให้ตรงกับผู้เล่น
        this.position = playerPosition;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_isAttackingActive)
        {
            // ถ้ากำลังโจมตี ให้นับเวลาถอยหลังและคงทิศทางอาวุธไว้
            _attackTimer -= deltaTime;
            if (_attackTimer <= 0)
            {
                _isAttackingActive = false;
                AttackHitbox = Rectangle.Empty;
            }
        }
        else
        {
            // ถ้าไม่ได้โจมตี ให้อาวุธหันตามเมาส์
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosWorld = new Vector2(mouseState.X, mouseState.Y) - cameraPosition;
            Vector2 dirToMouse = mousePosWorld - position;
            float angle = MathF.Atan2(dirToMouse.Y, dirToMouse.X);
            rotation = MathF.Round(angle / snapAngle) * snapAngle;
            AttackHitbox = Rectangle.Empty;
        }
    }

    // เมธอดใหม่สำหรับเริ่มการโจมตี ซึ่งจะถูกเรียกเมื่อกดปุ่มโจมตี
    public void PerformAttack(Vector2 playerPosition, Vector2 cameraPosition)
    {
        if (!_isAttackingActive)
        {
            _isAttackingActive = true;
            _attackTimer = _attackDuration; // ตั้งเวลาการโจมตี

            // คำนวณทิศทางและ Hitbox ณ ตอนที่เริ่มโจมตี (จะไม่เปลี่ยนจนกว่าจะหมดเวลา)
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosWorld = new Vector2(mouseState.X, mouseState.Y) - cameraPosition;
            Vector2 dirToMouse = mousePosWorld - playerPosition;
            float angle = MathF.Atan2(dirToMouse.Y, dirToMouse.X);

            // ใช้ rotation สำหรับ sprite
            rotation = MathF.Round(angle / snapAngle) * snapAngle;

            // กำหนด Hitbox ตามมุมที่ได้ (เหมือนโค้ดเดิม)
            if (angle < 0)
            {
                angle += MathF.PI * 2;
            }

            int hitboxSize1 = 96;
            int hitboxSize2 = 36;
            int distance = 24;

            if (angle >= MathF.PI * 0.25f && angle < MathF.PI * 0.75f) // (ล่าง)
            {
                AttackHitbox = new Rectangle((int)playerPosition.X - 60, (int)playerPosition.Y + 24, 96, hitboxSize2);
            }
            else if (angle >= MathF.PI * 0.75f && angle < MathF.PI * 1.25f) // (ซ้าย)
            {
                AttackHitbox = new Rectangle((int)playerPosition.X - distance - hitboxSize2, (int)playerPosition.Y - hitboxSize1 / 2, hitboxSize2, hitboxSize1);
            }
            else if (angle >= MathF.PI * 1.25f && angle < MathF.PI * 1.75f) //(บน)
            {
                AttackHitbox = new Rectangle((int)playerPosition.X - 60, (int)playerPosition.Y - distance - hitboxSize2, 96, hitboxSize2);
            }
            else //(ขวา)
            {
                AttackHitbox = new Rectangle((int)playerPosition.X, (int)playerPosition.Y - hitboxSize1 / 2, hitboxSize2, hitboxSize1);
            }
        }
    }
}