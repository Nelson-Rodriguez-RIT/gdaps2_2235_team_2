using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIO_Concept.Classes
{
    /// <summary>
    /// Contains information about how to handle a group of files in a specific directory
    /// based on registered file format
    /// </summary>
    internal class FileGroup
    {
        private string directory;
        private string filename;

        /// <summary>
        /// Contains how to handle each line of data
        /// </summary>
        private Delegate ruleset;

        /// <summary>
        /// Contains data read from file
        /// </summary>
        private Object data;

        public Object Data {
            get {  return data; }
        }

        public FileGroup(string directory, string filetype, Delegate ruleset, bool loadImmediately = true)
        {
            this.directory = directory;
            filename = filetype;

            this.ruleset = ruleset;

            // Must be null so the ruleset can know that it needs
            // to format this field with neccessary types
            data = null;

            if (loadImmediately)
                Load();
        }

        public void Load()
        {
            // Scan for subdirectories
            List<string> directories = new();
            ScanSubDirectories(directories, directory);

            // Scan for needed files based on filename
            List<string> files = new();
            ScanForFilename(files, directories);

            // Get formatted file data
            ExtractDataFromFile(files);
        }

        private void ScanSubDirectories(List<string> directoriesRef, string directory)
        {
            directoriesRef.Add(directory);

            // Get any sub directories
            string[] subDirectories = Directory.GetDirectories(directory);

            // If there aren't any, go back
            if (subDirectories == null)
                return;

            // If there are, scan each of those for additional directories
            foreach (string subDirectory in subDirectories)
                ScanSubDirectories(directoriesRef, subDirectory);
        }

        private void ScanForFilename(List<string> filesRef, List<string> directoriesRef)
        {
            foreach (string directory in directoriesRef) // For each found directory
                foreach (string file in Directory.GetFiles(directory)) // Search for desired files
                    if (file == directory + filename)
                        filesRef.Add(file);
        }

        private void ExtractDataFromFile(List<string> filesRef)
        {
            StreamReader fileIn;
            List<string> dataBlock;
            string dataLine;

            foreach (string file in filesRef) { // For each file
                // Get all of its data
                fileIn = new(file);
                dataBlock = new();

                while ((dataLine = fileIn.ReadLine()) != null)
                    dataBlock.Add(dataLine);

                // Apply formatting rules
                data = ruleset.DynamicInvoke(dataBlock);
            }
        }
    }
}
