using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameControls_Concept
{
    internal abstract class Entity
    {
        protected Vector2 position;
        protected Texture2D image;
        protected Rectangle hitbox;
        protected LevelManager levelManager;

        //Animation
        protected int spriteWidth;
        protected int spriteHeight;
        protected int fps;
        protected int frameCount;

        public virtual Rectangle Hitbox
        {
            get { return hitbox; }
        }

        public virtual Vector2 Position
        {
            get { return position; }
        }

        public Entity(LevelManager manager, Vector2 position)
        {
            this.position = position;          
            this.image = image;

            hitbox = new Rectangle
                ((int)position.X - (50),
                (int)position.Y - (50),
                100,
                100);
            this.levelManager = manager;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(image,
                hitbox,
                Color.White);
        }

        public virtual void Update(GameTime gameTime)
        {
            
        }

        public virtual void LoadSprite(Texture2D texture)
        {
            image = texture;
        }


        /// <summary>
        /// Gets data relevant to animation from a file folder.
        /// </summary>
        /// <typeparam name="T">The enum of animation states</typeparam>
        /// <param name="directory">Folder path for files related to the class</param>
        /// <param name="content">Content loader</param>
        /// <returns>A dictionary of animation states paired with the data that goes with them</returns>
        /// <exception cref="Exception"></exception>
        public static Dictionary<T, int[]> //This will hold the animation data for each sprite sheet
            LoadSpriteSheets<T>(string directory, ContentManager content)
        {
            //Variables we need
            StreamReader reader = null;
            List<string> lines = new List<string>();
            var states = Enum.GetValues(typeof(T));
            Dictionary<T, int[]> spriteData = new();

            //Load each line into a string array
            try
            {
                //font = content.Load<SpriteFont>("File");

                    /*
                idleSheet = content.Load<Texture2D>(directory + "/Idle");
                moveSheet = content.Load<Texture2D>(directory + "/Move");
                    */
                //actionSheet = content.Load<Texture2D>(directory + "/Action");

                foreach (var state in states)
                {
                    
                }

                reader = new StreamReader(directory + "/data.txt");
                string line = null;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line != "" && line[0] != '/')
                    {
                        lines.Add(line);
                    }
                }


            }
            catch (FileNotFoundException e)
            {
                //console log error. For now: 

                throw new Exception("File error");

            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
            
            int[] data = null;                          // the array to hold data for each animation
            int index = 0;                              // the index of the last animation identifier
            string[] names = Enum.GetNames(typeof(T));  // the names of each animation state
            int arrayLength = 3;                        // make this not hard coded in the future (?)

            for (int i = 0; i < lines.Count; i++)
            {
                //Check if the line is a animation identifier
                if (names.Contains(lines[i]) )
                {
                    index = i;                          // update the last identifier index
                    
                    if (data != null )                  // don't save data if it doesn't exist
                    {
                        T temp = (T)Enum.Parse(typeof(T), lines[index]);    // get the animation state the data is paired with

                        spriteData.Add(temp, data);     // add the data to the dictionary paired with the state
                    }

                    data = new int[arrayLength];                  // create a new data array
                }
                //if not, add the data to the array
                else
                {
                    data[i - index - 1] = int.Parse(lines[i]);
                }
            }

            return spriteData;
        }
       
    }
}
