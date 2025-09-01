using System;
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

        private Player _player;
        private List<Bullet> _bullets;
        private Texture2D _bulletTexture;
        private FollowsCamera camera;

        private List<Room> rooms;
        private int currentRoomIndex;
        private Random random;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            camera = new FollowsCamera(Vector2.Zero);
        }

        protected override void Initialize()
        {
            _bullets = new List<Bullet>();
            rooms = new List<Room>();
            random = new Random();
            currentRoomIndex = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bulletTexture = Content.Load<Texture2D>("A_job");
            Texture2D playerTexture = Content.Load<Texture2D>("firefoxBall");
            _player = new Player(playerTexture, _bulletTexture, new Vector2(960, 540));

            // Door texture
            Texture2D doorTexture = new Texture2D(GraphicsDevice, 1, 1);
            doorTexture.SetData(new[] { Color.Red });

            // Backgrounds
            Texture2D room1BG = Content.Load<Texture2D>("room_01");
            Texture2D room2BG = Content.Load<Texture2D>("room_02");
            Texture2D room3BG = Content.Load<Texture2D>("room_03");

            rooms = new List<Room>
            {
                new Room1(room1BG, doorTexture),
                new Room2(room2BG, doorTexture),
                new Room3(room3BG, doorTexture)
            };

            _player.SetPosition(rooms[currentRoomIndex].StartPosition);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Room currentRoom = rooms[currentRoomIndex];

            camera.Follow(_player.drect, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
            _player.Update(gameTime, camera.position);

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed)
            {
                Vector2 mouseWorld = new Vector2(mouseState.X, mouseState.Y) - camera.position;
                Bullet newBullet = _player.Shoot(mouseWorld);
                if (newBullet != null) _bullets.Add(newBullet);
            }

            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                _bullets[i].Update(gameTime);
                if (!_bullets[i].IsActive) _bullets.RemoveAt(i);
            }

            _player.ClampPosition(currentRoom.Bounds, currentRoom.Obstacles);

            if (_player.drect.Intersects(currentRoom.Door) && currentRoom.NextRooms.Count > 0)
            {
                int next = currentRoom.NextRooms[random.Next(currentRoom.NextRooms.Count)];
                currentRoomIndex = next;
                _player.SetPosition(rooms[currentRoomIndex].StartPosition);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var transform = Matrix.CreateTranslation(camera.position.X, camera.position.Y, 0);

            _spriteBatch.Begin(transformMatrix: transform);
            rooms[currentRoomIndex].Draw(_spriteBatch);

            _player.Draw(_spriteBatch);
            foreach (var b in _bullets) b.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

