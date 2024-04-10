using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Boss;
using Moonwalk.Classes.Entities;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Maps;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;


namespace Moonwalk.Classes.Managers {
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

        // Currently loaded entities
        public static Assortment<Entity> entities;
        // private static Dictionary<string, GUIButtonElement> guiButtonElements;
        private static Dictionary<string, List<GUIElement>> guiBuffers;
        
        List<Type> types;

        //Input handling
        private StoredInput storedInput;

        // For testing purposes
        private Vector2 cameraTarget;

        private bool isPauseEnabled;
        private bool isGamePaused;

        private GameManager(ContentManager content) {
            // Get content for loading needs
            Loader.Content = content;
            storedInput = new StoredInput();
            Camera.GlobalOffset = WindowManager.Instance.Center;

            isPauseEnabled = false;
            isGamePaused = false;

            //Testing for my new entity list concept
            types = new List<Type>();
            types.Add(typeof(Player));
            types.Add(typeof(Robot));
            types.Add(typeof(Enemy));
            types.Add(typeof(Projectile));
            types.Add(typeof(Entity));

            entities = new Assortment<Entity>(types);
            //guiButtonElements = new Dictionary<string, GUIButtonElement>();
            guiBuffers = new Dictionary<string, List<GUIElement>>();

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

            if (storedInput.PreviousMouse.LeftButton == ButtonState.Pressed &&
                    storedInput.CurrentMouse.LeftButton == ButtonState.Pressed)
                storedInput.Click();

            if (storedInput.PreviousKeyboard.IsKeyDown(Keys.Escape) && // Press ESC to pause the game
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.Escape) &&
                    isPauseEnabled) {
                if (isGamePaused) { // Prepare to unpause game
                    foreach (GUIButtonElement button in guiBuffers["PauseMenu"])
                        GUI.RemoveElement(button);

                    guiBuffers.Remove("PauseMenu");
                }
                else { // Prepare to pause game
                    guiBuffers.Add("PauseMenu", new List<GUIElement>());

                    guiBuffers["PauseMenu"].Add(new GUIButtonElement(
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 100),
                            (int)(WindowManager.Instance.Center.Y - 50),
                            GUI.GetTexture("ButtonStart").Width * 2,
                            GUI.GetTexture("ButtonStart").Height * 2
                            ),
                        "ButtonResume",
                        Color.White));
                    GUI.AddElement(guiBuffers["PauseMenu"][0]);

                    guiBuffers["PauseMenu"].Add(new GUIButtonElement(
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 100),
                            (int)(WindowManager.Instance.Center.Y + 50),
                            GUI.GetTexture("ButtonExit").Width * 2,
                            GUI.GetTexture("ButtonExit").Height * 2
                            ),
                        "ButtonMainMenu",
                        Color.White));
                    GUI.AddElement(guiBuffers["PauseMenu"][1]);
                }

                isGamePaused = !isGamePaused;
            }
            

            if (!isGamePaused) {
                if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F1) && // Toggle F1 to draw entity hitboxes
                    storedInput.CurrentKeyboard.IsKeyUp(Keys.F1))
                {
                    displayEntityHitboxes = !displayEntityHitboxes;
                    BossFight.DrawHitboxes = !BossFight.DrawHitboxes;
                }
                    
                    

                if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F2) && // Toggle F2 to draw terrain hitboxes
                        storedInput.CurrentKeyboard.IsKeyUp(Keys.F2))
                    displayTerrainHitboxes = !displayTerrainHitboxes;

                if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F3) && // Toggle F3 to draw hitbox hitboxes
                        storedInput.CurrentKeyboard.IsKeyUp(Keys.F3))
                    Hitbox.drawHitbox = !Hitbox.drawHitbox;

                if (storedInput.PreviousKeyboard.IsKeyDown(Keys.F4) && // Press F4 to reset the current map/gamestate
                        storedInput.CurrentKeyboard.IsKeyUp(Keys.F4)) {
                    Map.UnloadMap();
                    Transition(state);
                }

                switch (state) {
                    case GameState.MainMenu:
                        //Start button pressed
                        if (((GUIButtonElement)guiBuffers["MainMenu"][0]).Clicked)
                            Transition(GameState.Demo);

                        //Exit button pressed
                        else if (((GUIButtonElement)guiBuffers["MainMenu"][1]).Clicked)
                            GameMain.ExitGame();
                        break;

                    case GameState.Demo:
                        break;
                }

                foreach (Entity entity in entities) {
                    if (VectorMath.Magnitude
                            (VectorMath.Difference
                                (Player.Location.ToVector2(), entity.Hitbox.Center.ToVector2()))
                        < 400)
                    entity.Update(gt, storedInput);

                    
                }

                for (int i = 0; i < Hitbox.activeHitboxes.Count; i++) {
                    int length = Hitbox.activeHitboxes.Count;

                    Hitbox.activeHitboxes[i].Update(gt);

                    if (Hitbox.activeHitboxes.Count < length) 
                    {
                        i--;
                    }
                }

                for (int i = 0; i < Particle.Effects.Count; i++) {
                    int length = Particle.Effects.Count;

                    Particle.Effects[i].Update(gt);

                    if (Particle.Effects.Count < length) {
                        i--;
                    }
                }

                if (BossFight.Boss != null)
                BossFight.Boss.Update(gt);
            }
            else {
                //Resume button pressed
                if (((GUIButtonElement)guiBuffers["PauseMenu"][0]).Clicked) {
                    foreach (GUIButtonElement button in guiBuffers["PauseMenu"])
                        GUI.RemoveElement(button);

                    guiBuffers.Remove("PauseMenu");
                    isGamePaused = false;
                }
                    
                //Main Menu button pressed
                else if (((GUIButtonElement)guiBuffers["PauseMenu"][1]).Clicked) {
                    Transition(GameState.MainMenu);
                    isGamePaused = false;
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

            if (BossFight.Boss != null)
            {
                BossFight.Boss.Draw(batch);
            }

            if (!isGamePaused)
                switch (state) {
                    case GameState.MainMenu:
                        graphics.Clear(Color.Black);
                        break;

                    case GameState.Demo:
                        graphics.Clear(Color.Gray);
                        break;
                }
            else {
                
            }

            // Elements drawn ever iteration
            foreach (Entity entity in entities) {
                entity.Draw(batch);

                if (displayEntityHitboxes)
                    entity.DrawHitbox(batch);
            }

            if (Hitbox.drawHitbox) {
                foreach (Hitbox h in Hitbox.activeHitboxes) {
                    h.DrawHitbox(batch);
                }
            }

            foreach (Particle p in Particle.Effects) {
                p.Draw(batch);
            }

            GUI.Draw(batch);
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
                    guiBuffers.Clear();
                    GUI.Clear();

                    guiBuffers.Add("MainMenu", new List<GUIElement>());
                    isPauseEnabled = false;

                    GUI.AddElement(new GUITextElement(
                        WindowManager.Instance.Center - new Vector2(150, 150),
                        "Moonwalk",
                        "MonogramTitle",
                        Color.White
                        ));

                    guiBuffers["MainMenu"].Add(new GUIButtonElement(
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 100),
                            (int)(WindowManager.Instance.Center.Y - 50),
                            GUI.GetTexture("ButtonStart").Width * 2,
                            GUI.GetTexture("ButtonStart").Height * 2
                            ),
                        "ButtonStart",
                        Color.White));
                    GUI.AddElement(guiBuffers["MainMenu"][0]);

                    guiBuffers["MainMenu"].Add(new GUIButtonElement(
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 100),
                            (int)(WindowManager.Instance.Center.Y + 50),
                            GUI.GetTexture("ButtonExit").Width * 2,
                            GUI.GetTexture("ButtonExit").Height * 2
                            ),
                        "ButtonExit",
                        Color.White));
                    GUI.AddElement(guiBuffers["MainMenu"][1]);

                    GUI.AddElement(new GUITextureElement(
                        new Rectangle(
                            (int)(WindowManager.Instance.Center.X - 230),
                            (int)(WindowManager.Instance.Center.Y - 245),
                            GUI.GetTexture("MenuBorder").Width * 10,
                            GUI.GetTexture("MenuBorder").Height * 10),
                        "MenuBorder",
                        Color.White));

                    GUI.AddElement(new GUITextElement(
                        new Vector2 (10, 10),
                        "Instructions:\n" +
                        "WASD - Movement\n" +
                        "E - Melee Attack\n" +
                        "SHIFT (HOLD) - Ranged attack\n" +
                        "M1 - Impulse\n" +
                        "M2 - Tether",
                        "MonogramRegular",
                        Color.White
                        ));
                    break;

                case GameState.Demo:
                    GUI.Clear();
                    guiBuffers.Clear();
                    isPauseEnabled = true;
                    /*
                    if (!Map.Loaded) {
                        Map.LoadMap("Demo");

                        Player player = SpawnEntity<Player>();
                        Robot robot = SpawnEntity<Robot>(new Vector2(128, 48));
                        SpawnEntity<Box>(new Vector2(200, -100));
                        SpawnEntity<Turret>(new Vector2(250, 100));

                        // Set player as the Camera's target
                        Camera.SetTarget(player);

                        //Add subscribers to player events
                        player.GetRobotPosition += robot.GetPosition;
                        player.ToggleBotLock += robot.ToggleLock;
                    }*/
                    if (!Map.Loaded)
                    {
                        Map.LoadMap("MoonwalkMap");

                        Player.Respawn();                        

                        WidowBoss.Start();
                    }

                    else
                    {
                        // Loads player + companion
                        Player player = SpawnEntity<Player>();
                        Robot robot = SpawnEntity<Robot>(new Object[] { new Vector2(128, 48) });

                        // Set player as the Camera's target
                        Camera.SetTarget(player);

                        //Add subscribers to player events
                        player.GetRobotPosition += robot.GetPosition;
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
        public static T SpawnEntity<T>(Object[] args = null) where T : Entity
        {
            Entity entity = (Entity)Activator.CreateInstance(typeof(T), args);
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
