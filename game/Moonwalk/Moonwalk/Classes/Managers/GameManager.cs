using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Maps;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;


namespace Moonwalk.Classes.Managers
{
    enum GameState {
        MainMenu,
        Demo,
        Dead
    }

    /// <summary>
    /// Central class that controls/ maintains all elements of the game
    /// </summary>
    internal sealed class GameManager {
        private static GameManager _instance = null;

        private bool displayEntityHitboxes = false;
        private bool displayTerrainHitboxes = false;

        // Gameplay related states
        private GameState state;
        private KeyboardState kbState;
        private MouseState msState;

        // Currently loaded entities
        public static Assortment<Entity> entities;

        List<Type> types;

        //Input handling:
        private StoredInput storedInput;

        // For testing purposes
        private Vector2 cameraTarget;

        private GameManager(ContentManager content) {
            // Get content for loading needs
            Loader.Content = content;
            storedInput = new StoredInput();
            Camera.GlobalOffset = WindowManager.Instance.Center;


            //Testing for my new entity list concept
            types = new List<Type>();
            types.Add(typeof(Player));
            types.Add(typeof(Robot));
            types.Add(typeof(Enemy));
            types.Add(typeof(KeyObject));
            types.Add(typeof(Projectile));
            entities = new Assortment<Entity>(types);

            // Prepares neccessary elements
            Transition(GameState.MainMenu);
        }


        /// <summary>
        /// Gets GameManager's singleton instance
        /// </summary>
        /// <returns>A GameManager object</returns>
        public static GameManager GetInstance(ContentManager content) {
            if (_instance == null)
                _instance = new GameManager(content);

            return _instance;
        }

        /// <summary>
        /// Handles gameplay logic
        /// </summary>
        public void Update(GameTime gt) {
            // Get user input
            storedInput.Update();

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F1) && // Toggle F1 to draw entity hitboxes
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F1))
                displayEntityHitboxes = !displayEntityHitboxes;

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F2) && // Toggle F2 to draw terrain hitboxes
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F2))
                displayTerrainHitboxes = !displayTerrainHitboxes;

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F3) && // Toggle F3 to draw hitbox hitboxes
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F3))
                Hitbox.drawHitbox = !Hitbox.drawHitbox;

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F4) && // Press F4 to reset the current map/gamestate
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F4))
                Transition(state);

            switch (state) {
                case GameState.MainMenu:
                        if(storedInput.CurrentMouse.LeftButton == ButtonState.Pressed &&
                                storedInput.PreviousMouse.LeftButton == ButtonState.Released) {

                        //Start button pressed
                        if (new Rectangle
                            (540, 310, 186, 66)     // Start button position and size
                            .Contains(storedInput.CurrentMouse.Position))
                                Transition(GameState.Demo);

                        //Exit button pressed
                        if (new Rectangle(
                            540, 410, 186, 66) // Exit button position    
                            .Contains(storedInput.CurrentMouse.Position))
                                GameMain.ExitGame();
                    }
                            
                    break;

                case GameState.Demo:
                    foreach (Entity entity in entities) {
                        if (entity is Player) {
                            cameraTarget = new Vector2(entity.Position.X, entity.Position.Y);
                            break;
                        }
                    }

                    if (storedInput.IsPressed(Keys.Escape))
                    {
                        Map.UnloadMap();
                        Transition(GameState.MainMenu);
                    }

                    break;
            }

            foreach (Entity entity in entities)
            {
                entity.Update(gt, storedInput);

                if (entity is IDamageable)
                {
                    IDamageable damageable = (IDamageable) entity;

                    if (damageable.Health <= 0)
                    {
                        DespawnEntity(entity);
                    }
                }

                if (entity is Projectile)
                {
                    Projectile projectile = (Projectile) entity;

                    if (projectile.Collisions <= 0)
                    {
                        DespawnEntity(entity);
                    }
                }
            }

            for (int i = 0; i < Hitbox.activeHitboxes.Count; i++)
            {
                int length = Hitbox.activeHitboxes.Count;

                Hitbox.activeHitboxes[i].Update(gt);

                if (Hitbox.activeHitboxes.Count < length)
                {
                    i--;
                }
            }

            for (int i = 0; i < Particle.Effects.Count; i++)
            {
                int length = Particle.Effects.Count;

                Particle.Effects[i].Update(gt);

                if (Particle.Effects.Count < length)
                {
                    i--;
                }
            }


            storedInput.UpdatePrevious();
        }

        /// <summary>
        /// Handles draw logic
        /// </summary>
        public void Draw(SpriteBatch batch, GraphicsDevice graphics) {
            // Elements draw based on game state (i.e. GUI or menu elements)
            if (Map.Loaded)
                Map.Draw(batch, displayTerrainHitboxes);

            switch (state) {
                case GameState.MainMenu:
                    graphics.Clear(Color.Black);
                    GUI.Draw(batch);
                    break;

                case GameState.Demo:
                    graphics.Clear(Color.Gray);
                    GUI.Draw(batch);
                    break;
            }

            // Elements drawn ever iteration
            foreach (Entity entity in entities) {
                entity.Draw(batch);

                if (displayEntityHitboxes)
                    entity.DrawHitbox(batch);
            }

            if (Hitbox.drawHitbox)
            {
                foreach (Hitbox h in Hitbox.activeHitboxes)
                {
                    h.DrawHitbox(batch);
                }
            }

            foreach (Particle p in Particle.Effects)
            {
                p.Draw(batch);
            }

        }

        /// <summary>
        /// Handles logic when switching between game states
        /// </summary>
        /// <param name="nextState">The next game state to transition to</param>
        private void Transition(GameState nextState, bool clearEntities = true) {
            if (clearEntities)
                entities = new Assortment<Entity>(types);

            switch (nextState) {
                case GameState.MainMenu:
                    GUI.AddElement(new GUITextElement(
                        WindowManager.Instance.Center - new Vector2(150, 150),
                        "Moonwalk",
                        "MonogramTitle",
                        Color.White
                        ));

                    GUI.AddElement(new GUITextureElement(
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 100),
                            (int)(WindowManager.Instance.Center.Y - 50),
                            GUI.GetTexture("ButtonStart").Width * 2,
                            GUI.GetTexture("ButtonStart").Height * 2
                            ),
                        "ButtonStart",
                        Color.White));

                    GUI.AddElement(new GUITextureElement(
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 100),
                            (int)(WindowManager.Instance.Center.Y + 50),
                            GUI.GetTexture("ButtonExit").Width * 2,
                            GUI.GetTexture("ButtonExit").Height * 2
                            ),
                        "ButtonExit",
                        Color.White));

                    GUI.AddElement(new GUITextureElement(
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 230),
                            (int)(WindowManager.Instance.Center.Y - 245),
                            GUI.GetTexture("MenuBorder").Width * 10,
                            GUI.GetTexture("MenuBorder").Height * 10),
                        "MenuBorder",
                        Color.White));

                    break;

                case GameState.Demo:
                    GUI.Clear();

                    if (!Map.Loaded) {
                        Map.LoadMap("Demo");

                        Player player = SpawnEntity<Player>(new Vector2(64, 0));
                        Robot robot = SpawnEntity<Robot>(new Vector2(128, 48));

                        // Set player as the Camera's target
                        Camera.SetTarget(player);

                        //Add subscribers to player events
                        player.GetRobotPosition += robot.GetPosition;
                        player.ToggleBotLock += robot.ToggleLock;
                    }
                    else {
                        // Loads player + companion
                        Player player = SpawnEntity<Player>(new Vector2(800, 290));
                        Robot robot = SpawnEntity<Robot>(new Vector2(128, 48));
                        SpawnEntity<TestEnemy>(new Vector2(200, 250));
                        SpawnEntity<KeyObject>(new Vector2(900, 250));

                        // Set player as the Camera's target
                        Camera.SetTarget(player);

                        //Add subscribers to player events
                        player.GetRobotPosition += robot.GetPosition;
                        player.ToggleBotLock += robot.ToggleLock;
                    }
                        
                    
                    break;
            }

            // Set up map loading triggers
            if (Map.Loaded)
                foreach (MapTrigger trigger in Map.Geometry[typeof(MapTrigger)])
                    trigger.OnCollision += Reset;

            state = nextState;
        }

        /// <summary>
        /// Handles any neccassray logic when spawning an entity
        /// </summary>
        public static T SpawnEntity<T>(Vector2 position, Object[] args = null) where T : Entity {
            //Copy everything into args
            object[] newArgs = new object[
                args != null ? args.Length + 1 : 1
                ];
            newArgs[0] = position;

            if (args != null )
            {
                for (int i = 0; i < args.Length; i++)
                {
                    newArgs[i + 1] = args[i];
                }
            } 

            Entity entity = (Entity)Activator.CreateInstance(typeof(T), newArgs);
            entities.Add(entity);

            return (T)entity;
        }

        /// <summary>
        /// Handles any neccessary logic when despawning an entity
        /// </summary>
        /// <param name="entity">Entity to despawn</param>
        public static void DespawnEntity(Entity entity) {
            entities.Remove(entity);
        }

        private void Reset() {
            Transition(state);
        }
    }


}
