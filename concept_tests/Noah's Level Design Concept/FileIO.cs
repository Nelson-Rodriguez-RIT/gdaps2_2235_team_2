using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Noah_s_Level_Design_Concept
{
    internal class FileIO
    {
        private StreamReader reader = null;

        public void ReadFile()
        {
            string file = "/../../MoonwalkMapStartText";
            try 
            {
                reader = new StreamReader(file);
                string line;

                while ((line = reader.ReadLine()) != null) 
                {
                    if (!line.Contains(" <layer id=")) continue;
                    else line.Split(" ");
                    

                }
            }
            catch { }
        
        
        }






    }
}
