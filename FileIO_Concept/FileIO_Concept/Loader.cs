using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FileIO_Concept
{
    enum DataHeader
    {
        value_int,
        value_string
    }
    /// <summary>
    /// A static class to help with loading data from files
    /// </summary>
    internal static class Loader
    {
        /// <summary>
        /// Puts data from a file into a dictionary.
        /// This could be made into seperate methods for different kinds of files 
        /// and/or different things that need data (entity, level, etc.)
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns></returns>
        public static void Load(Type cl)
        {
            const string RootDirectory = "../../../";
            string dataPath = // Get relevant class file path
                $"{RootDirectory}{cl.GetProperty("FilePath").GetMethod}data.txt";

            // Get relevant class DataHeader enum

            Dictionary<DataHeader, string> _data = new Dictionary<DataHeader, string>();
            

            // For data reading
            StreamReader file = null;
            string fileDataRaw;
            string[] fileDataFormatted;

            // For data sorting
            Dictionary<string, string> dataBuffer = new Dictionary<string, string>();

            int line = 0; // For error logging purposes

            try
            {
                if (!File.Exists(dataPath)) // For error logging purposes
                    throw new FileNotFoundException();

                file = new StreamReader(dataPath);

                // Read file content
                // Error checking should be done here
                while ((fileDataRaw = file.ReadLine()) != null)
                {
                    line++;

                    if (fileDataRaw[0] == '/') // Ignore comments
                        continue;

                    // Check for improper line formatting
                    if (!fileDataRaw.Contains('=') ||
                        // Check for any file format errors that would prevent
                        // proper data formatting before this next line
                        (fileDataFormatted = fileDataRaw.Split('=')).Length != 2)
                        throw new MalformedLineException();

                    // Prepare data for sorting via enums
                    dataBuffer.Add(fileDataFormatted[0], fileDataFormatted[1]);
                }

                // Check to see if the right amount of data was loaded
                if (dataBuffer.Count != Enum.GetNames(typeof(DataHeader)).Length)
                    throw new FileLoadException();

                // Store read data via its respective enum
                foreach (DataHeader header in Enum.GetValues(typeof(DataHeader)))
                    _data.Add(
                        header,
                        dataBuffer[header.ToString()]);

            }
            // Add debug console call for FileNotFoundException
            catch
            {

            }
            finally
            {
                file?.Close(); // Closes file if successfully opened
            }
        }

    }
}
