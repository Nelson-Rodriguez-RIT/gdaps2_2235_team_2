using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace GameControls_Concept
{
    internal class LevelManager
    {
        private List<Platform> platforms;
        public Texture2D texture;

        private LevelManager instance;

        public List<Platform> Platforms
        {
            get { return platforms; }
        }

        public LevelManager(Texture2D texture, LevelManager instance)
        {
            platforms = new List<Platform>();
            this.texture = texture;
            AddPlatform(new Rectangle(0, 800, 600, 100));

            this.instance = instance;
        }

        private void AddPlatform(Rectangle hitbox)
        {
            platforms.Add(new Platform(hitbox, texture, instance));
        }

        public void LoadFromFile(string path)
        {
            StreamReader reader = null;
            string[] lines = null;

            try
            {
                reader = new StreamReader(path);
                string line = null;
                List<string> linesList = new List<string>();

                while ((line = reader.ReadLine()) != null)
                {
                    linesList.Add(line);
                }

                lines = linesList.ToArray();
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception("File does not exist");
            }
            finally
            {
                reader.Close();
            }

            foreach (string line in lines)
            {
                int[] dimensions = new int[4];
                string[] dimensionsString = line.Split(',');

                for (int i = 0; i < dimensionsString.Length; i++)
                {
                    dimensions[i] = int.Parse(dimensionsString[i]);
                }

                AddPlatform(new Rectangle(
                    dimensions[0],
                    dimensions[1],
                    dimensions[2],
                    dimensions[3]));
            }
        }
    }
}
