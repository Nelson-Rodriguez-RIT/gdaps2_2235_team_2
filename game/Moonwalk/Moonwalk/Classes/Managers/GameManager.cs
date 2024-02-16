using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers
{
    enum GameState {
        MainMenu,
        Options,
        Playing,
        Death,
        LevelCompletion
    }

    /// <summary>
    /// Central class that controls/ maintains all elements of the game
    /// </summary>
    internal sealed class GameManager
    {

        private static GameManager instance = null;

        // Game element managers
        private Camera camera;
        private Level level;
        private GUI hud;

        // Gameplay related states
        private GameState state;
        private KeyboardState kbState;
        private MouseState msState;

        // Currently loaded entities
        private List<Entity> entities;

        private GameManager() {
            // Get each managers respective instance
            camera = Camera.GetInstance();
            level = Level.GetInstance();
            hud = GUI.GetInstance();

            state = GameState.MainMenu;
        }


        /// <summary>
        /// Gets GameManager's singleton instance
        /// </summary>
        /// <returns>A GameManager object</returns>
        public static GameManager GetInstance() {
            if (instance == null)
                instance = new GameManager();

            return instance;
        }


        /// <summary>
        /// Handles gameplay logic
        /// </summary>
        public void Update(GameTime gt) {
            // Get user input
            kbState = Keyboard.GetState();
            msState = Mouse.GetState();

            switch (state) {
                case GameState.MainMenu:
                    break;

                case GameState.Options:
                    break;

                case GameState.Playing:
                    break;

                case GameState.Death:
                    break;

                case GameState.LevelCompletion:
                    break;
            }
        }

        /// <summary>
        /// Handles draw logic
        /// </summary>
        public void Draw(SpriteBatch sb) {
            switch (state) {
                case GameState.MainMenu:
                    break;

                case GameState.Options:
                    break;

                case GameState.Playing:
                    break;

                case GameState.Death:
                    break;

                case GameState.LevelCompletion:
                    break;
            }
        }

        private void UpdateEntities(GameTime gt) {

        }
    }


}
