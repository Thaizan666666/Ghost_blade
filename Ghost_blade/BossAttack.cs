using Ghost_blade;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

public abstract class BossAttack
{
    protected Boss boss;
    public bool IsFinished { get; protected set; }
    protected Texture2D pixelTexture; // Add this property

    // Update the constructor to accept the pixel texture
    public BossAttack(Boss owner, Texture2D pixelTexture)
    {
        this.boss = owner;
        this.pixelTexture = pixelTexture; // Assign it here
        IsFinished = false;
    }

    public abstract void Start(Player player);
    public abstract void Update(GameTime gameTime, Player player);
    public abstract void Draw(SpriteBatch spriteBatch);
}