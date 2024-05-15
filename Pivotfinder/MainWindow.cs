using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Pixelfinder
{
    public partial class MainWindow : Form
    {
        private List<Image> imagesList = new List<Image>();
        private bool changeAplhaToFullTransparent = false;
        private bool changeAlphaToFullOpaque = false;
        private bool removePixel = false;
        private bool findCoordinates = false;
        private bool setNewAlphaColor = false;
        private Stopwatch stopwatch = new Stopwatch();
        private Color targetColor = Color.FromArgb(255, 255, 0, 255);
        private Color changeAlphaTo = Color.FromArgb(255, 0, 0, 0);

        // Initializes the main window and GUI components.
        public MainWindow()
        {
            InitializeComponent();
            InitializeTooltips();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.AllowDrop = true;
        }

        // Configures tooltips for various controls in the main window.
        private void InitializeTooltips()
        {
            // Configure the ToolTip
            toolTip1.AutoPopDelay = 5000;    // How long the tooltip remains visible
            toolTip1.InitialDelay = 1000;    // Delay before the tooltip appears
            toolTip1.ReshowDelay = 500;      // Delay before the tooltip reappears after first display
            toolTip1.ShowAlways = true;      // Show the tooltip even if the form is not active

            // Set the tooltip text
            toolTip1.SetToolTip(this.checkBoxChangeAlpha, "Change pixels with alpha values in the scope to be fully opaque. Users can select a different color if preferred.");
            toolTip1.SetToolTip(this.checkBoxRemoveAlpha, "Change pixels with alpha values in the scope to be fully transparent. Users can select a different color if preferred.");
            toolTip1.SetToolTip(this.checkBoxFindPivots, "Identify pivot points using the pivot color and the sprite size. Users can select a different color or sprite size if preferred.");
            toolTip1.SetToolTip(this.checkBoxRemovePivot, "Removes the pivot-point by changing it to the color of the dominant neighboring pixel.");
            toolTip1.SetToolTip(this.buttonPivotToSpriteSheet, "Draws the pivots from a .txt file onto an image, using the specified sprite size and pivot color options.");
            toolTip1.SetToolTip(this.buttonSelectPivotColor, "Selects the color to identify the pivot points.");
            toolTip1.SetToolTip(this.buttonAlphaToColor, "Changes the alpha values in scope to a color if selected.");
        }

        // Handles keyboard inputs, particularly the deletion of images from the list.
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Handle image deletion when the delete key is pressed
            if (e.KeyCode == Keys.Delete && listBox.SelectedItems.Count > 0)
            {
                RemoveSelectedImages();
                e.Handled = true;  // Marks the event as handled
            }
            // Handle selecting all items when Ctrl+A is pressed
            else if (e.Control && e.KeyCode == Keys.A)
            {
                for (int i = 0; i < listBox.Items.Count; i++)
                    listBox.SetSelected(i, true);

                e.Handled = true;  // Marks the event as handled
            }
        }

        // Handles the event when files are dragged into the form.
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            HandleDragEnter(e);
        }

        // Handles the event when dragged files leave the form.
        private void Form1_DragLeave(object sender, EventArgs e)
        {
            listBox.BackColor = SystemColors.Window;
        }

        // Handles dropping files into the main window after dragging.
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
            listBox.BackColor = SystemColors.Window;
        }

        // Handles dragging files over the ListBox.
        private void listBox_DragEnter(object sender, DragEventArgs e)
        {
            HandleDragEnter(e);
        }

        // Handles dragged files leaving the ListBox.
        private void listBox_DragLeave(object sender, EventArgs e)
        {
            listBox.BackColor = SystemColors.Window;
        }

        // Handles dropping files into the ListBox.
        private void listBox_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
            listBox.BackColor = SystemColors.Window;
        }

        // Handles dragging files over the PictureBox control.
        private void PictureBox_DragEnter(object sender, DragEventArgs e)
        {
            HandleDragEnter(e);
        }

        // Handles dragged files leaving the PictureBox.
        private void PictureBox_DragLeave(object sender, EventArgs e)
        {
            listBox.BackColor = SystemColors.Window;
        }

        // Handles dropping files into the PictureBox.
        private void PictureBox_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
            listBox.BackColor = SystemColors.Window;
        }

        // Checks the dragged data and determines if it can be accepted.
        private void HandleDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                bool allFilesAreImages = true;
                foreach (string file in files)
                {
                    if (!IsImageFile(file))
                    {
                        allFilesAreImages = false;
                        break;
                    }
                }

                if (allFilesAreImages)
                {
                    listBox.BackColor = Color.LightGray; // Change background color if all files are images
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    listBox.BackColor = SystemColors.Window; // Reset background if at least one invalid file is found
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        // Processes file dropping into the main window and loads images.
        private void HandleDragDrop(DragEventArgs e)
        {
            // Load the dragged data as images
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            LoadImages(files);

            // Return focus to the main window
            this.Activate();
        }

        // Checks if a file has a supported image format.
        private bool IsImageFile(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp";
        }

        // Checks if an image is already present in the list.
        private bool IsImageDuplicate(string filePath)
        {
            foreach (Image img in imagesList)
            {
                // Check if the file path is already present in the list
                if (filePath.Equals((string)img.Tag))
                {
                    MessageBox.Show("The image with the same name and path already exists.");
                    return true;
                }
            }
            return false;
        }

        // Displays the selected image in the PictureBox.
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                // Retrieve the selected image from the ListBox
                Image selectedImage = imagesList[listBox.SelectedIndex];

                // Display the image in the PictureBox
                pictureBox.Image = selectedImage;
            }
        }

        // Deletes the selected image from the list.
        private void buttonDeleteListItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedImages();
        }

        // Opens a dialog to select a color to use for alpha values.
        private void buttonAlphaToColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialogAlpha = new ColorDialog();

            // Define a custom color as RGB value 255, 0, 0, 0 (Black)
            int[] customColors = new int[] { ColorTranslator.ToOle(Color.FromArgb(255, 0, 0, 0)) };
            colorDialogAlpha.CustomColors = customColors;

            // Set the initially selected color to Black
            colorDialogAlpha.Color = Color.FromArgb(255, 0, 0, 0);

            if (colorDialogAlpha.ShowDialog() == DialogResult.OK)
            {
                // Use the selected color
                Color selectedColor = colorDialogAlpha.Color;
                changeAlphaTo = selectedColor;
                checkBoxSetNewAlphaColor.Checked = true;
            }
        }

        // Opens a dialog to select a color to use as the target color for pixel operations.
        private void buttonSelectPixelColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            // Define a custom color as RGB value 255, 0, 255 (Magenta)
            int[] customColors = new int[] { ColorTranslator.ToOle(Color.FromArgb(255, 0, 255)) };
            colorDialog.CustomColors = customColors;

            // Set the initially selected color to Magenta
            colorDialog.Color = Color.FromArgb(255, 0, 255);

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Use the selected color
                Color selectedColor = colorDialog.Color;
                targetColor = selectedColor;
            }
        }

        private void checkBoxSetNewAlphaColor_CheckedChanged(object sender, EventArgs e)
        {
            setNewAlphaColor = checkBoxSetNewAlphaColor.Checked;
        }

        // Enables or disables pixel removal based on the state of the corresponding checkbox.
        private void checkBoxRemovePixel_CheckStateChanged(object sender, EventArgs e)
        {
            removePixel = checkBoxRemovePivot.Checked;
        }

        // Enables or disables changing of alpha values based on the state of the corresponding checkbox.
        private void checkBoxChangeAlpha_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxChangeAlpha.Checked)
            {
                changeAlphaToFullOpaque = true;
                changeAplhaToFullTransparent = false;
                checkBoxRemoveAlpha.Checked = false;
            }
            else
            {
                changeAlphaToFullOpaque = false;
            }
        }

        // Enables or disables removal of alpha values based on the state of the corresponding checkbox.
        private void checkBoxRemoveAlpha_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRemoveAlpha.Checked)
            {
                changeAplhaToFullTransparent = true;
                changeAlphaToFullOpaque = false;
                checkBoxChangeAlpha.Checked = false;
            }
            else
            {
                changeAplhaToFullTransparent = false;
            }
        }

        // Enables or disables finding coordinates based on the state of the corresponding checkbox.
        private void checkBoxFindCoordinates_CheckedChanged(object sender, EventArgs e)
        {
            findCoordinates = checkBoxFindPivots.Checked;
        }

        // Opens a dialog to select images to add to the list.
        private void buttonAddListItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.Multiselect = true; // Allows selecting multiple files

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Load the selected images
                LoadImages(openFileDialog.FileNames);
            }
        }

        // Exits the application.
        private void buttonExit_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        // Applies pixel operations to an image based on a list of coordinates.
        private void buttonCoordinatesToSpriteSheet_Click(object sender, EventArgs e)
        {
            ApplyPixelsFromFile();
        }

        // Starts image processing based on the selected options.
        private void startOperation_Click(object sender, EventArgs e)
        {
            // Check if images exist and an operation has been selected
            if (imagesList.Count > 0 && (changeAlphaToFullOpaque || removePixel || findCoordinates || changeAplhaToFullTransparent))
            {
                progressBar.Maximum = imagesList.Count;
                progressBar.Value = 0;

                Point spriteSize = new Point((int)numericUpDownSpriteWidth.Value, (int)numericUpDownSpriteHeight.Value);
                Point alphaRange = new Point((int)numericUpDownAlphaFrom.Value, (int)numericUpDownAlphaTo.Value);
                List<string> messages = new List<string>();  // List to store messages for the log

                // Iterate over each image in the list
                foreach (Image img in imagesList)
                {
                    string imagePath = img.Tag.ToString();  // Store the image path
                    try
                    {
                        Bitmap bitmap = new Bitmap(imagePath);  // Create a Bitmap object from the image path
                        bitmap.Tag = img.Tag;  // Set the bitmap's tag

                        stopwatch.Restart();  // Restart the stopwatch

                        // Perform pixel processing and collect coordinates and error image paths
                        var (coordinates, errorImagePaths) = ModifyImage.FindPixelInSpriteSheet(bitmap, spriteSize, targetColor, changeAlphaTo, removePixel, changeAlphaToFullOpaque, findCoordinates, changeAplhaToFullTransparent, setNewAlphaColor, alphaRange);

                        stopwatch.Stop();  // Stop the stopwatch

                        string modifiedPath = "Image modified and saved successfully in: " + Path.Combine(Path.GetDirectoryName(imagePath), Path.GetFileNameWithoutExtension(imagePath) + "_modified.png");
                        string message = $"Processing time: {stopwatch.ElapsedMilliseconds} ms";
                        messages.Add(imagePath);
                        messages.Add(message);  // Add the message to the list
                        if (changeAplhaToFullTransparent || removePixel || changeAlphaToFullOpaque)
                        {
                            messages.Add(modifiedPath);  // Add the message to the list
                        }

                        // Add error messages to the list if error image paths are present
                        if (errorImagePaths.Any() && findCoordinates)
                        {
                            foreach (var errorPath in errorImagePaths)
                            {
                                messages.Add($"Error: Multiple pivots found in: {errorPath}, saving pivots in text file canceled.");
                            }
                        }
                        // Save coordinates if the option is selected and coordinates are found
                        else if (findCoordinates && coordinates.Any())
                        {
                            SaveCoordinatesToFile(imagePath, coordinates, messages);
                        }
                        messages.Add("");
                        bitmap.Dispose();  // Release the resources of the Bitmap object

                        // Update the progress bar
                        progressBar.Value++;
                    }
                    catch (Exception ex)
                    {
                        messages.Add($"Error loading image: {imagePath}. Error: {ex.Message}");
                    }
                }

                messages.Add("Operations finished.");  // Add a completion message

                LogForm logForm = new LogForm();  // Create a new LogForm object
                logForm.AddMessagesToListBox(messages);  // Add the messages to the ListBox
                logForm.ShowDialog();  // Display the LogForm window
                progressBar.Value = 0;
            }
            else
            {
                // Display error messages if no images are present or no option is selected
                if (imagesList.Count == 0)
                {
                    MessageBox.Show("There are no images in the list.");
                }
                else
                {
                    MessageBox.Show("Please select an option.");
                }
            }
        }

        // Adds an image to the PictureBox and the internal image list.
        private void AddImageToPictureBoxAndList(Image img)
        {
            // Display the image in the PictureBox
            pictureBox.Image = img;

            // Hide the background image (set to null)
            pictureBox.BackgroundImage = null;

            // Add the image to the list
            imagesList.Add(img);

            // Update the ListBox to show the list of added images
            UpdateListBox();
        }

        // Updates the ListBox to display the current list of images.
        private void UpdateListBox()
        {
            // Clear the ListBox and re-add the list of images
            listBox.Items.Clear();
            foreach (Image img in imagesList)
            {
                // Retrieve the image's file path and add it to the ListBox
                listBox.Items.Add((string)img.Tag);
            }
        }

        // Removes the selected image from the ListBox and the internal image list.
        private void RemoveSelectedImages()
        {
            // Create a list to store the images to be deleted
            var selectedItems = listBox.SelectedItems.Cast<string>().ToList();

            if (selectedItems.Count > 0)
            {
                foreach (var selectedItem in selectedItems)
                {
                    // Find the image by tag
                    var imageToRemove = imagesList.FirstOrDefault(img => img.Tag.ToString() == selectedItem);
                    if (imageToRemove != null)
                    {
                        imageToRemove.Dispose(); // Release resources
                        imagesList.Remove(imageToRemove); // Remove from the list
                    }
                }
            }
            else if (imagesList.Any())
            {
                // No image selected, delete the last added image
                var lastAddedImage = imagesList.LastOrDefault();
                if (lastAddedImage != null)
                {
                    lastAddedImage.Dispose();
                    imagesList.Remove(lastAddedImage);
                }
            }

            // Update the ListBox
            UpdateListBox();

            // Reset the PictureBox, update it, or display the last image in the list
            if (imagesList.Count == 0)
            {
                pictureBox.Image = null;
                pictureBox.BackgroundImage = Pivotfinder.Properties.Resources.icon;
                pictureBox.BackgroundImageLayout = ImageLayout.Center;
            }
            else
            {
                // Display the last remaining image in the list
                pictureBox.Image = imagesList.Last();
            }
        }

        // Loads images from the specified file paths into memory and adds them to the application.
        private void LoadImages(string[] fileNames)
        {
            foreach (string file in fileNames)
            {
                try
                {
                    if (!IsImageDuplicate(file))
                    {
                        // Open a file stream for reading the image file
                        using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            // Copy image data into a MemoryStream
                            using (MemoryStream ms = new MemoryStream())
                            {
                                fs.CopyTo(ms);
                                ms.Seek(0, SeekOrigin.Begin); // Rewind the stream to the beginning

                                // Load the image from the MemoryStream
                                Image img = Image.FromStream(ms);

                                // Assign the file path to the Tag property for later reference
                                img.Tag = file;

                                // Add the image to the PictureBox and the list
                                AddImageToPictureBoxAndList(img);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }

        // Handles the closing of the main window and performs cleanup operations.
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitApplication();
        }

        // Takes the necessary steps to properly exit the application.
        private void ExitApplication()
        {
            // Release all images in the imagesList
            foreach (Image img in imagesList)
            {
                img.Dispose();
            }
            imagesList.Clear();

            // Exit the program
            System.Windows.Forms.Application.Exit();
        }

        // Applies pixel changes to an image based on a file with coordinates.
        public void ApplyPixelsFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images (*.png)|*.png";
            openFileDialog.Title = "Select an Image File";

            // Opens the dialog window for the image file
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string bitmapPath = openFileDialog.FileName;
                Bitmap bitmap = new Bitmap(bitmapPath);

                openFileDialog.Filter = "Text Files (*.txt)|*.txt";
                openFileDialog.Title = "Select a Text File with pivot-points.";

                // Opens the dialog window for the text file containing coordinates
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string textFilePath = openFileDialog.FileName;
                    List<Point> coordinates = ReadCoordinatesFromTextFile(textFilePath);

                    // Set the sprite size
                    Point spriteSize = new Point((int)numericUpDownSpriteWidth.Value, (int)numericUpDownSpriteHeight.Value);

                    ColorDialog colorDialog = new ColorDialog();
                    int[] customColors = new int[] { ColorTranslator.ToOle(Color.FromArgb(255, 0, 255)) }; // Magenta as preset
                    colorDialog.CustomColors = customColors;
                    colorDialog.Color = Color.FromArgb(255, 0, 255); // Set default color to Magenta

                    // Initialize colorToDraw with a default value
                    Color colorToDraw = Color.FromArgb(255, 0, 255);

                    // Display the dialog window to allow the user to change the color
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        colorToDraw = colorDialog.Color; // Update the color to draw if the user selects a new one
                    }

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // Applies the color to the coordinates
                    ModifyImage.SetPixelFromList(bitmap, spriteSize, colorToDraw, coordinates);

                    stopwatch.Stop();
                    long processingTime = stopwatch.ElapsedMilliseconds;

                    // Saves the image
                    string modifiedPath = Path.Combine(Path.GetDirectoryName(bitmapPath), "pivots_" + Path.GetFileName(bitmapPath));
                    bitmap.Save(modifiedPath);
                    bitmap.Dispose();

                    List<string> messages = new List<string>
            {
                $"Image modified and saved successfully at {modifiedPath}.",
                $"Processing time: {processingTime} ms"
            };

                    LogForm logForm = new LogForm();
                    logForm.AddMessagesToListBox(messages);
                    logForm.ShowDialog();
                }
            }
        }

        // Saves coordinates to a file.
        private void SaveCoordinatesToFile(string imagePath, List<string> coordinates, List<string> messages)
        {
            // Extract the image's filename without extension
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(imagePath);

            // Create the path for the text file (same folder as the image file)
            string directory = System.IO.Path.GetDirectoryName(imagePath);
            string textureFilePath = System.IO.Path.Combine(directory, fileNameWithoutExtension + ".txt");

            try
            {
                // Open or create the text file to write coordinates
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(textureFilePath))
                {
                    // Write the coordinates to the text file
                    foreach (string coordinate in coordinates)
                    {
                        file.WriteLine(coordinate);
                    }
                }

                // Add a success message to the list
                messages.Add($"Coordinates saved in {textureFilePath}.");
            }
            catch (Exception ex)
            {
                // Add an error message to the list if saving fails
                messages.Add($"Error saving coordinates to {textureFilePath}: {ex.Message}");
            }
        }

        // Reads coordinate points from a text file.
        private static List<Point> ReadCoordinatesFromTextFile(string filePath)
        {
            List<Point> coordinates = new List<Point>();  // Create a list for coordinate points
            string[] lines = File.ReadAllLines(filePath); // Read all lines from the file

            // Iterate over each line in the file
            foreach (string line in lines)
            {
                string[] parts = line.Trim().Split(','); // Split the line by commas and remove whitespace
                if (parts.Length == 2) // Check if the line has exactly two parts
                {
                    // Try to interpret the parts of the line as coordinates x and y
                    if (int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                    {
                        coordinates.Add(new Point(x, y)); // Add the point to the list
                        Console.WriteLine("coords: " + x + "," + y);  // Output the coordinates to the console
                    }
                }
            }

            return coordinates; // Return the list of coordinates
        }
    }
}
