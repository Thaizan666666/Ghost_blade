using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Texture2D _swordTexture;

        private Texture2D _bossTexture;
        private Boss boss;
        private Texture2D _pixel;


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
            Texture2D playerTexture = Content.Load<Texture2D>("GB_Idle-Sheet");
            Texture2D EnemyTexture = Content.Load<Texture2D>("firefoxBall");
            _swordTexture = new Texture2D(GraphicsDevice, 50, 20); // Create a 50x20 pixel texture
            Color[] data = new Color[50 * 20];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White; // Fill with red color
            _swordTexture.SetData(data); // Apply the color data
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _player = new Player(playerTexture, _bulletTexture, _swordTexture, new Vector2(960, 540));
            _enemy = new Enemy(EnemyTexture, new Vector2(50, 50), 1.0f, 500f);
            _enemyShooting = new Enemy_Shooting(EnemyTexture, new Vector2(50, 200), 1.5f, 500f, _bulletTexture);

            // สมัครรับ Event OnShoot ของศัตรู เพื่อเพิ่มกระสุนที่ยิงใหม่เข้าสู่ List หลัก
            _enemyShooting.OnShoot += bullet => _enemyBullets.Add((EnemyBullet)bullet);

            _bossTexture = new Texture2D(GraphicsDevice, 1, 1);
            _bossTexture.SetData(new[] { Color.White });

            // Pass the pixel texture to the Beholster constructor
            boss = new Boss(_bossTexture);


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
            // Check if the sword is swinging and if it collides with an active enemy.
            if (_player.MeleeAttackRectangle != Rectangle.Empty)
            {
                if (_enemy.IsActive && _player.MeleeAttackRectangle.Intersects(_enemy.boundingBox))
                {
                    if (_player._isSlash)
                    {
                        Random random = new Random();
                        int r = random.Next(50, 70);
                        _enemy.TakeDamage(r);
                        Debug.WriteLine($"Enemy hit by sword! {r}");
                        _player._isSlash = false;
                    }
                }

                if (_enemyShooting.IsActive && _player.MeleeAttackRectangle.Intersects(_enemyShooting.boundingBox))
                {
                    if (_player._isSlash)
                    {
                        Random random = new Random();
                        int r = random.Next(50, 70);
                        _enemyShooting.TakeDamage(r);
                        Debug.WriteLine($"Shooting enemy hit by sword! {r}");
                        _player._isSlash = false;
                    }
                }
            }

            // Update and check for bullet collisions
            for (int i = _playerBullets.Count - 1; i >= 0; i--)
            {
                Bullet bullet = _playerBullets[i];
                bullet.Update(gameTime, currentRoom.Obstacles);

                // Check for player bullet hitting enemies
                if (_enemy.IsActive && bullet.boundingBox.Intersects(_enemy.boundingBox))
                {
                    _enemy.TakeDamage(40);
                    bullet.IsActive = false;
                }

                if (_enemyShooting.IsActive && bullet.boundingBox.Intersects(_enemyShooting.boundingBox))
                {
                    _enemyShooting.TakeDamage(40);
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
            boss.Update(gameTime,_player);

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

            _enemy.Draw(_spriteBatch);

            _enemyShooting.Draw(_spriteBatch);

            foreach (var bullet in _playerBullets)
            {
                bullet.Draw(_spriteBatch);
            }

            // Draw enemy bullets
            foreach (var bullet in _enemyBullets)
            {
                bullet.Draw(_spriteBatch);
            }
            boss.Draw(_spriteBatch);
            DrawRectangle(_spriteBatch, _player.drect, Color.Red, 1);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, float thickness)
        {
            // Draw the filled rectangle
            spriteBatch.Draw(_pixel, rectangle, color);
        }
    }
}
