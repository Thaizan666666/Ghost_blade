using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _321_Lab05_3;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace Ghost_blade
{
    internal class GameOverScreen
    {
        private AnimatedTexture background;
        private Rectangle playButton;
        private Rectangle exitButton;
        private Texture2D buttonTexture;

        public bool StartGame { get; set; }
        public bool Menu { get; set; }
        public GameOverScreen(GraphicsDevice graphicsDevice, Microsoft.Xna.Framework.Content.ContentManager content)
        {

            background = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
            background.Load(content, "gameover-Sheet", 4, 3, 8);

            buttonTexture = new Texture2D(graphicsDevice, 1, 1);
            buttonTexture.SetData(new[] { Color.White });
            playButton = new Rectangle(1390, 880, 410, 140);
            exitButton = new Rectangle(170, 880, 350, 140);
        }
        public void Update(GameTime gameTime)
        {
            background.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);

            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (playButton.Contains(mouse.Position))
                {
                    StartGame = true;
                }
                if (exitButton.Contains(mouse.Position))
                {
                    Menu = true;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            background.DrawFrame(spriteBatch, Vector2.Zero);

        }

    }
}