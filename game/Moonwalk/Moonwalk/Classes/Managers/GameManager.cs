using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers
{
    /// <summary>
    /// Central class that controls/ maintains all elements of the game
    /// </summary>

    //game states (maybe discern states between who has control?)
    enum GameStates
    {
        Menu,
        Settings,
        Playing,
        Dead
    }
    internal class GameManager
    {
        //property that gets/sets game state
        public GameStates State
        {
            get { return State; }
            set { State = value; }
        }
    }
}
