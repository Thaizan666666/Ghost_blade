using Ghost_blade;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class SpawnAttack : BossAttack
{
    private Texture2D enemyTexture1;
    private Texture2D enemyTexture2;
    private Texture2D bulletTexture;
    private Random random;

    private List<Enemy> spawnedEnemies;
    private bool hasSpawned = false;

    private float spawnTimer = 0f;
    private float spawnInterval = 1.0f;
    private int maxEnemiesToSpawn = 5;
    private int enemiesSpawnedCount = 0;

    // A temporary list to hold enemies just spawned in this frame
    private List<Enemy> newlySpawnedEnemies;

    public SpawnAttack(Boss owner, Texture2D pixelTexture, Texture2D enemyTex1, Texture2D enemyTex2, Texture2D bulletTexture)
        : base(owner, pixelTexture)
    {
        this.enemyTexture1 = enemyTex1;
        this.enemyTexture2 = enemyTex2;
        this.bulletTexture = bulletTexture;
        this.random = new Random();
        this.spawnedEnemies = new List<Enemy>();
        this.newlySpawnedEnemies = new List<Enemy>();
    }

    public override void Start(Player player)
    {
        IsFinished = false;
        hasSpawned = false;
        enemiesSpawnedCount = 0;
        spawnedEnemies.Clear();
        newlySpawnedEnemies.Clear();
    }

    public override void Update(GameTime gameTime, Player player, List<Rectangle> obstacles)
    {
        if (!hasSpawned)
        {
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (spawnTimer >= spawnInterval && enemiesSpawnedCount < maxEnemiesToSpawn)
            {
                Vector2 spawnPosition = boss.Position + new Vector2(random.Next(-100, 100), random.Next(-100, 100));

                Enemy newEnemy;
                if (random.Next(2) == 0)
                {
                    newEnemy = new Enemy_Melee(enemyTexture1, spawnPosition, 1.5f, 1000f);
                }
                else
                {
                    // โค้ดนี้ถูกต้องอยู่แล้ว
                    newEnemy = new Enemy_Shooting(enemyTexture2, spawnPosition, 1.0f, 1000f, bulletTexture);
                }

                newlySpawnedEnemies.Add(newEnemy);
                spawnedEnemies.Add(newEnemy);

                enemiesSpawnedCount++;
                spawnTimer = 0f;
            }

            if (enemiesSpawnedCount >= maxEnemiesToSpawn)
            {
                hasSpawned = true;
            }
        }

        // Update and remove dead enemies from our local list
        spawnedEnemies.RemoveAll(e => !e.IsActive);

        // The attack is only finished when all spawned enemies are defeated
        if (hasSpawned && spawnedEnemies.Count == 0)
        {
            IsFinished = true;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // We only draw the enemies if they are still active
        foreach (var enemy in spawnedEnemies)
        {
            enemy.Draw(spriteBatch);
        }
    }

    // This method is called by the Boss class to get the newly spawned enemies
    public List<Enemy> GetNewEnemies()
    {
        var list = new List<Enemy>(newlySpawnedEnemies);
        newlySpawnedEnemies.Clear(); // Clear the list after giving it to the boss
        return list;
    }
}