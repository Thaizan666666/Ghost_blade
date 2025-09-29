// ต้องเพิ่ม using System.Diagnostics;
using Ghost_blade;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Boss
{
    // Properties
    public int Health { get; set; } = 1500;
    public Vector2 Position;
    public Texture2D pixel;
    public Texture2D bulletTexture;

    // Boss State
    private enum BossState { Idle, Attacking, Damaged };
    private BossState currentState;
    private float attackTimer;

    // Attack management
    private List<BossAttack> attacks;
    private BossAttack currentAttack;
    private Random random;
    private float timeBetweenAttacks;

    // Constructor
    public Boss(Texture2D texture, Vector2 position, Texture2D pixelTexture, Texture2D enemyTex1, Texture2D enemyTex2, Texture2D bulletTexture)
    {
        this.Position = position;
        this.pixel = pixelTexture;
        this.bulletTexture = bulletTexture;
        this.random = new Random();
        this.timeBetweenAttacks = 0.5f;

        attacks = new List<BossAttack>();

        // Pass the pixel texture for attacks that use it
        attacks.Add(new LaserAttack(this, pixelTexture));
        attacks.Add(new BossBulletAttacks(this, bulletTexture));
        attacks.Add(new SpawnAttack(this, pixelTexture, enemyTex1, enemyTex2, bulletTexture));

        currentState = BossState.Idle;
        attackTimer = 0.1f;
    }

    public void Update(GameTime gameTime, Player player, List<Rectangle> obstacles)
    {
        // Update logic based on the current state
        switch (currentState)
        {
            case BossState.Idle:
                // Count down to the next attack
                attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (attackTimer >= timeBetweenAttacks)
                {
                    SelectNewAttack(player);
                }
                break;

            case BossState.Attacking:
                // Update the current attack
                if (currentAttack != null)
                {
                    currentAttack.Update(gameTime, player, obstacles);

                    // Check if the attack is finished
                    if (currentAttack.IsFinished)
                    {
                        currentAttack = null; // Clear the current attack
                        currentState = BossState.Idle; // Return to Idle state
                        attackTimer = 0f; // Reset the timer for the next attack
                    }
                }
                break;

            case BossState.Damaged:
                // Handle a brief stun or visual effect
                break;
        }
        // NOTE: The code to get spawned enemies must be called from Game1's Update method
        // You cannot add them to the main list from here.
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Define the boss's drawing rectangle
        Rectangle bossRect = new Rectangle(
            (int)(Position.X),
            (int)(Position.Y),
            1632, // width
            1055  // height
        );

        // Draw the red square using the 1x1 pixel texture
        spriteBatch.Draw(pixel, bossRect, Color.Brown);

        // THIS IS THE MISSING PART: Draw the current attack's visuals
        if (currentAttack != null)
        {
            currentAttack.Draw(spriteBatch);
        }
    }

    private void SelectNewAttack(Player player)
    {
        // เก็บดัชนีของการโจมตีที่ถูกเลือกไว้
        int randomIndex;
        BossAttack selectedAttack;
        bool isValidAttack = false;

        // วนลูปเพื่อสุ่มการโจมตีจนกว่าจะได้การโจมตีที่ถูกต้องตามเงื่อนไข
        while (!isValidAttack)
        {
            randomIndex = random.Next(attacks.Count);
            selectedAttack = attacks[randomIndex];

            // 1. ตรวจสอบเงื่อนไขพิเศษสำหรับ SpawnAttack
            if (selectedAttack is SpawnAttack spawnAttack)
            {
                // ตรวจสอบว่ายังมีศัตรูที่ถูกเสกมาก่อนหน้าเหลืออยู่หรือไม่
                if (spawnAttack.GetActiveSpawnedEnemyCount() > 0)
                {
                    isValidAttack = false;
                }
                else
                {
                    // ถ้าศัตรูหมดแล้ว หรือเป็นการโจมตีใหม่ที่พร้อมจะเริ่ม
                    isValidAttack = true;
                    currentAttack = selectedAttack;
                }
            }
            else
            {
                // 2. การโจมตีอื่น ๆ (เช่น Laser, Bullet) ถือว่า 'ถูกต้อง' เสมอ
                isValidAttack = true;
                currentAttack = selectedAttack;
            }
        }

        // เมื่อได้ currentAttack ที่ถูกต้องแล้ว
        currentState = BossState.Attacking;
        Debug.WriteLine($"Attack = {currentAttack.GetType().Name}");
        currentAttack.Start(player);
    }

    // Method to get newly spawned enemies to add to the main game list
    public List<Enemy> GetSpawnedEnemies()
    {
        if (currentAttack is SpawnAttack spawnAttack)
        {
            return spawnAttack.GetNewEnemies();
        }
        return new List<Enemy>(); // Return an empty list if it's not a spawn attack
    }
}