using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using _321_Lab05_3;
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
        private int stageStep;
        private Random random;

        private Texture2D _swordTexture;

        private Texture2D _bossTexture;
        private Boss boss;
        private Texture2D _pixel;
        private Texture2D EnemyTexture;

        private GameState gameState = GameState.MainMenu;
        private MainMenuScreen mainMenu;
        private GameOverScreen gameOver;

        private AnimatedTexture Hp_bar;
        private AnimatedTexture cursorTexture;
        private AnimatedTexture cursorReloadTexture;
        SpriteFont uiFont;

        public const float SCALE = 2f;

        public enum GameState
        {
            MainMenu,
            Playing,
            GameOver
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            camera = new FollowsCamera(Vector2.Zero);
            Hp_bar = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
            cursorTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            cursorReloadTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
        }
        
        protected override void Initialize()
        {
            _playerBullets = new List<Bullet>();
            _enemyBullets = new List<EnemyBullet>();
            rooms = new List<Room>();
            random = new Random();
            currentRoomIndex = 0;
            stageStep = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bulletTexture = Content.Load<Texture2D>("A_job");
            Texture2D playerTexture = Content.Load<Texture2D>("GB_Idle-Sheet");
            EnemyTexture = Content.Load<Texture2D>("firefoxBall");
            _swordTexture = new Texture2D(GraphicsDevice, 50, 20); // Create a 50x20 pixel texture
            Color[] data = new Color[50 * 20];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White; // Fill with red color
            _swordTexture.SetData(data); // Apply the color data
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _player = new Player(playerTexture, _bulletTexture, _swordTexture, new Vector2(960*2, 540*2));
            // Removed direct Enemy and Enemy_Shooting creation.

            _bossTexture = new Texture2D(GraphicsDevice, 1, 1);
            _bossTexture.SetData(new[] { Color.White });

            // Pass the pixel texture to the Beholster constructor
            boss = new Boss(_bossTexture,new Vector2(55 * 48, 98 * 48), _pixel, EnemyTexture, EnemyTexture, _bulletTexture);


            // Door texture
            Texture2D doorTexture = new Texture2D(GraphicsDevice, 1, 1);
            doorTexture.SetData(new[] { Color.Red });

            // Backgrounds
            Texture2D Map_tutorial_01 = Content.Load<Texture2D>("Map_tutorial_01");
            Texture2D Map_city_01 = Content.Load<Texture2D>("Map_city_01");
            Texture2D Map_city_02 = Content.Load<Texture2D>("Map_city_02");
            Texture2D Map_city_03 = Content.Load<Texture2D>("Map_city_03");
            Texture2D Map_lab_01 = Content.Load<Texture2D>("Map_lab_01");
            Texture2D Map_lab_02 = Content.Load<Texture2D>("Map_lab_02");
            Texture2D Map_lab_03 = Content.Load<Texture2D>("Map_lab_03");
            Texture2D Map_Boss_01 = Content.Load<Texture2D>("Map_Boss_01");

            // Now, pass the textures to the Room constructors
            rooms = new List<Room>
            {
                new MapTutorial01(Map_tutorial_01, doorTexture, EnemyTexture, _bulletTexture),
                new MapCity01(Map_city_01, doorTexture, EnemyTexture, _bulletTexture),
                new MapCity02(Map_city_02, doorTexture, EnemyTexture, _bulletTexture),
                new MapCity03(Map_city_03, doorTexture, EnemyTexture, _bulletTexture),
                new MapLab01(Map_lab_01, doorTexture, EnemyTexture, _bulletTexture),
                new MapLab02(Map_lab_02, doorTexture, EnemyTexture, _bulletTexture),
                new MapLab03(Map_lab_03, doorTexture, EnemyTexture, _bulletTexture),
                new MapBoss01(Map_Boss_01, doorTexture, EnemyTexture, _bulletTexture),
            };

            // Set up a reference to the shooting enemy's OnShoot event.
            // This is still a bit clunky, but necessary for now.
            foreach (var room in rooms)
            {
                foreach (var enemy in room.Enemies)
                {
                    // Check if the enemy is an Enemy_Shooting instance
                    if (enemy is Enemy_Shooting shootingEnemy)
                    {
                        // Subscribe to the OnShoot event
                        shootingEnemy.OnShoot += bullet => _enemyBullets.Add((EnemyBullet)bullet);
                    }
                }
            }

            _player.SetPosition(rooms[currentRoomIndex].StartPosition);
            mainMenu = new MainMenuScreen(GraphicsDevice, Content);

            gameOver = new GameOverScreen(GraphicsDevice, Content);

            Hp_bar.Load(Content, "HP-Sheet", 6, 1, 8);
            cursorTexture.Load(Content, "crosshairs-Sheet", 4, 1, 8);
            cursorReloadTexture.Load(Content, "crosshairs_reload-Sheet", 4, 1, 8);
            _player.change_Weapon = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
            _player.change_Weapon.Load(Content, "UI_weapon-Sheet", 4, 1, 1);
            _player.GunDashingTexture.Load(Content, "GB_Dash-Sheet", 4, 1, 20);
            _player.BladeDashingTexture.Load(Content, "GB_Dash-Blade_Sheet", 4, 1, 20);
            _player.IdleTexture.Load(Content, "GB_Idle-Sheet", 4, 1, 8);
            _player.RunningTexture.Load(Content, "GB_Run_Blade-Sheet", 8, 1, 8);
            _player.AttackingTexture.Load(Content, "GB_Slash2-Sheet", 4, 1, 20);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            uiFont = Content.Load<SpriteFont>("UI_Font");

        }

        protected override void Update(GameTime gameTime)
        {
            Debug.WriteLine($"Player Position: X={_player.position.X/24}, Y={_player.position.Y/24}");
            Room currentRoom = rooms[currentRoomIndex];
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if (gameState == GameState.Playing)
                {
                    gameState = GameState.MainMenu;
                    // เริ่มเกมตั้งค่า player position, reset enemies
                    _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                    _player.Reset();
                    currentRoomIndex = 0;
                    stageStep = 0;
                    currentRoom.ResetRoom();
                    _enemyBullets.Clear();
                    _playerBullets.Clear();
                    mainMenu.StartGame = false;
                    mainMenu.ExitGame = false;
                }
                return;
            }


            if (gameState == GameState.MainMenu)
            {
                mainMenu.Update(gameTime);

                if (mainMenu.StartGame)
                {
                    gameState = GameState.Playing;
                    // เริ่มเกมตั้งค่า player position, reset enemies
                    mainMenu.StartGame = false;
                    _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                }
                if (mainMenu.ExitGame)
                {
                    Exit();
                }
                return;
            }
            if (gameState == GameState.Playing)
            {
                if (_player.Health <= 0)
                {
                    gameState = GameState.GameOver;

                    _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                    _player.Reset();
                    currentRoomIndex = 0;
                    stageStep = 0;
                    currentRoom.ResetRoom();
                    _enemyBullets.Clear();
                    _playerBullets.Clear();
                    gameOver.StartGame = false;
                    gameOver.Menu = false;
                    return;
                }
            }
            
            if (gameState == GameState.GameOver)
            {
                gameOver.Update(gameTime);

                if (gameOver.StartGame)
                {
                    gameState = GameState.Playing;
                    _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                    gameOver.StartGame = false;
                    gameOver.Menu = false;
                }
                if (gameOver.Menu)
                {
                    gameState = GameState.MainMenu;
                    gameOver.Menu = false;
                    gameOver.StartGame = false;
                }
                return;
            }

            camera.Follow(_player.drect, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));

            if (_player.IsAlive)
            {
                _player.Update(gameTime, camera.position);
            }
            else { return; }

            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mouseWorld = new Vector2(mouseState.X, mouseState.Y) - camera.position;
                Bullet newBullet = _player.Shoot(mouseWorld);
                if (newBullet != null) _playerBullets.Add(newBullet);
            }

            // Update all enemies in the current room
            foreach (var enemy in currentRoom.Enemies)
            {
                if (enemy.IsActive)
                {
                    enemy.Update(_player, currentRoom.Obstacles,gameTime);
                    enemy.ClampPosition(currentRoom.Bounds, currentRoom.Obstacles);
                }
            }

            // Check for player sword collisions with all enemies
            if (_player.meleeWeapon.AttackHitbox != Rectangle.Empty)
            {
                foreach (var enemy in currentRoom.Enemies)
                {
                    if (enemy.IsActive && _player.meleeWeapon.AttackHitbox.Intersects(enemy.boundingBox))
                    {
                        if (_player._isSlash)
                        {
                            enemy.TakeDamage(70, _player.position);
                            _player._isSlash = false;
                            break; // Exit the loop after hitting the first enemy
                        }
                    }
                }
            }

            // Update and check for player bullet collisions with all enemies
            for (int i = _playerBullets.Count - 1; i >= 0; i--)
            {
                Bullet bullet = _playerBullets[i];
                bullet.Update(gameTime, currentRoom.Obstacles);

                foreach (var enemy in currentRoom.Enemies)
                {
                    if (enemy.IsActive && bullet.boundingBox.Intersects(enemy.boundingBox))
                    {
                        enemy.TakeDamage(40,_player.position);
                        bullet.IsActive = false;
                        break; // Exit the inner loop after hitting an enemy
                    }
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
                    stageStep++;
                    switch (stageStep)
                    {
                        case 1:
                            currentRoomIndex = random.Next(1, 4); // 1,2,3
                            break;
                        case 2:
                            currentRoomIndex = currentRoom.NextRooms[random.Next(currentRoom.NextRooms.Count)];
                            break;
                        case 3:
                            currentRoomIndex = random.Next(4, 7); // 4,5,6
                            break;
                        case 4:
                            currentRoomIndex = currentRoom.NextRooms[random.Next(currentRoom.NextRooms.Count)];
                            break;
                        case 5:
                            currentRoomIndex = 7; // index boss room
                            break;
                        default:
                            gameState = GameState.MainMenu;
                            currentRoomIndex = 0;
                            stageStep = 0;
                            _player.Reset();
                            break;
                    }
                    rooms[currentRoomIndex].ResetRoom();
                    _playerBullets.Clear();
                    _enemyBullets.Clear();
                    _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                }
            }
            boss.Update(gameTime, _player,currentRoom.Obstacles);
            // *** เพิ่มโค้ดส่วนนี้เข้าไปเพื่อรับศัตรูที่บอสสร้างขึ้นมา ***
            var newlySpawnedEnemies = boss.GetSpawnedEnemies();
            if (newlySpawnedEnemies.Count > 0)
            {
                currentRoom.Enemies.AddRange(newlySpawnedEnemies);
            }

            // ... โค้ดสำหรับอัปเดตและเช็กการชนของศัตรูใน currentRoom.Enemies ...
            foreach (var enemy in currentRoom.Enemies)
            {
                if (enemy.IsActive)
                {
                    enemy.Update(_player, currentRoom.Obstacles, gameTime);
                    enemy.ClampPosition(currentRoom.Bounds, currentRoom.Obstacles);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (gameState == GameState.MainMenu)
            {
                // สำหรับเมนู ไม่ต้องใช้ camera
                _spriteBatch.Begin();
                mainMenu.Draw(_spriteBatch);
                _spriteBatch.End();
            }
            else if (gameState == GameState.GameOver)
            {
                _spriteBatch.Begin();
                gameOver.Draw(_spriteBatch);
                _spriteBatch.End();
            }
            else if (gameState == GameState.Playing)
            {
                var transform = Matrix.CreateTranslation(camera.position.X, camera.position.Y, 0);

                _spriteBatch.Begin(transformMatrix: transform);

                // Draw the current room
                rooms[currentRoomIndex].Draw(_spriteBatch);

                // Draw the player
                _player.Draw(_spriteBatch);

                // Draw all active enemies in the current room
                foreach (var enemy in rooms[currentRoomIndex].Enemies)
                {
                    enemy.Draw(_spriteBatch);
                }

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

                // Draw hitboxes
                //DrawRectangle(_spriteBatch, _player.drect, Color.Red, 1);
                DrawRectangle(_spriteBatch, _player.drect, Color.Blue, 1);
                DrawRectangle(_spriteBatch, _player.meleeWeapon.AttackHitbox, Color.Red, 1);
                foreach (var enemy in rooms[currentRoomIndex].Enemies)
                {
                    if (enemy.IsActive)
                    {
                        // Draw the enemy's main bounding box
                        DrawRectangle(_spriteBatch, enemy.boundingBox, Color.Orange, 1);

                        // Check if this specific enemy is a Melee type
                        if (enemy is Enemy_Melee meleeEnemy)
                        {
                            // Draw the melee attack hitbox only if it's currently active
                            if (meleeEnemy.AttackHitbox != Rectangle.Empty)
                            {
                                DrawRectangle(_spriteBatch, meleeEnemy.AttackHitbox, Color.Red, 1);
                            }
                        }
                    }
                }
                _spriteBatch.End();

                _spriteBatch.Begin();
                switch (_player.Health)
                {
                    case 5:
                        {
                            Hp_bar.DrawFrame(_spriteBatch, 0, new Vector2(0, 0));
                            break;
                        }
                    case 4:
                        {
                            Hp_bar.DrawFrame(_spriteBatch, 1, new Vector2(0, 0));
                            break;
                        }
                    case 3:
                        {
                            Hp_bar.DrawFrame(_spriteBatch, 2, new Vector2(0, 0));
                            break;
                        }
                    case 2:
                        {
                            Hp_bar.DrawFrame(_spriteBatch, 3, new Vector2(0, 0));
                            break;
                        }
                    case 1:
                        {
                            Hp_bar.DrawFrame(_spriteBatch, 4, new Vector2(0, 0));
                            break;
                        }
                    case 0:
                        {
                            Hp_bar.DrawFrame(_spriteBatch, 5, new Vector2(0, 0));
                            break;
                        }
                }
                string ammoText = $"{_player.currentAmmo}/10";
                _player.change_Weapon.DrawFrame(_spriteBatch, _player.currentWeaponFrame, new Vector2(1562, 885));
                _spriteBatch.DrawString(uiFont, ammoText, new Vector2(1770, 840), Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                _spriteBatch.End();
            }
            MouseState mouseState = Mouse.GetState();
            Vector2 cursorPosition = new Vector2(mouseState.X, mouseState.Y);
            _spriteBatch.Begin();
            if (_player.isReloading)
            {
                cursorReloadTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                cursorReloadTexture.DrawFrame(_spriteBatch, cursorPosition);
            }
            else
            {
                cursorTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                cursorTexture.DrawFrame(_spriteBatch, cursorPosition);
            }
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