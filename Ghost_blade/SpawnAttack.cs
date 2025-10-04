using _321_Lab05_3;
using Ghost_blade;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

public class SpawnAttack : BossAttack
{
    private Texture2D enemyTexture1;
    private Texture2D enemyTexture2;
    private Texture2D bulletTexture;
    private Texture2D parryTexture;
    private AnimatedTexture Enemymelee_Idle;
    private AnimatedTexture Enemymelee_Walk;
    private AnimatedTexture Enemymelee_Attack;
    private AnimatedTexture EnemyShooting_Idle;
    private AnimatedTexture EnemyShooting_Walk;

    private Random random;

    private List<Enemy> spawnedEnemies;
    private bool hasFinishedInitialSpawns = false; // Renamed for clarity
    public Vector2 position;

    // --- New/Modified Fields for Spawning Warning ---
    private const float SpawnInterval = 1.0f;
    private const float WarningDuration = 0.2f; // The 0.2 second delay you requested
    private const float FullCycleTime = SpawnInterval + WarningDuration; // Total time for one cycle

    private float timer = 0f; // Single timer for the full cycle
    private int maxEnemiesToSpawn = 5;
    private int enemiesSpawnedCount = 0;

    // Stores the position and time of the next enemy to spawn
    private struct PendingSpawn
    {
        public Vector2 SpawnPosition;
        public float WarningEndTime;
        public int EnemyType; // 0 for Melee, 1 for Shooting
    }

    private PendingSpawn? nextPendingSpawn = null;

    // A temporary list to hold enemies just spawned in this frame
    private List<Enemy> newlySpawnedEnemies;
    private Texture2D pixelTexture; // Keep a reference to the pixelTexture for drawing the warning

    public SpawnAttack(Boss owner, Texture2D pixelTexture, AnimatedTexture Enemymelee_Idle, AnimatedTexture Enemymelee_Walk, AnimatedTexture Enemymelee_Attack,
        AnimatedTexture EnemyShooting_Idle, AnimatedTexture EnemyShooting_Walk,
        Texture2D enemyTex1, Texture2D enemyTex2, Texture2D bulletTexture, Texture2D parryTexture)
        : base(owner, pixelTexture)
    {
        this.pixelTexture = pixelTexture; // Store the pixel texture
        this.enemyTexture1 = enemyTex1;
        this.enemyTexture2 = enemyTex2;
        this.Enemymelee_Idle = Enemymelee_Idle;
        this.Enemymelee_Attack = Enemymelee_Attack;
        this.Enemymelee_Walk = Enemymelee_Walk;
        this.EnemyShooting_Idle = EnemyShooting_Idle;
        this.EnemyShooting_Walk = EnemyShooting_Walk;
        this.bulletTexture = bulletTexture;
        this.random = new Random();
        this.spawnedEnemies = new List<Enemy>();
        this.newlySpawnedEnemies = new List<Enemy>();
        this.parryTexture = parryTexture;
    }

    public override void Start(Player player)
    {
        IsFinished = false;
        hasFinishedInitialSpawns = false;
        enemiesSpawnedCount = 0;
        timer = 0f;
        nextPendingSpawn = null;
        spawnedEnemies.Clear();
        newlySpawnedEnemies.Clear();
        // Assuming position is the general area for spawning
        position = boss.Position + new Vector2(0, boss.Boss_height);
        // Start the first spawn immediately by preparing the pending spawn
        PrepareNextSpawn();
    }

    // --- Helper Method to Prepare Next Spawn ---
    private void PrepareNextSpawn()
    {
        if (enemiesSpawnedCount < maxEnemiesToSpawn)
        {
            Vector2 spawnPosition = position + new Vector2(random.Next(-100, 100), random.Next(0, 500));
            int enemyType = random.Next(2); // 0 or 1

            nextPendingSpawn = new PendingSpawn
            {
                SpawnPosition = spawnPosition,
                WarningEndTime = timer + WarningDuration,
                EnemyType = enemyType
            };
        }
        else
        {
            nextPendingSpawn = null;
            hasFinishedInitialSpawns = true;
        }
    }

    // --- Helper Method to Actually Spawn the Enemy ---
    private void ExecuteSpawn(PendingSpawn spawnData)
    {
        Enemy newEnemy;
        Vector2 spawnPosition = spawnData.SpawnPosition;

        if (spawnData.EnemyType == 0)
        {
            newEnemy = new Enemy_Melee(Enemymelee_Idle,Enemymelee_Walk,Enemymelee_Attack, enemyTexture1, spawnPosition, 1.5f, 1000f);
        }
        else
        {
            newEnemy = new Enemy_Shooting(EnemyShooting_Idle, EnemyShooting_Walk, enemyTexture2, spawnPosition, 1.0f, 1000f, bulletTexture,parryTexture);
        }

        newlySpawnedEnemies.Add(newEnemy);
        spawnedEnemies.Add(newEnemy);

        enemiesSpawnedCount++;

        // Prepare the next spawn for the next cycle
        timer = 0f;
        PrepareNextSpawn();
    }


    public override void Update(GameTime gameTime, Player player, List<Rectangle> obstacles)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        timer += deltaTime;

        // 1. Check for enemy spawning
        if (nextPendingSpawn.HasValue)
        {
            // If the warning time is over, spawn the enemy
            if (timer >= nextPendingSpawn.Value.WarningEndTime)
            {
                ExecuteSpawn(nextPendingSpawn.Value);
            }
        }

        // 2. Update and remove dead enemies from our local list
        spawnedEnemies.RemoveAll(e => !e.IsActive);

        if (hasFinishedInitialSpawns)
        {
            IsFinished = true;
        }
    }

    // --- New Helper Method to Draw a Solid Rectangle ---
    // (You likely have a similar method in your base class or a utility class)
    private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        // This assumes pixelTexture is a 1x1 white texture
        spriteBatch.Draw(pixelTexture, rect, color);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // 1. Draw the flashing warning marker
        if (nextPendingSpawn.HasValue && timer < nextPendingSpawn.Value.WarningEndTime)
        {
            // Calculate the warning rect size (e.g., 32x32)
            int warningSize = 32;
            Vector2 center = nextPendingSpawn.Value.SpawnPosition;
            Rectangle warningRect = new Rectangle(
                (int)(center.X - warningSize / 2),
                (int)(center.Y - warningSize / 2),
                warningSize,
                warningSize);

            // Create a flashing effect by checking the time
            // The warning will flash for 0.2 seconds. This makes it flash 10 times per second (10Hz).
            if (((int)(timer * 10)) % 2 == 0)
            {
                DrawRectangle(spriteBatch, warningRect, Color.Red); // Draw a solid red square
            }
        }

        // 2. Draw the active enemies
        foreach (var enemy in spawnedEnemies)
        {
            enemy.Draw(spriteBatch);
        }
    }

    // This method is called by the Boss class to get the newly spawned enemies
    public List<Enemy> GetNewEnemies()
    {
        var list = newlySpawnedEnemies.ToList();
        newlySpawnedEnemies.Clear(); // Clear the list after giving it to the boss
        return list;
    }
    public int GetActiveSpawnedEnemyCount()
    {
        return spawnedEnemies.Count;
    }
}