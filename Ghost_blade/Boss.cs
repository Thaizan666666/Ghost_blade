using Ghost_blade;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
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
    public Boss(Texture2D pixelTexture, Texture2D bulletTexture)
    {
        Position = new Vector2(55 * 24, 98 * 24 + 50);
        timeBetweenAttacks = 0.5f;
        this.pixel = pixelTexture;
        this.bulletTexture = bulletTexture;
        this.random = new Random();

        attacks = new List<BossAttack>();

        // Pass the pixel texture for attacks that use it
        attacks.Add(new LaserAttack(this, pixelTexture));

        // **Change this line:** Pass the bulletTexture to the bullet attack
        attacks.Add(new BossBulletAttacks(this, bulletTexture));

        currentState = BossState.Idle;
        attackTimer = 0.1f;
    }

    public void Update(GameTime gameTime, Player player)
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
                    currentAttack.Update(gameTime, player);
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

        // Other update logic (e.g., movement)
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Define the boss's drawing rectangle
        Rectangle bossRect = new Rectangle(
            (int)(Position.X - 25),
            (int)(Position.Y - 25),
            50, // width
            50  // height
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
        currentState = BossState.Attacking;
        int randomIndex = random.Next(attacks.Count);
        currentAttack = attacks[randomIndex];
        Debug.WriteLine($"Attack = {randomIndex}");
        currentAttack.Start(player);
    }
}