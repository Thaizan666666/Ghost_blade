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

    // ** New variables for Parry State **
    private bool _isParryActive = false;
    public float _parryTimer { get; private set; } = 0f;
    // Set a very short window for parry at the start of the swing
    private float _parryDuration = 1f;

    private bool _isULTActive = false;
    public float _ultTimer = 0f;
    private float _ultTotalDuration = 2.0f; // ระยะเวลารวมของ ULT (2.0 วินาที)
    private float _ultHitboxInterval = 0.2f; // ช่วงเวลาที่ต้องสร้าง Hitbox (0.2 วินาที)
    private float _ultHitboxCooldown = 0f; // ตัวนับเวลาสำหรับช่วง 0.2 วิ

    public Rectangle AttackHitbox { get; private set; }
    public Rectangle ParryHitbox { get; private set; }
    public Rectangle ULTHitbox { get; private set; }
    public int Attack {  get; private set; }

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
        if (_isParryActive)
        {
            _parryTimer -= deltaTime;
            if (_parryTimer <= 0)
            {
                _isParryActive = false;
                ParryHitbox = Rectangle.Empty; // Parry Hitbox is cleared here
            }
        }
        else if (!_isAttackingActive)
        {
            // Only clear the ParryHitbox if neither attack nor parry is active
            ParryHitbox = Rectangle.Empty;
        }
        // --- END NEW PARRY LOGIC ---
        if (_isULTActive)
        {
            _ultTimer -= deltaTime;
            _ultHitboxCooldown -= deltaTime;

            // เช็คว่าหมดเวลา ULT แล้วหรือไม่
            if (_ultTimer <= 0)
            {
                _isULTActive = false;
                ULTHitbox = Rectangle.Empty; // ล้าง Hitbox เมื่อ ULT จบลง
                _ultHitboxCooldown = 0f;
            }
            // ถ้าถึงช่วงเวลาที่ต้องสร้าง Hitbox ใหม่ (ทุก 0.2 วินาที)
            else if (_ultHitboxCooldown <= 0)
            {
                // สร้าง Hitbox ขนาดใหญ่รอบตัวผู้เล่น
                ULTHitbox = new Rectangle(
                    (int)playerPosition.X - 900 / 2,
                    (int)playerPosition.Y - 700 / 2,
                    900,
                    700
                );
                Attack = 5; // กำหนดค่า Attack สำหรับการโจมตี ULT
                _ultHitboxCooldown = _ultHitboxInterval; // รีเซ็ตตัวนับสำหรับช่วงเวลาถัดไป
            }
            else // ในช่วงระหว่างการสร้าง Hitbox (0.2 วิ) ให้เคลียร์ Hitbox
            {
                ULTHitbox = Rectangle.Empty;
            }
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

            int hitboxSize1 = 156;
            int hitboxSize2 = 72;
            int distance = 24;

            if (angle >= MathF.PI * 0.25f && angle < MathF.PI * 0.75f) // (ล่าง)
            {
                AttackHitbox = new Rectangle((int)playerPosition.X - 100, (int)playerPosition.Y + distance * 2, 156, hitboxSize2);
                Attack = 1;
            }
            else if (angle >= MathF.PI * 0.75f && angle < MathF.PI * 1.25f) // (ซ้าย)
            {
                AttackHitbox = new Rectangle((int)playerPosition.X - distance - 96, (int)playerPosition.Y - hitboxSize1 / 2, hitboxSize2, hitboxSize1);
                Attack = 2;
            }
            else if (angle >= MathF.PI * 1.25f && angle < MathF.PI * 1.75f) //(บน)
            {
                AttackHitbox = new Rectangle((int)playerPosition.X - 100, (int)playerPosition.Y - distance * 2 - hitboxSize2, 156, hitboxSize2);
                Attack = 3;
            }
            else //(ขวา)
            {
                AttackHitbox = new Rectangle((int)playerPosition.X, (int)playerPosition.Y - hitboxSize1 / 2, hitboxSize2, hitboxSize1);
                Attack = 4;
            }
        }
    }
    public void PerformParry(Vector2 playerPosition, Vector2 cameraPosition)
    {
        // Only allow parry if not already parrying and not in the middle of a full attack swing
        if (!_isParryActive && !_isAttackingActive)
        {
            _isParryActive = true;
            _parryTimer = _parryDuration;

            // Recalculate the angle based on the current mouse position 
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosWorld = new Vector2(mouseState.X, mouseState.Y) - cameraPosition;
            Vector2 dirToMouse = mousePosWorld - playerPosition;
            float angle = MathF.Atan2(dirToMouse.Y, dirToMouse.X);

            // Snap the rotation for the sprite, even if the parry hitbox is simple
            rotation = MathF.Round(angle / snapAngle) * snapAngle;

            // Define the Parry Hitbox: A small, centered, precise area for deflection
            int parrySize = 72;
            ParryHitbox = new Rectangle(
                (int)playerPosition.X - 60,
                (int)playerPosition.Y - 60,
                parrySize,
                parrySize * 2
            );
        }
    }
    public void PerformULT(Vector2 playerPosition)
    {
        if (!_isULTActive)
        {
            _isULTActive = true;
            _ultTimer = _ultTotalDuration;    // เริ่มนับเวลารวม (2 วิ)
            _ultHitboxCooldown = 0f;          // ตั้งให้สร้าง Hitbox ทันทีในการ Update ครั้งถัดไป
        }
    }
}