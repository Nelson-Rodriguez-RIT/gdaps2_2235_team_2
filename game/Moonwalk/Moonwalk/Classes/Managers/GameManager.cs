using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Managers.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers
{
    enum GameState {
        Test_Load,
        Test,
    }

    /// <summary>
    /// Central class that controls/ maintains all elements of the game
    /// </summary>
    internal sealed class GameManager {

        private static GameManager _instance = null;

        private ContentManager _content;

        // Game element managers
        private Camera _camera;
        private MapManager _map;

        // Gameplay related states
        private GameState state;
        private KeyboardState kbState;
        private MouseState msState;

        // Currently loaded entities
        private List<Entity> entities;

        private GameManager(ContentManager content) {
            // Get content for loading needs
            _content = content;

            // Get each managers respective instance
            _camera = Camera.GetInstance();
            _map = MapManager.GetInstance();

            Loader.Content = _content;

            state = GameState.Test_Load;
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
            kbState = Keyboard.GetState();
            msState = Mouse.GetState();

            switch (state) {
                case GameState.Test_Load:
                    _map.Load(MapGroups.Test);
                    state = GameState.Test;
                    break;

                case GameState.Test:
                    break;
            }
        }

        /// <summary>
        /// Handles draw logic
        /// </summary>
        public void Draw(SpriteBatch sb) {
            switch (state) {
                case GameState.Test_Load:
                    break;

                case GameState.Test:
                    _map.Draw(sb);
                    break;
            }
        }
    }


}
