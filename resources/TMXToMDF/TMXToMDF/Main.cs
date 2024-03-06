using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
                    if ((stream = new StreamWriter(dialog.FileName)) != null) {
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

            string[] dataBlock;
            string data;

            // Get raw data
            while ((data = stream.ReadLine()) != null)
                rawData.Enqueue(data);

            // Format raw data by extracting useful elements
            while (rawData.Count != 0) {
                // TODO: Implement already existing code from Moonwalk
            }


            stream.Close();
            return formatData;
        }
    }
}
