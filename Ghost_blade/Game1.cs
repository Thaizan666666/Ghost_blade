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
        private List<Bullet> _playerBullets;
        private List<EnemyBullet> _enemyBullets;
        private Texture2D _bulletTexture;
        private FollowsCamera camera;

        private List<Room> rooms;
        private int currentRoomIndex;
        private Random random;
        private Enemy _enemy;
        private Enemy_Shooting _enemyShooting;

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
            _playerBullets = new List<Bullet>();
            _enemyBullets = new List<EnemyBullet>();
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
            Texture2D EnemyTexture = Content.Load<Texture2D>("firefoxBall");

            _player = new Player(playerTexture, _bulletTexture, new Vector2(960, 540));
            _enemy = new Enemy(EnemyTexture, new Vector2(50, 50), 1.0f, 500f);
            _enemyShooting = new Enemy_Shooting(EnemyTexture, new Vector2(50, 200), 1.5f, 500f, _bulletTexture);

            // สมัครรับ Event OnShoot ของศัตรู เพื่อเพิ่มกระสุนที่ยิงใหม่เข้าสู่ List หลัก
            _enemyShooting.OnShoot += bullet => _enemyBullets.Add((EnemyBullet)bullet);


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

            if (_player.IsAlive)
            {
                _player.Update(gameTime, camera.position);
            }
            else {return;}

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed || mouseState.RightButton == ButtonState.Pressed)
            {
                Vector2 mouseWorld = new Vector2(mouseState.X, mouseState.Y) - camera.position;
                Bullet newBullet = _player.Shoot(mouseWorld);
                if (newBullet != null) _playerBullets.Add(newBullet);
            }

            // Update and check for bullet collisions
            for (int i = _playerBullets.Count - 1; i >= 0; i--)
            {
                Bullet bullet = _playerBullets[i];
                bullet.Update(gameTime, currentRoom.Obstacles);

                // Check for player bullet hitting enemies
                if (_enemy.IsActive && bullet.boundingBox.Intersects(_enemy.boundingBox))
                {
                    _enemy.IsActive = false;
                    bullet.IsActive = false;
                }

                if (_enemyShooting.IsActive && bullet.boundingBox.Intersects(_enemyShooting.boundingBox))
                {
                    _enemyShooting.IsActive = false;
                    bullet.IsActive = false;
                }

                // Remove inactive bullets
                if (!bullet.IsActive)
                {
                    _playerBullets.RemoveAt(i);
                }
            }

            // Enemy bullets update and collision checks
            for (int i = _enemyBullets.Count - 1; i >= 0; i--)
            {
                EnemyBullet bullet = _enemyBullets[i];
                bullet.Update(gameTime, currentRoom.Obstacles, _player);

                // No need for explicit IsActive check here, the bullet's Update handles it.
                if (!bullet.IsActive)
                {
                    _enemyBullets.RemoveAt(i);
                }
            }

            if (_player.IsAlive)
            {
                _player.ClampPosition(currentRoom.Bounds, currentRoom.Obstacles);
                if (_player.drect.Intersects(currentRoom.Door) && currentRoom.NextRooms.Count > 0)
                {
                    int next = currentRoom.NextRooms[random.Next(currentRoom.NextRooms.Count)];
                    currentRoomIndex = next;
                    _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                }
            }

            if (_enemy.IsActive)
            {
                _enemy.Update(_player, currentRoom.Obstacles);
            }

            if (_enemyShooting.IsActive)
            {
                _enemyShooting.Update(_player, currentRoom.Obstacles);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var transform = Matrix.CreateTranslation(camera.position.X, camera.position.Y, 0);

            _spriteBatch.Begin(transformMatrix: transform);

            // Draw the current room
            rooms[currentRoomIndex].Draw(_spriteBatch);

            // Draw the player
            _player.Draw(_spriteBatch);

            // Draw active enemies
            if (_enemy.IsActive)
            {
                _enemy.Draw(_spriteBatch);
            }
            if (_enemyShooting.IsActive)
            {
                _enemyShooting.Draw(_spriteBatch);
            }

            // Draw player bullets
            foreach (var bullet in _playerBullets)
            {
                bullet.Draw(_spriteBatch);
            }

            // Draw enemy bullets
            foreach (var bullet in _enemyBullets)
            {
                bullet.Draw(_spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
