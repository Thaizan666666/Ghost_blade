using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private bool _isOpenhitbox;

        private Player _player;
        private List<Bullet> _playerBullets;
        private List<EnemyBullet> _enemyBullets;
        private Texture2D _PlayerbulletTexture;
        private FollowsCamera camera;
        private Texture2D _parrybullet;

        private List<Room> rooms;
        private int currentRoomIndex;
        private int stageStep;
        private Random random;

        private Texture2D _swordTexture;

        private Texture2D _EnemybulletTexture;
        private Texture2D _bossTexture;
        private Boss boss;
        private Texture2D _pixel;
        private Texture2D EnemyTexture;
        private Texture2D ActiveEnemyTexture;
        private Texture2D AmmoDrop;
        private Texture2D HealDrop;
        private Texture2D Boss_Max_Hp;
        private Texture2D Boss_Current_Hp;
        private Texture2D Energy_Max_bar;
        private Texture2D Energy_Current_bar;
        private bool _isSlashUlt;

        private GameState gameState = GameState.MainMenu;
        private MainMenuScreen mainMenu;
        private GameOverScreen gameOver;
        private PausedScreen paused;
        private bool Door_Open = false;

        private KeyboardState previousKState;
        private KeyboardState currentKState;

        private AnimatedTexture Hp_bar;
        private AnimatedTexture cursorTexture;
        private AnimatedTexture cursorReloadTexture;
        private AnimatedTexture DoorCityOpenTexture;
        private AnimatedTexture DoorLabOpenTexture;
        private AnimatedTexture Enemymelee_Idle;
        private AnimatedTexture Enemymelee_Walk;
        private AnimatedTexture Enemymelee_Attack;
        private AnimatedTexture EnemyShooting_Idle;
        private AnimatedTexture EnemyShooting_Walk;
        private AnimatedTexture Enemymelee_Death;
        private AnimatedTexture EnemyShooting_Death;
        private AnimatedTexture EnemyLaser_Death;
        private AnimatedTexture EnemyLaser_ChargingLaser;
        private AnimatedTexture stat_tutorial_Sheet;
        private AnimatedTexture start_obivionlab_Sheet;
        private AnimatedTexture start_city_Sheet;
        private AnimatedTexture fullenergy_Sheet;
        private AnimatedTexture boss_laser;
        private AnimatedTexture boss_laser_start;
        private AnimatedTexture boss_laser_done;
        private AnimatedTexture boss_gun;
        private AnimatedTexture boss_gun_start;
        private AnimatedTexture boss_gun_done;
        private AnimatedTexture boss_death;

        private int Enemy_Count;
        SpriteFont uiFont;

        private bool isBossDead = false;
        private float timer = 0f;
        private float fadeAlpha = 0f;
        private Texture2D whiteTexture;
        float attackTimer = 0;
        public const float SCALE = 2f;

        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            justmentcut,
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
            cursorTexture = new AnimatedTexture(Vector2.Zero, 0f, 4f, 0f);
            cursorReloadTexture = new AnimatedTexture(Vector2.Zero, 0f, 4f, 0f);
            DoorCityOpenTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            DoorLabOpenTexture = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            Enemymelee_Idle = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            Enemymelee_Walk = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            Enemymelee_Attack = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            EnemyShooting_Idle = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            EnemyShooting_Walk = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            Enemymelee_Death = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            EnemyShooting_Death = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            EnemyLaser_Death = new AnimatedTexture(Vector2.Zero, 0f, 2f, 0f);
            EnemyLaser_ChargingLaser = new AnimatedTexture(Vector2.Zero,0f,2f,0f);
            stat_tutorial_Sheet = new AnimatedTexture(Vector2.Zero, 0f, 0.4f, 0f);
            start_obivionlab_Sheet = new AnimatedTexture(Vector2.Zero, 0f, 0.4f, 0f);
            start_city_Sheet = new AnimatedTexture(Vector2.Zero, 0f, 0.4f, 0f);
            fullenergy_Sheet = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
            boss_gun = new AnimatedTexture(Vector2.Zero, 0f, 4f, 0f);
            boss_gun_done = new AnimatedTexture(Vector2.Zero, 0f, 4f, 0f);
            boss_gun_start = new AnimatedTexture(Vector2.Zero, 0f, 4f, 0f);
            boss_laser = new AnimatedTexture(Vector2.Zero, 0f, 4f, 0f);
            boss_laser_done = new AnimatedTexture(Vector2.Zero, 0f, 4f, 0f);
            boss_laser_start = new AnimatedTexture(Vector2.Zero, 0f, 4f, 0f);
            boss_death = new AnimatedTexture(Vector2.Zero, 0f, 4f, 0f);
        }

        protected override void Initialize()
        {
            _playerBullets = new List<Bullet>();
            _enemyBullets = new List<EnemyBullet>();
            rooms = new List<Room>();
            random = new Random();
            currentRoomIndex = random.Next(1,4);
            stageStep = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _PlayerbulletTexture = Content.Load<Texture2D>("Player_Bullet");
            _EnemybulletTexture = Content.Load<Texture2D>("Enemy_Bullet");
            _parrybullet = Content.Load<Texture2D>("Parry_Bullet");
            AmmoDrop = Content.Load<Texture2D>("Ammo");
            HealDrop = Content.Load<Texture2D>("Heal");
            Texture2D playerTexture = Content.Load<Texture2D>("GB_Idle-Sheet");
            EnemyTexture = Content.Load<Texture2D>("IdleLaser");
            ActiveEnemyTexture = Content.Load<Texture2D>("Laser");
            Boss_Max_Hp = Content.Load<Texture2D>("hp_boss_1");
            Boss_Current_Hp = Content.Load<Texture2D>("hp_boss_2");
            Energy_Max_bar = Content.Load<Texture2D>("energy");
            Energy_Current_bar = Content.Load<Texture2D>("energy");

            _swordTexture = new Texture2D(GraphicsDevice, 50, 20); // Create a 50x20 pixel texture
            Color[] data = new Color[50 * 20];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White; // Fill with red color
            _swordTexture.SetData(data); // Apply the color data
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _player = new Player(playerTexture, _PlayerbulletTexture, _swordTexture, new Vector2(960 * 2, 540 * 2));
            // Removed direct Enemy and Enemy_Shooting creation.
            _bossTexture = new Texture2D(GraphicsDevice, 1, 1);
            _bossTexture.SetData(new[] { Color.White });


            // Door texture
            Texture2D doorRectangle = new Texture2D(GraphicsDevice, 1, 1);
            doorRectangle.SetData(new[] { Color.Red });

            // Backgrounds
            Texture2D Map_tutorial_01 = Content.Load<Texture2D>("Map_tutorial");
            Texture2D Map_city_01 = Content.Load<Texture2D>("Map_City_01");
            Texture2D Map_city_02 = Content.Load<Texture2D>("Map_City_03");
            Texture2D Map_city_03 = Content.Load<Texture2D>("Map_City_02");
            Texture2D Map_lab_01 = Content.Load<Texture2D>("Map_lab_01");
            Texture2D Map_lab_02 = Content.Load<Texture2D>("Map_lab_02");
            Texture2D Map_lab_03 = Content.Load<Texture2D>("Map_lab_03");
            Texture2D Map_Boss_01 = Content.Load<Texture2D>("Map_boss");
            Texture2D Map_tutorial_01_void = Content.Load<Texture2D>("Map_tutorial_void");
            Texture2D Map_city_01_void = Content.Load<Texture2D>("Map_City_01_void");
            Texture2D Map_city_02_void = Content.Load<Texture2D>("Map_City_02_void");
            Texture2D Map_city_03_void = Content.Load<Texture2D>("Map_City_03_void");
            Texture2D Map_lab_01_void = Content.Load<Texture2D>("Map_lab_01_void");
            Texture2D Map_lab_02_void = Content.Load<Texture2D>("Map_lab_02_void");
            Texture2D Map_lab_03_void = Content.Load<Texture2D>("Map_lab_03_void");
            Texture2D Map_Boss_void = Content.Load<Texture2D>("Map_boss-void");

            Hp_bar.Load(Content, "HP-Sheet", 6, 1, 8);
            cursorTexture.Load(Content, "crosshairs-Sheet", 4, 1, 20);
            cursorReloadTexture.Load(Content, "crosshairs_reload-Sheet", 4, 1, 20);
            _player.change_Weapon = new AnimatedTexture(Vector2.Zero, 0f, 1f, 0f);
            _player.change_Weapon.Load(Content, "UI_weapon-Sheet", 4, 1, 1);
            _player.GunDashingTexture.Load(Content, "GB_Dash-Sheet", 4, 1, 10);
            _player.BladeDashingTexture.Load(Content, "GB_Dash-Blade_Sheet", 4, 1, 10);
            _player.IdleBladeTexture.Load(Content, "GB_Idle-Blade_Sheet", 4, 1, 8);
            _player.IdleGunTexture.Load(Content, "GB_Idle-Sheet", 4, 1, 8);
            _player.RunningBladeTexture.Load(Content, "GB_Run_Blade-Sheet", 8, 1, 8);
            _player.RunningGunTexture.Load(Content, "GB_Run-Sheet", 8, 1, 8);
            _player.AttackingTexture.Load(Content, "GB_Slash2-Sheet", 4, 1, 20);
            _player.AttackingTexture2.Load(Content, "GB_Slash4-Sheet", 4, 1, 20);
            _player.AttackingTextureUp.Load(Content, "GB_SlashUp-Sheet", 4, 1, 20);
            _player.AttackingTextureDown.Load(Content, "GB_SlashDown-Sheet", 4, 1, 20);
            _player.ParryTexture.Load(Content, "GB_Parry-Sheet", 8, 1, 8);
            DoorCityOpenTexture.Load(Content, "Door_city_Sheet", 4, 1, 8);
            DoorLabOpenTexture.Load(Content, "Door_lab_Sheet", 5, 1, 8);
            Enemymelee_Idle.Load(Content, "enemy-Idle-12Frame_Sheet", 12, 1, 8);
            Enemymelee_Walk.Load(Content, "enemywalk-Sheet", 8, 1, 4);
            Enemymelee_Attack.Load(Content, "enemyattack-Sheet", 11, 1, 4);
            EnemyShooting_Idle.Load(Content, "enemyshooting-Sheet", 12, 1, 8);
            EnemyShooting_Walk.Load(Content, "enemyshooting_walk-Sheet", 8, 1, 4);
            Enemymelee_Death.Load(Content, "enemydeath-25FrameSheet", 25, 1, 12);
            EnemyShooting_Death.Load(Content, "enemyshooting_death-25FrameSheet", 25, 1, 10);
            EnemyLaser_ChargingLaser.Load(Content, "ChargingLaser-Sheet",14 ,1, 8);
            EnemyLaser_Death.Load(Content, "DeathLaser16Frame-Sheet", 16, 1, 8);
            stat_tutorial_Sheet.Load(Content, "stat_tutorial-Sheet", 3, 4, 12);
            start_obivionlab_Sheet.Load(Content, "start_obivionlab-Sheet", 3, 4, 12);
            start_city_Sheet.Load(Content, "start_city-Sheet", 3, 4, 12);
            fullenergy_Sheet.Load(Content, "energy-Sheet", 8, 1, 4);
            boss_gun.Load(Content, "boss_gun", 4, 1, 12);
            boss_gun_start.Load(Content, "boss_gun_start", 6, 3, 9);
            boss_gun_done.Load(Content, "boss_gun_done", 4, 2, 8);
            boss_laser.Load(Content, "boss_laser", 4, 1, 12);
            boss_laser_start.Load(Content, "boss_laser_start", 6, 3, 9);
            boss_laser_done.Load(Content, "boss_laser_done", 4, 2, 4);
            boss_death.Load(Content, "boss_death", 4, 4, 8);

            // Pass the pixel texture to the Beholster constructor
            boss = new Boss(_bossTexture, new Vector2(41 * 48, 8 * 48), _pixel, 
                Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, EnemyTexture, EnemyTexture, _EnemybulletTexture,_parrybullet);

            // Now, pass the textures to the Room constructors
            rooms = new List<Room>
            {
                new MapTutorial01(Map_tutorial_01, Map_tutorial_01_void, DoorCityOpenTexture, Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, EnemyTexture, _EnemybulletTexture,_parrybullet,_pixel),
                new MapCity01(Map_city_01, Map_city_01_void, DoorCityOpenTexture, Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, EnemyTexture, _EnemybulletTexture, _parrybullet,_pixel),
                new MapCity02(Map_city_02, Map_city_02_void, DoorCityOpenTexture, Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, EnemyTexture, _EnemybulletTexture, _parrybullet,_pixel),
                new MapCity03(Map_city_03, Map_city_03_void, DoorCityOpenTexture, Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, EnemyTexture, _EnemybulletTexture, _parrybullet,_pixel),
                new MapLab01(Map_lab_01, Map_lab_01_void, DoorLabOpenTexture, Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, EnemyTexture,EnemyLaser_ChargingLaser,EnemyLaser_Death,EnemyTexture, _EnemybulletTexture,_parrybullet,_pixel),
                new MapLab02(Map_lab_02, Map_lab_02_void, DoorLabOpenTexture, Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, EnemyTexture,EnemyLaser_ChargingLaser,EnemyLaser_Death,EnemyTexture, _EnemybulletTexture,_parrybullet,_pixel),
                new MapLab03(Map_lab_03, Map_lab_03_void, DoorLabOpenTexture, Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, EnemyTexture,EnemyLaser_ChargingLaser,EnemyLaser_Death,EnemyTexture, _EnemybulletTexture,_parrybullet,_pixel),
                new MapBoss01(Map_Boss_01, Map_Boss_void, DoorLabOpenTexture, Enemymelee_Idle, Enemymelee_Walk, Enemymelee_Attack, Enemymelee_Death, EnemyShooting_Idle, EnemyShooting_Walk, EnemyShooting_Death, EnemyTexture, _EnemybulletTexture,_parrybullet,_pixel),
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
            paused = new PausedScreen(GraphicsDevice, Content);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            uiFont = Content.Load<SpriteFont>("UI_Font");
            _player.Hand = Content.Load<Texture2D>("Gun"); 
            _player.HandOrigin = new Vector2(0, 7 / 2f);

            whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            whiteTexture.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            Debug.WriteLine($"ULT Charge: {_player.meleeWeapon.ultCharge:F2} / {_player.meleeWeapon.ultChargeMax} | Active: {_player.meleeWeapon._isULTActive}");

            currentKState = Keyboard.GetState();

            if (currentKState.IsKeyDown(Keys.Escape))
            {
                if (gameState == GameState.Playing)
                {
                    gameState = GameState.Paused;
                    paused.StartGame = false;
                    paused.Menu = false;
                }
                return;
            }
            if (currentKState.IsKeyDown(Keys.O) && !previousKState.IsKeyDown(Keys.O) && _isOpenhitbox == false)
            {
                _isOpenhitbox = true;
            }
            else if (currentKState.IsKeyDown(Keys.O) && !previousKState.IsKeyDown(Keys.O) && _isOpenhitbox == true)
            {
                _isOpenhitbox = false;
            }
            Debug.WriteLine($"Player Position: X={_player.position.X/48}, Y={_player.position.Y/48}");
            Room currentRoom = rooms[currentRoomIndex];
            
            if (currentKState.IsKeyDown(Keys.I))
            {
                currentRoomIndex = 7;
                _player.position = rooms[currentRoomIndex].StartPosition;
            }
            if (currentKState.IsKeyDown(Keys.U))
            {
                currentRoomIndex = 4;
                _player.position = rooms[currentRoomIndex].StartPosition;
            }
            if (currentKState.IsKeyDown(Keys.P) && !previousKState.IsKeyDown(Keys.P))
            {
                currentRoomIndex += 1;
                _player.position = rooms[currentRoomIndex].StartPosition;
            }

            if (gameState == GameState.Paused) 
            {
                paused.Update(gameTime);
                if (paused.StartGame) 
                {
                    gameState = GameState.Playing;
                    paused.StartGame = false;
                    paused.Menu = false;
                }
                if (paused.Menu)
                {
                    gameState = GameState.MainMenu;
                    _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                    _player.Reset();
                    boss.Reset();
                    currentRoomIndex = random.Next(1, 4); ;
                    stageStep = 0;
                    currentRoom.ResetRoom();
                    Door_Open = false;
                    DoorCityOpenTexture.Reset();
                    DoorLabOpenTexture.Reset();
                    _enemyBullets.Clear();
                    _playerBullets.Clear();
                    paused.StartGame = false;
                    paused.Menu = false;
                }
                return;
            }

            if (gameState == GameState.MainMenu)
            {
                mainMenu.Update(gameTime);

                if (mainMenu.StartGame)
                {
                    gameState = GameState.Playing;
                    boss.Reset();
                    mainMenu.StartGame = false;
                    _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                    start_city_Sheet.Play();
                    start_obivionlab_Sheet.Play();
                }
                if (mainMenu.tutorial)
                {
                    currentRoomIndex = 0;
                    gameState = GameState.Playing;
                    mainMenu.tutorial = false;
                    _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                    stat_tutorial_Sheet.Play();
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
                    boss.Reset();
                    currentRoomIndex = random.Next(1, 4);
                    stageStep = 0;
                    currentRoom.ResetRoom();
                    Door_Open = false;
                    DoorCityOpenTexture.Reset();
                    DoorLabOpenTexture.Reset();
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
                    start_city_Sheet.Play();
                    start_obivionlab_Sheet.Play();
                }
                if (gameOver.Menu)
                {
                    gameState = GameState.MainMenu;
                    gameOver.Menu = false;
                    gameOver.StartGame = false;
                }
                return;
            }

            if (gameState == GameState.Playing)
            {
                if (boss.Health <= 0 && !isBossDead)
                {
                    isBossDead = true;
                    timer = 0f;
                }
                if (isBossDead)
                {
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (timer >= 3f && fadeAlpha < 1f) // ดีเลย์ 3 วิ แล้วเริ่ม fade
                    {
                        fadeAlpha += (float)gameTime.ElapsedGameTime.TotalSeconds / 3f; // 3 วิ ให้เต็ม
                        if (fadeAlpha >= 1f)
                        {
                            fadeAlpha = 1f;
                            gameState = GameState.MainMenu;

                            isBossDead = false;
                            timer = 0f;
                            fadeAlpha = 0f;
                        }
                    }
                }


                camera.Follow(_player.drect, new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));

                if (_player.IsAlive)
                {
                    _player.Update(gameTime, camera.position);
                    if (currentRoom.Enemies.All(e => !e.IsActive))
                    {
                        Door_Open = true;
                    }

                    _player.ClampPosition(currentRoom.Bounds, currentRoom.Obstacles);

                    if (Door_Open)
                    {
                        if (_player.drect.Intersects(currentRoom.Door) && currentRoom.NextRooms.Count > 0 && currentRoomIndex != 0)
                        {
                            stageStep++;
                            switch (stageStep)
                            {
                                case 1:
                                    currentRoomIndex = currentRoom.NextRooms[random.Next(currentRoom.NextRooms.Count)];
                                    break;
                                case 2:
                                    currentRoomIndex = random.Next(4, 7);
                                    break;
                                case 3:
                                    currentRoomIndex = currentRoom.NextRooms[random.Next(currentRoom.NextRooms.Count)]; // 4,5,6
                                    break;
                                case 4:
                                    currentRoomIndex = 7;
                                    break;
                                default:
                                    gameState = GameState.MainMenu;
                                    currentRoomIndex = 0;
                                    stageStep = 0;
                                    _player.Reset();
                                    boss.Reset();
                                    break;
                            }
                            rooms[currentRoomIndex].ResetRoom();
                            _playerBullets.Clear();
                            _enemyBullets.Clear();
                            _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                            Door_Open = false;
                            DoorCityOpenTexture.Reset();
                            DoorLabOpenTexture.Reset();
                        }
                        else if (_player.drect.Intersects(currentRoom.Door) && currentRoomIndex == 0)
                        {
                            gameState = GameState.MainMenu;
                            _player.SetPosition(rooms[currentRoomIndex].StartPosition);
                            _player.Reset();
                            boss.Reset();
                            
                            currentRoomIndex = random.Next(1, 4); ;
                            stageStep = 0;
                            currentRoom.ResetRoom();
                            Door_Open = false;
                            DoorCityOpenTexture.Reset();
                            DoorLabOpenTexture.Reset();
                            _enemyBullets.Clear();
                            _playerBullets.Clear();
                            mainMenu.StartGame = false;
                            mainMenu.ExitGame = false;
                            mainMenu.tutorial = false;
                        }
                    }
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
                        enemy.Update(_player, currentRoom.Obstacles, gameTime);
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
                                enemy.TakeDamage(70, _player.position, true);
                                _player._isSlash = false;
                                break; // Exit the loop after hitting the first enemy
                            }
                        }
                    }
                    if (boss.IsbossAticve && _player.meleeWeapon.AttackHitbox.Intersects(boss.HitboxgetDamage))
                    {
                        if (_player._isSlash)
                        {
                            boss.TakeDamage(70);
                            _player._isSlash = false;
                        }
                    }
                }
                for (int i = _enemyBullets.Count - 1; i >= 0; i--)
                {
                    EnemyBullet bullet = _enemyBullets[i];
                    Bullet newParriedBullet = bullet.Update(gameTime, currentRoom.Obstacles, _player);

                    // 2. ตรวจสอบว่ามีกระสุน Parry ถูกสร้างขึ้นมาหรือไม่
                    if (newParriedBullet != null)
                    {
                        // 3. เพิ่มกระสุน Parry เข้าไปใน List ของกระสุนฝ่ายผู้เล่น
                        _playerBullets.Add(newParriedBullet);
                    }

                    if (!bullet.IsActive)
                    {
                        _enemyBullets.RemoveAt(i);
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
                            enemy.TakeDamage(40, _player.position, false);
                            bullet.IsActive = false;
                            break; // Exit the inner loop after hitting an enemy
                        }
                    }
                    if (boss.IsbossAticve && bullet.boundingBox.Intersects(boss.HitboxgetDamage))
                    {
                        boss.TakeDamage(40);
                        bullet.IsActive = false;
                    }

                    // Remove inactive bullets
                    if (!bullet.IsActive)
                    {
                        _playerBullets.RemoveAt(i);
                    }
                }

                if (currentRoomIndex == 7 && boss.Health > 0)
                {
                    boss.IsbossAticve = true;
                }
                else
                {
                    boss.IsbossAticve = false;
                }
                if (boss.IsbossAticve)
                {
                    boss.Update(gameTime, _player, currentRoom.Obstacles);
                    var newlySpawnedEnemies = boss.GetSpawnedEnemies();

                    // *** เพิ่มโค้ดส่วนนี้เข้าไป ***
                    if (newlySpawnedEnemies.Count > 0)
                    {
                        foreach (var enemy in newlySpawnedEnemies)
                        {
                            // ตรวจสอบว่าเป็น Enemy_Shooting หรือไม่
                            if (enemy is Enemy_Shooting shootingEnemy)
                            {
                                // สมัครรับเหตุการณ์ OnShoot ที่นี่
                                shootingEnemy.OnShoot += bullet => _enemyBullets.Add((EnemyBullet)bullet);
                            }
                            // เพิ่มศัตรูใหม่ลงในรายการหลัก
                            currentRoom.Enemies.Add(enemy);
                        }
                    }
                }
                // ... โค้ดสำหรับอัปเดตและเช็กการชนของศัตรูใน currentRoom.Enemies ...
                foreach (var enemy in currentRoom.Enemies)
                {
                    if (enemy.IsActive)
                    {
                        enemy.Update(_player, currentRoom.Obstacles, gameTime);
                        enemy.ClampPosition(currentRoom.Bounds, currentRoom.Obstacles);
                    }
                    if (_player.drect.Intersects(enemy.bulletammo) && enemy._isbulletammo == true)
                    {
                        _player.maxAmmo += 10;
                        enemy.bulletammo = new Rectangle(0, 0, 0, 0);
                        enemy._isbulletammo = false;
                    }
                    else if (_player.drect.Intersects(enemy.HpDrop) && enemy._isHpDrop == true)
                    {
                        _player.Health += 1;
                        enemy.HpDrop = new Rectangle(0, 0, 0, 0);
                        enemy._isHpDrop = false;
                    }
                }
            }
            if (currentKState.IsKeyDown(Keys.X) && !previousKState.IsKeyDown(Keys.X) && _player.isSwordEquipped)
            {
                if (gameState == GameState.Playing && _player.meleeWeapon.CanUseUlt)
                {
                    gameState = GameState.justmentcut;
                    _player.meleeWeapon.getULTHitbox(_player.position);
                    _player.meleeWeapon._ultTimer = 2f; // <-- ตั้งค่าเป็น 2f
                    _player.meleeWeapon._isULTActive = true; // <-- ต้องแน่ใจว่าตั้งค่านี้ด้วย
                    _isSlashUlt = true;
                }
            }
            if (gameState == GameState.justmentcut)
            {
                // 1. อัปเดต Player เพื่อให้ Animation ขยับ
                _player.Update(gameTime, camera.position); // Update Player แต่ไม่ให้รับ Input/เคลื่อนที่

                // 2. จัดการ ULT Timer และ Hitbox
                if (_player.meleeWeapon._isULTActive) // ตรวจสอบว่า ULT กำลังทำงาน
                {
                    if (_isSlashUlt)
                    {
                        // *คุณต้องมีตัวแปร Timer ใน MeleeWeapon*
                        foreach (var enemy in currentRoom.Enemies)
                        {
                            if (enemy.IsActive && _player.meleeWeapon.ULTHitbox.Intersects(enemy.boundingBox))
                            {
                                enemy.TakeDamage(500, _player.position, false);
                            }
                        }
                        if (boss.IsbossAticve && _player.meleeWeapon.ULTHitbox.Intersects(boss.HitboxgetDamage))
                        {
                            boss.TakeDamage(500);
                        }
                        _isSlashUlt = false;
                    }
                    _player.meleeWeapon._ultTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (_player.meleeWeapon._ultTimer <= 0)
                    {
                        // สิ้นสุด ULT
                        _player.meleeWeapon._isULTActive = false;
                        _player.meleeWeapon.ULTHitbox = Rectangle.Empty;
                        _player.meleeWeapon.CanUseUlt = false;
                        _player.meleeWeapon.ultCharge = 0f;
                        gameState = GameState.Playing;
                    }
                }
            }
            previousKState = currentKState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (gameState == GameState.Paused)
            {
                _spriteBatch.Begin();
                paused.Draw(_spriteBatch);
                _spriteBatch.End();
            }
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
            else if (gameState == GameState.Playing || gameState == GameState.justmentcut)
            {
                var transform = Matrix.CreateTranslation(camera.position.X, camera.position.Y, 0);

                _spriteBatch.Begin(transformMatrix: transform);

                rooms[currentRoomIndex].Draw(_spriteBatch);

                if (Door_Open)
                {
                    if(currentRoomIndex == 0 ||currentRoomIndex == 1 || currentRoomIndex == 2 || currentRoomIndex == 3) 
                    {
                        if (!DoorCityOpenTexture.IsEnd)
                        { DoorCityOpenTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds); }
                        else if (DoorCityOpenTexture.IsEnd)
                        {
                            DoorCityOpenTexture.DrawFrame(_spriteBatch, 3, rooms[currentRoomIndex].DoorPosition);
                        }
                    }
                    if (currentRoomIndex == 4 || currentRoomIndex == 5 || currentRoomIndex == 6 || currentRoomIndex == 7)
                    {
                        if (!DoorLabOpenTexture.IsEnd)
                        { DoorLabOpenTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds); }
                        else if (DoorLabOpenTexture.IsEnd)
                        {
                            DoorLabOpenTexture.DrawFrame(_spriteBatch, 4, rooms[currentRoomIndex].DoorPosition);
                        }
                    }
                }
                // Draw the player
                _player.Draw(_spriteBatch, camera.position);

                
                // Draw all active enemies in the current room
                foreach (var enemy in rooms[currentRoomIndex].Enemies)
                {
                    enemy.Draw(_spriteBatch,gameTime);

                    if (enemy._isbulletammo) 
                    {
                        _spriteBatch.Draw(AmmoDrop, enemy.Position - new Vector2(17 , 48), null , Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                    }
                    if (enemy._isHpDrop)
                    {
                        _spriteBatch.Draw(HealDrop, enemy.Position - new Vector2(17, 48), null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                    }
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
                if (boss.IsbossAticve)
                {
                    boss.Draw(_spriteBatch);

                    int previous_randomIndex;

                    if (boss.currentState == Boss.BossState.Idle)
                    {
                        boss_laser_start.Reset();
                        boss_gun_start.Reset();
                        previous_randomIndex = boss.randomIndex;
                        if (previous_randomIndex == 0)
                        {
                            if (!boss_laser_done.IsEnd) 
                            {
                                boss_laser_done.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                                boss_laser_done.DrawFrame(_spriteBatch, boss.Position);
                            }
                        }
                        else if (previous_randomIndex == 1)
                        {
                            if (!boss_gun_done.IsEnd)
                            {
                                boss_gun_done.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                                boss_gun_done.DrawFrame(_spriteBatch, boss.Position);
                            }
                        }
                    }
                    else if (boss.currentState == Boss.BossState.Attacking)
                    {
                        boss_gun_done.Reset();
                        boss_laser_done.Reset();
                        if (boss.randomIndex == 0)
                        {
                            if (!boss_laser_start.IsEnd)
                            {
                                boss_laser_start.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                                boss_laser_start.DrawFrame(_spriteBatch, boss.Position);
                                previous_randomIndex = boss.randomIndex;
                            }
                            if (boss_laser_start.IsEnd)
                            {
                                attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                boss_laser.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                                boss_laser.DrawFrame(_spriteBatch, boss.Position);
                            }
                        }
                        else if (boss.randomIndex == 1)
                        {
                            if (!boss_gun_start.IsEnd)
                            {
                                boss_gun_start.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                                boss_gun_start.DrawFrame(_spriteBatch, boss.Position);
                                previous_randomIndex = boss.randomIndex;
                            }
                            else if (boss_gun_start.IsEnd)
                            {
                                attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                boss_gun.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                                boss_gun.DrawFrame(_spriteBatch, boss.Position);
                            }
                        }
                    }
                }
                if (boss.Health <= 0)
                {
                    boss_death.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                    boss_death.DrawFrame(_spriteBatch, boss.Position);
                }
                rooms[currentRoomIndex].DrawLayer2(_spriteBatch);

                // Draw hitboxes
                if (_isOpenhitbox)
                {
                    DrawRectangle(_spriteBatch, _player.drect, Color.Black, 1);
                    DrawRectangle(_spriteBatch, boss.HitboxgetDamage, Color.Yellow, 1);
                    DrawRectangle(_spriteBatch, _player.HitboxgetDamage, Color.Blue, 1);
                    DrawRectangle(_spriteBatch, _player.meleeWeapon.AttackHitbox, Color.Yellow, 1);
                    DrawRectangle(_spriteBatch, _player.meleeWeapon.ParryHitbox, Color.Red, 1);
                    DrawRectangle(_spriteBatch, _player.meleeWeapon.ULTHitbox, Color.Red, 1);
                    foreach (var room in rooms)
                    {
                        foreach (var enemy in room.Enemies)
                        {
                            DrawRectangle(_spriteBatch, enemy.bulletammo, Color.Yellow, 1);
                            DrawRectangle(_spriteBatch, enemy.HpDrop, Color.Green, 1);
                        }
                    }
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
                }
                _spriteBatch.End();

                _spriteBatch.Begin();

                if (boss.IsbossAticve)
                {
                    Vector2 bossHpPos = new Vector2(1920 / 2 - Boss_Max_Hp.Width / 2, 950);
                    float healthPercent = (float)boss.Health / boss.MaxHealth;
                    if (healthPercent < 0f) healthPercent = 0f;
                    _spriteBatch.Draw(Boss_Max_Hp, bossHpPos, Color.White);
                    Rectangle srcRect = new Rectangle(0, 0, (int)(Boss_Current_Hp.Width * healthPercent), Boss_Current_Hp.Height);
                    Rectangle destRect = new Rectangle((int)bossHpPos.X, (int)bossHpPos.Y, srcRect.Width, srcRect.Height);
                    _spriteBatch.Draw(Boss_Current_Hp, destRect, srcRect, Color.White);
                }
                
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

                Vector2 EnergyPos = new Vector2(0,120);
                float EnergyPercent = _player.meleeWeapon.ultCharge / _player.meleeWeapon.ultChargeMax;
                if (EnergyPercent < 0f) EnergyPercent = 0f;
                _spriteBatch.Draw(Energy_Max_bar, EnergyPos, Color.White);
                Rectangle Energy_srcRect = new Rectangle(0, 0, (int)(Energy_Current_bar.Width * EnergyPercent), Energy_Current_bar.Height);
                Rectangle Energy_destRect = new Rectangle((int)EnergyPos.X, (int)EnergyPos.Y, Energy_srcRect.Width, Energy_srcRect.Height);
                _spriteBatch.Draw(Energy_Current_bar, Energy_destRect, Energy_srcRect, Color.White);

                if (_player.meleeWeapon.CanUseUlt) 
                {
                    fullenergy_Sheet.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                    fullenergy_Sheet.DrawFrame(_spriteBatch, EnergyPos);
                }

                if (gameState == GameState.Playing)
                {
                    if (currentRoomIndex == 0)
                    {
                        if (stat_tutorial_Sheet.IsEnd) { stat_tutorial_Sheet.Stop(); }
                        stat_tutorial_Sheet.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                        stat_tutorial_Sheet.DrawFrame(_spriteBatch, new Vector2(1200, 50));
                    }
                    else if (stageStep == 0 && currentRoomIndex != 0)
                    {
                        if (start_city_Sheet.IsEnd) { start_city_Sheet.Stop(); }
                        start_city_Sheet.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                        start_city_Sheet.DrawFrame(_spriteBatch, new Vector2(1200, 50));
                    }
                    else if (stageStep == 2 && currentRoomIndex != 0)
                    {
                        if (start_obivionlab_Sheet.IsEnd) { start_obivionlab_Sheet.Stop(); }
                        start_obivionlab_Sheet.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                        start_obivionlab_Sheet.DrawFrame(_spriteBatch, new Vector2(1200, 50));
                    }
                }
                string ammoText = $"{_player.currentAmmo}/{_player.maxAmmo}";
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
                cursorReloadTexture.DrawFrame(_spriteBatch, cursorPosition - new Vector2(15,15));
            }
            else
            {
                if (_player.currentState == PlayerState.Attacking) 
                {
                    cursorTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                    cursorTexture.DrawFrame(_spriteBatch, cursorPosition - new Vector2(25, 25));
                }
                else if (_player.isSwordEquipped)
                {
                    if (_player.currentState != PlayerState.Attacking)
                    {
                        cursorTexture.DrawFrame(_spriteBatch, 2, cursorPosition - new Vector2(25, 25));
                    }
                }
                else if (!_player.isSwordEquipped)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        cursorTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);
                        cursorTexture.DrawFrame(_spriteBatch, cursorPosition - new Vector2(25, 25));
                    }else
                    {
                        cursorTexture.DrawFrame(_spriteBatch, 2, cursorPosition - new Vector2(25, 25));
                    }
                }
            }

            if (fadeAlpha > 0f)
            {
                _spriteBatch.Draw(whiteTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White * fadeAlpha);
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