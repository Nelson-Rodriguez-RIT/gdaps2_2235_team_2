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
using System.Text.RegularExpressions;

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
        public static Dictionary<T, Tuple<Texture2D, int[]>> //This will hold the animation data for each sprite sheet
            LoadSpriteSheets<T>(string directory, ContentManager content)
        {

            //Variables we need
            StreamReader reader = null;
            List<string> lines = new List<string>();    // the lines in the file
            Object[] states = Enum.GetValues(typeof(T))
                .Cast<Object>()
                .ToArray();     // each constant in the enum
            string[] names = Enum.GetNames(typeof(T));  // the names of each animation state
            Dictionary<T, Tuple<Texture2D, int[]>> spriteData = new();    // what we are going to return
            

            //Load each line into a string array
            try
            {

                for (int i = 0; i < states.Length; i++)
                {
                    string tempPath = directory + "/" + names[i];
                    try
                    {
                        spriteData.Add(
                        (T)states[i],
                        new Tuple<Texture2D, int[]>
                            (content.Load<Texture2D>(tempPath),
                            null));

                    }
                    catch (ContentLoadException e )
                    {
                        //console log "spritesheet not found"
                    }
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
            int arrayLength = 3;                        // make this not hard coded in the future (?)

            for (int i = 0; i < lines.Count; i++)
            {
                //Check if the line is a animation identifier
                if (names.Contains(lines[i]))
                {                
                    if (data != null )                  // don't save data if it doesn't exist
                    {
                        T temp = (T)Enum.Parse(typeof(T), lines[index]);    // get the animation state the data is paired with

                        spriteData[temp] = new Tuple
                            <Texture2D, int[]>(spriteData[temp].Item1, data);     // add the data to the dictionary paired with the state
                    }

                    index = i;                                    // update the last identifier index
                    data = new int[arrayLength];                  // create a new data array
                }
                //if not, add the data to the array
                else
                {
                    data[i - index - 1] = int.Parse(lines[i]);

                    if (i == lines.Count() - 1)
                    {
                        T temp = (T)Enum.Parse(typeof(T), lines[index]);    // get the animation state the data is paired with

                        spriteData[temp] = new Tuple
                            <Texture2D, int[]>(spriteData[temp].Item1, data);     // add the data to the dictionary paired with the state
                    }
                }
            }

            //Check to make sure each animation has data
            foreach (KeyValuePair<T, Tuple<Texture2D, int[]>> keyValuePair in spriteData)
            {
                if (keyValuePair.Value.Item2 == null)
                {
                    throw new Exception($"Data for '{keyValuePair.Key}'" +
                        $"was not found in the file.");
                }
            }

            return spriteData;
        }
       
    }
}
