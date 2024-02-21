using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonwalk.Classes.Managers {
    internal static class Loader {
        /*
        TODO:
            - Implement fallback mechanics in case a file can't be open doesn't exist.
            This fallback should lead to a folder containing sprites that exist to fill
            content that otherwise is missing (i.e. think of what source does when missing a texture)
        */

        private static ContentManager content = null;

        public static ContentManager Content {
            set {
                if (content == null)
                    content = value;
            }
        }

        /// <summary>
        /// Searches, formats, and returns specific file data
        /// </summary>
        /// <param name="rootDirectory">Initial place to start searching for files</param>
        /// <param name="fileType">File type to search for</param>
        /// <param name="decodeRules">Rules on how to format the raw file data</param>
        /// <returns>Formatted file data</returns>
        public static Object ProcessFileData(
                string rootDirectory,
                string fileType,
                Delegate decodeRules) {
            // Get needed sub directories
            List<string> subDirectories = new();
            ScanSubDirectories(rootDirectory, subDirectories);

            // Find desired files
            List<string> files = ScanForFileType(fileType, subDirectories);

            // Get raw data from files
            List<List<string>> rawFileData = GetRawFileData(files);

            // Apply file decoding rules
            return DecodeFileData(rawFileData, decodeRules);
        }

        public static Object ProcessSpriteData(
                string rootDirectory,
                Delegate formatRules) {
            // Get needed sub directories
            List<string> subDirectories = new();
            ScanSubDirectories(rootDirectory, subDirectories);

            // Find desired png files
            List<string> files = ScanForFileType("png", subDirectories);

            // Get sprites and apply formatting rules
            return LoadAndFormatSprites(files, formatRules);
        }

        /// <summary>
        /// Searches and records any subdirectories in a given directory
        /// </summary>
        /// <param name="directory">Directory to search in</param>
        /// <param name="directoriesRef">Reference to a list to store these directories paths</param>
        private static void ScanSubDirectories(string directory, List<string> directoriesRef) {
            directoriesRef.Add(directory); // A reference used instead of a return type
                                           // since this methods uses recursion

            // Scan for sub directories
            string[] subDirectories = Directory.GetDirectories(directory);

            // If there aren't any, go back
            if (subDirectories == null)
                return;

            // If there are, scan each of those for additional directories
            foreach (string subDirectory in subDirectories)
                ScanSubDirectories(subDirectory, directoriesRef);
        }

        /// <summary>
        /// Scans for any files with a specific type in several directories
        /// </summary>
        /// <param name="fileType">File type to search for</param>
        /// <param name="directories">Directories to search through</param>
        /// <returns>Paths to each file that has the respective file type</returns>
        private static List<string> ScanForFileType(string fileType, List<string> directories) {
            List<string> files = new();

            foreach (string directory in directories)                  // For each found directory
                foreach (string file in Directory.GetFiles(directory)) // Search for desired files
                    if ((file.Remove(0, directory.Length - 1))  // Get filename
                            .Split('.')[1]                      // Get file ending
                            == fileType)                        // Check if wanted file type
                        files.Add(file);

            return files;
        }

        /// <summary>
        /// Searches through provided file paths and gathers their raw text data
        /// </summary>
        /// <param name="files">Files to read</param>
        /// <returns>A collection of gathered raw data</returns>
        private static List<List<string>> GetRawFileData(List<string> files) {
            StreamReader reader;
            List<List<string>> _data = new();
            string incomingData;

            // Begin scanning each file
            foreach (string file in files) {
                reader = new StreamReader(file);
                List<string> data = new();

                // Get all file contents
                while ((incomingData = reader.ReadLine()) != null)
                    data.Add(incomingData);

                _data.Add(data);
            }

            return _data;
        }

        /// <summary>
        /// Decodes raw file data with a set of provided "rules"
        /// </summary>
        /// <param name="rawFileData">Data to decode</param>
        /// <param name="decodeRules">Set of "rules" to follow</param>
        /// <returns>Decoded object of data</returns>
        private static Object DecodeFileData(
                List<List<string>> rawFileData,
                Delegate decodeRules) {
            return decodeRules.DynamicInvoke(rawFileData);
        }

        /// <summary>
        /// Formats sprites with a set of provided "rules"
        /// </summary>
        /// <param name="files">Sprites to load</param>
        /// <param name="formatRules">Set of "rules" to follow</param>
        /// <returns></returns>
        private static Object LoadAndFormatSprites(
                List<string> files,
                Delegate formatRules) {
            return formatRules.DynamicInvoke(content, files);
        }
    }
}
