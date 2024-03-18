using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TMXToMDF {
    public partial class Main : Form {
        private const int WindowWidth = 500;
        private const int WindowHeight = 500;

        // Provies the user with a general description of the program
        private Label description;

        // Functionality related getting the TMX file
        private Button pathInputButton;
        private TextBox pathInputTextBox;

        private Button convertButton;

        public Main() {
            InitializeComponent();

            // Window set up
            FormBorderStyle = FormBorderStyle.FixedSingle; // Prevent window resizing
            MaximizeBox = false;    // Disables the maximize button
            MinimizeBox = false;    // Disables the minimize button 

            Size = new Size(WindowWidth, WindowHeight); // Window Size

            Text = "TMX To MDF Converter";
            BackColor = Color.FromArgb( // Dark gray
                49,     // Red
                51,     // Green
                53      // Blue
                );


            // Set up description label
            description = new Label() {
                // Text content
                Font = new Font(FontFamily.GenericMonospace, 9),
                Text =
                    "Converts useful data inside of a TMX file to the MDF file format. " +
                    "Only works for the, non-encoded, JSON varient.\n" +
                    "Made by Nelson Rodriguez",

                // Text properties
                TextAlign = ContentAlignment.MiddleCenter, // Centers the text

                // Text appearance
                Location = new Point(10, 10),
                Size = new Size(460, 50),
                ForeColor = Color.White
            };
            Controls.Add(description);


            // Set up input path button
            pathInputButton = new Button() {
                // Text properties
                ForeColor = Color.White,
                Text = "...",

                // Button properties
                Location = new Point(40, 87),
                Size = new Size(25, 25)
            };
            pathInputButton.Click += OnPathInputButtonClick;
            Controls.Add(pathInputButton);


            // Set up input path text box
            pathInputTextBox = new TextBox() {
                ForeColor = Color.Black,    // Text color
                BackColor = Color.White,    // Box color

                // Text box properties
                Location = new Point(75, 90),
                Size = new Size(295, 30),
            };
            Controls.Add(pathInputTextBox);


            // Set up convert button
            convertButton = new Button() {
                // Text properties
                ForeColor = Color.White,
                Text = "Convert",

                // Button properties
                Location = new Point(380, 87),
                Size = new Size(60, 25),
            };
            convertButton.Click += OnConvertButtonClick;
            Controls.Add(convertButton);
        }


        private void OnPathInputButtonClick(object sender, EventArgs e) {
            // Open a File Explorer prompt to get file location
            using (OpenFileDialog dialog = new OpenFileDialog()) { // The keyword "using" makes it so component is discarded after use
                dialog.InitialDirectory = "c:/Users/%USERPROFILE%"; // Starts at the relative User path
                // BUG: Files with the TMX extension don't show when looking at only *.tmx files
                dialog.Filter = "tmx files (*.tmx)|*.txt|All files (*.*)|*.*";
                dialog.FilterIndex = 2; // idk what this does :)
                dialog.RestoreDirectory = true;

                // Once user selects a directory
                if (dialog.ShowDialog() == DialogResult.OK)
                    // Set pathInputTextBox's text to the chosen path
                    pathInputTextBox.Text = dialog.FileName;
            }
        }

        private void OnConvertButtonClick(object sender, EventArgs e) {
            // Open a File Explorer prompt to get path to save a file to
            using (SaveFileDialog dialog = new SaveFileDialog()) {
                StreamWriter stream;

                dialog.Filter = "tmx files (*.tmx)|*.txt|All files (*.*)|*.*";
                dialog.FilterIndex = 2;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                    if ((stream = new StreamWriter($"{dialog.FileName}")) != null) {
                        List<string> data = ExtractTMXData();

                        if (data == null) { // Data wasn't extracted successfully
                            stream.Close();
                            return;
                        }

                        foreach (string line in data) // Write data to file
                            stream.WriteLine(line);

                        stream.Close();
                    }
            }
        }

        private List<string> ExtractTMXData() {
            if (!File.Exists(pathInputTextBox.Text)) {
                // Error message
                return null;
            }

            StreamReader stream = new StreamReader(pathInputTextBox.Text); // Exception checking? Not today lmao

            Queue<string> rawData = new Queue<string>();
            List<string> formatData = new List<string>();

            MatchCollection parsedData;
            string[] dataBlock;
            string data;

            // Get raw data
            while ((data = stream.ReadLine()) != null)
                rawData.Enqueue(data);

            // First line contains TMX formatting data (not useful)
            rawData.Dequeue();

            // Second line contains general information about the overall tile structure
            // We care about the 5th/6th numerical values (tilewidth and tileheight)
            parsedData = Regex.Matches(rawData.Dequeue(), @"[.\d]+");
            dataBlock = parsedData.Cast<Match>().Select(match => match.Value).ToArray();

            formatData.Add($"SIZE={dataBlock[4]},{dataBlock[5]}");

            // Format raw data by extracting useful elements
            while (rawData.Count != 0) {
                
                // Tile data
                if (rawData.Peek().Contains("<layer")) {
                    // Ignore tmx file formatting
                    rawData.Dequeue();
                    rawData.Dequeue();

                    // Read tile data for each row
                    List<int[]> tileRows = new List<int[]>();
                    while (!rawData.Peek().Contains("</data>")) {
                        // Get relevant numerical data
                        parsedData = Regex.Matches(rawData.Dequeue(), @"[.\d]+");
                        dataBlock = parsedData.Cast<Match>().Select(match => match.Value).ToArray();

                        // Extract each tile ID from parsed data
                        int[] tileRow = new int[dataBlock.Length];
                        for (int i = 0; i < tileRow.Length; i++)
                            tileRow[i] = int.Parse(dataBlock[i]);

                        tileRows.Add(tileRow);
                    }

                    // Format data into a MDF form
                    formatData.Add("TILESTART");

                    foreach (int[] row in tileRows) {
                        // Creates a string formated like "ID,ID,ID,ID,ID ... ID,"
                        string line = "";
                        foreach (int ID in row)
                            line += $"{ID},";

                        line.Remove(line.Length - 1); // Removes the last, unneeded comma
                        formatData.Add(line);
                    }

                    formatData.Add("TILEEND");
                }

                else if (rawData.Peek().Contains("<objectgroup")) {
                    // Ignore tmx file formatting
                    rawData.Dequeue();

                    while (!rawData.Peek().Contains("</objectgroup")) {
                        // Get relevant numerical data
                        parsedData = Regex.Matches(rawData.Dequeue(), @"[.\d]+");
                        dataBlock = parsedData.Cast<Match>().Select(match => match.Value).ToArray();

                        // We care for the 2nd, 3rd, 4th, and 5th numeric values in the file
                        // Tiled doesn't allow for special geometry, therefore we assume all
                        // geometry is basic Terrain
                        formatData.Add($"Terrain=" +    
                            $"{(int)float.Parse(dataBlock[1])}," +  // Collision's relative X position
                            $"{(int)float.Parse(dataBlock[2])}," +  // Collision's relative Y position
                            $"{(int)float.Parse(dataBlock[3])}," +  // Collision's width
                            $"{(int)float.Parse(dataBlock[4])}");   // Collision's height
                    }
                }

                else // If it isn't one of the above, most likely tmx file formatting data
                    rawData.Dequeue();
            }


            stream.Close();
            return formatData;
        }
    }
}
