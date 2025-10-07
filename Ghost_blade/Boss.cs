// ต้องเพิ่ม using System.Diagnostics;
using _321_Lab05_3;
using Ghost_blade;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Boss
{
    // Properties
    public int MaxHealth { get; set; } = 1500;
    public int Health { get; set; } = 1500;
    public Vector2 Position;
    public Texture2D pixel;
    public Texture2D bulletTexture;
    public Texture2D parrybulletTexture;
    public int Boss_width = 1056;
    public int Boss_height = 528;
    public bool IsbossAticve { get; set; }
    public int randomIndex { get; set; }
    public bool isValidAttack {  get; private set; }

    // Boss State
    public enum BossState { Idle, Attacking, Damaged };
    public BossState currentState { get;private set; }
    private float attackTimer;

    // Attack management
    private List<BossAttack> attacks;
    private BossAttack currentAttack;
    private Random random;
    public float timeBetweenAttacks {  get; private set; }
    public Rectangle HitboxgetDamage
    {
        get
        {
            return new Rectangle(
                        (int)(Position.X),
                        (int)(Position.Y),
                        Boss_width,
                        Boss_height
                        );
        }
    }

    // Constructor
    public Boss(Texture2D texture, Vector2 position, Texture2D pixelTexture, 
        AnimatedTexture Enemymelee_Idle, AnimatedTexture Enemymelee_Walk ,AnimatedTexture Enemymelee_Attack, AnimatedTexture Enemymelee_Death,
        AnimatedTexture EnemyShooting_Idle, AnimatedTexture EnemyShooting_Walk, AnimatedTexture EnemyShooting_Death,
        Texture2D enemyTex1, Texture2D enemyTex2, Texture2D bulletTexture,Texture2D parry)
    {
        this.Position = position;
        this.pixel = pixelTexture;
        this.bulletTexture = bulletTexture;
        this.parrybulletTexture = parry;
        this.random = new Random();
        this.timeBetweenAttacks = 1.0f;

        attacks = new List<BossAttack>();

        // Pass the pixel texture for attacks that use it
        attacks.Add(new LaserAttack(this, pixelTexture));
        attacks.Add(new BossBulletAttacks(this, bulletTexture, parry));
        attacks.Add(new SpawnAttack(this, pixelTexture, Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Death, Enemymelee_Attack, EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, enemyTex1, enemyTex2, bulletTexture, parry));

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
            Boss_width, // width
            Boss_height  // height
        );

            // Draw the red square using the 1x1 pixel texture
            //spriteBatch.Draw(pixel, bossRect, Color.Brown);

            // THIS IS THE MISSING PART: Draw the current attack's visuals
            if (currentAttack != null)
            {
                currentAttack.Draw(spriteBatch);
            }
    }

    private void SelectNewAttack(Player player)
    {
        // เก็บดัชนีของการโจมตีที่ถูกเลือกไว้
        
        BossAttack selectedAttack = attacks[randomIndex];
        isValidAttack = false;

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
    public void TakeDamage(int damage)
    {
        this.Health -= damage;
        Debug.WriteLine($"Health Boss = {Health}");
        if (this.Health <= 0)
        {
            this.IsbossAticve = false;
        }
    }

    public void Reset()
    {
        IsbossAticve = false;
        Health = MaxHealth;
        currentState = BossState.Idle;
        attackTimer = 0f;
        currentAttack = null;

        // 1. รีเซ็ตสถานะการโจมตีทั้งหมด
        foreach (var attack in attacks)
        {
            attack.Reset();

            // 2. ตรวจสอบว่าเป็นการโจมตีแบบ SpawnAttack หรือไม่
            if (attack is SpawnAttack spawnAttack)
            {
                // 3. เรียกเมธอดเพื่อรีเซ็ต/ล้างศัตรูที่ถูกเสกออกมา
                spawnAttack.ClearSpawnedEnemies();
            }
        }
        // ส่วนเดิมที่วนลูปผ่าน 'Enemy' จะถูกลบออกไป เพราะตัวแปรนี้ไม่มีอยู่จริง
        // และการจัดการศัตรูถูกย้ายไปทำใน SpawnAttack แล้ว
    }
}