using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ghost_blade
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Player _player;
        private List<Bullet> _bullets;
        private Texture2D _bulletTexture;
        private FollowsCamera camera;
        private Texture2D BG1;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
            camera = new(Vector2.Zero);

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _bullets = new List<Bullet>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bulletTexture = Content.Load<Texture2D>("A_job");
            Texture2D playerTexture = Content.Load<Texture2D>("firefoxBall");
            Vector2 initialPlayerPosition = new Vector2(
                (GraphicsDevice.Viewport.Width / 2) - (playerTexture.Width / 2),
                (GraphicsDevice.Viewport.Height / 2) - (playerTexture.Height / 2)
            );
            _player = new Player(playerTexture, _bulletTexture, initialPlayerPosition);
            BG1 = Content.Load<Texture2D>("room_01");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            camera.Follow(_player.drect, new Vector2(_graphics.PreferredBackBufferWidth,_graphics.PreferredBackBufferHeight));
            _player.Update(gameTime);
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed)
            {
                Bullet newBullet = _player.Shoot(new Vector2(mouseState.X, mouseState.Y));
                if (newBullet != null)
                {
                    _bullets.Add(newBullet);
                }
            }
            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update(gameTime);
                if (!_bullets[i].IsActive)
                {
                    _bullets.RemoveAt(i);
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var transformMatrix = Matrix.CreateTranslation(camera.position.X, camera.position.Y, 0);
            _spriteBatch.Begin(transformMatrix: transformMatrix);
            _spriteBatch.Draw(BG1, new Rectangle(0, 0, 600, 800), Color.White);
            _player.Draw(_spriteBatch);
            foreach (Bullet bullet in _bullets)
            {
                bullet.Draw(_spriteBatch);
            }
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
