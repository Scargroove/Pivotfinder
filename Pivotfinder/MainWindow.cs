using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Pivotfinder
{
    public partial class MainWindow : Form
    {
        private List<Image> imagesList = new List<Image>();
        private bool changeAlphaToFullTransparent = false;
        private bool changeAlphaToFullOpaque = false;
        private bool removePixel = false;
        private bool findCoordinates = false;
        private bool changeAlphaToNewColor = false;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private Color pivotColor = Color.FromArgb(255, 255, 0, 255);
        private int pivotColorInt = 0;
        private Color newAlphaColor = Color.FromArgb(255, 0, 0, 0);
        private Point spriteSize = new Point(128, 128);
        Point alphaRange = new Point(0, 0);
        List<string> messages = new List<string>();  // List to store messages for the log

        public MainWindow()
        {
            InitializeComponent();
            InitializeTooltips();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.AllowDrop = true;
        }

        #region UI Elements
        // Initializes tooltips for various controls in the main window.
        private void InitializeTooltips()
        {
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
            string ext = Path.GetExtension(fileName).ToLower();
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
                newAlphaColor = selectedColor;
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
                pivotColor = selectedColor;
            }
        }

        private void checkBoxSetNewAlphaColor_CheckedChanged(object sender, EventArgs e)
        {
            changeAlphaToNewColor = checkBoxSetNewAlphaColor.Checked;
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
                changeAlphaToFullTransparent = false;
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
                changeAlphaToFullTransparent = true;
                changeAlphaToFullOpaque = false;
                checkBoxChangeAlpha.Checked = false;
            }
            else
            {
                changeAlphaToFullTransparent = false;
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

        // Handles the closing of the main window and performs cleanup operations.
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
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
            StartOperation();
        }
        #endregion

        #region Logic
        // Starts all selected operations.
        private void StartOperation()
        {
            // Check if images exist and an operation has been selected
            if (imagesList.Count > 0 && (changeAlphaToFullOpaque || removePixel || findCoordinates || changeAlphaToFullTransparent || changeAlphaToNewColor))
            {
                progressBar.Maximum = imagesList.Count;
                progressBar.Value = 0;
                spriteSize = new Point((int)numericUpDownSpriteWidth.Value, (int)numericUpDownSpriteHeight.Value);
                alphaRange = new Point((int)numericUpDownAlphaFrom.Value, (int)numericUpDownAlphaTo.Value);
                pivotColorInt = pivotColor.ToArgb();
                messages = new List<string>();  // Resets the list

                // Iterate over each image in the list
                foreach (Image img in imagesList)
                {
                    string imagePath = img.Tag.ToString();  // Store the image path
                    try
                    {
                        Bitmap bitmap = new Bitmap(imagePath);  // Create a Bitmap object from the image path
                        bitmap.Tag = img.Tag;  // Set the bitmap's tag

                        stopwatch.Restart();  // Restart the stopwatch

                        // Perform pixel processing and collect coordinates
                        List<string> coordinates = ProccessSpritesheet(bitmap);

                        stopwatch.Stop();  // Stop the stopwatch

                        messages.Add(imagePath);
                        messages.Add("Processing time: " + stopwatch.ElapsedMilliseconds + " ms");
                        if (changeAlphaToFullTransparent || removePixel || changeAlphaToFullOpaque)
                        {
                            messages.Add("Image modified and saved successfully in: " + Path.Combine(Path.GetDirectoryName(imagePath), Path.GetFileNameWithoutExtension(imagePath) + "_modified.png"));
                        }

                        // Save coordinates if the option is selected and coordinates are found
                        else if (findCoordinates && coordinates.Any())
                        {
                            SaveCoordinatesToFile(imagePath, coordinates, messages);
                        }
                        messages.Add("");
                        bitmap.Dispose();  // Release the resources of the Bitmap object

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
            Application.Exit();
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
                    spriteSize = new Point((int)numericUpDownSpriteWidth.Value, (int)numericUpDownSpriteHeight.Value);

                    ColorDialog colorDialog = new ColorDialog();
                    int[] customColors = new int[] { ColorTranslator.ToOle(Color.FromArgb(255, 0, 255)) }; // Magenta as preset
                    colorDialog.CustomColors = customColors;
                    colorDialog.Color = Color.FromArgb(255, 0, 255); // Set default color to Magenta

                    // Display the dialog window to allow the user to change the color
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        pivotColor = colorDialog.Color; // Update the color to draw if the user selects a new one
                    }

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // Applies the color to the coordinates
                    SetPixelFromList(bitmap, coordinates);

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
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);

            // Create the path for the text file (same folder as the image file)
            string directory = Path.GetDirectoryName(imagePath);
            string textureFilePath = Path.Combine(directory, fileNameWithoutExtension + ".txt");

            try
            {
                // Open or create the text file to write coordinates
                using (StreamWriter file = new StreamWriter(textureFilePath))
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

        // Finds pixels of a specified color in a bitmap and can optionally remove these pixels.
        private List<string> ProccessSpritesheet(Bitmap bitmap)
        {
            // Ensure that the size of a single sprite is less than or equal to the size of the entire bitmap.
            if (spriteSize.X > bitmap.Width || spriteSize.Y > bitmap.Height)
            {
                throw new ArgumentException("The sprite size must not be larger than the spritesheet size.");
            }

            // Calculate the number of sprites horizontally and vertically on the spritesheet.
            Point spriteAmount = new Point(bitmap.Width / spriteSize.X, bitmap.Height / spriteSize.Y);

            List<string> resultsList = new List<string>();

            // Lock the bitmap in memory for quick access.
            // This ensures that the pixel data can be accessed and modified directly.
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height), // Define the rectangle area to lock.
                ImageLockMode.ReadWrite, // Specify that we will read and write to the bitmap.
                PixelFormat.Format32bppArgb // Use 32 bits per pixel format with alpha, red, green, and blue channels.
            );

            // Create a byte array to hold the pixel data.
            // The size of the array is determined by the stride (the width of a single row in bytes) and the height of the bitmap.
            byte[] pixelData = new byte[bitmapData.Stride * bitmapData.Height];

            // Copy the pixel data from the bitmap to the byte array.
            // Marshal.Copy is used to transfer data between unmanaged memory (bitmapData.Scan0) and managed memory (pixelData).
            Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);

            // Iterate over each sprite on the spritesheet.
            for (int y = 0; y < spriteAmount.Y; y++)
            {
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    // Find the position of the desired pixel within the sprite.
                    Point result = ProcessSprite(pixelData, bitmapData.Stride, new Point(spriteSize.X * x, spriteSize.Y * y), new Point(x, y), bitmap.Tag.ToString());
                    if (findCoordinates)
                    {
                        if (result.X != -1 && result.Y != -1)
                        {
                            if (result.X == 0 && result.Y == 0)
                            {
                                resultsList.Add(result.X + "," + result.Y);
                            }
                            else
                            {
                                resultsList.Add((result.X + 1) + "," + (result.Y + 1));
                            }
                        }
                    }
                }
            }

            // Change alpha values if required
            if (changeAlphaToFullOpaque || changeAlphaToFullTransparent || changeAlphaToNewColor)
            {
                ChangeAlpha(pixelData);
            }

            // Copy the modified pixel data back into the bitmap.
            Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            // Optionally, save the modified bitmap.
            if (removePixel || changeAlphaToFullOpaque || changeAlphaToFullTransparent || changeAlphaToNewColor)
            {
                SaveModifiedBitmap(bitmap);
            }

            // Release bitmap memory.
            bitmap.Dispose();
            return resultsList;
        }

        // Searches for the pixel with the target color in a single sprite.
        private Point ProcessSprite(byte[] pixelData, int stride, Point startPos, Point spritePosition, String savePath)
        {
            Point result = new Point(0, 0);
            int foundPixelCount = 0; // Counter for the number of found pixels.
            bool foundMultiplePixel = false;

            // Search each pixel within the sprite.
            for (int y = 0; y < spriteSize.Y; y++)
            {
                int rowStart = (startPos.Y + y) * stride + startPos.X * 4;

                for (int x = 0; x < spriteSize.X; x++)
                {
                    int position = rowStart + x * 4;
                    int color = BitConverter.ToInt32(pixelData, position);

                    if (color == pivotColorInt)
                    {
                        if (findCoordinates)
                        {
                            if (foundPixelCount > 0 && !foundMultiplePixel)
                            {
                                // Error: Multiple pixels found
                                messages.Add("!!! Multiple Pivots found in " + savePath + " in sprite: " + (spritePosition.X + 1) + "," + (spritePosition.Y + 1) + " saved the last found pivot.");
                                foundMultiplePixel = true;
                            }

                            result = new Point(x, y);
                        }

                        if (removePixel)
                        {
                            ReplacePixelWithDominantOrNearestAverageColor(pixelData, stride, position);
                        }
                        foundPixelCount++; // Increment the count for found pixels.
                    }
                }
            }
         return result;
        }

        // Saves the modified bitmap under a new filename.
        public static void SaveModifiedBitmap(Bitmap bitmap, string modifiedText = "_modified")
        {
            // Check if the bitmap tag contains a valid storage path
            if (bitmap.Tag == null || string.IsNullOrEmpty(bitmap.Tag.ToString()))
            {
                throw new InvalidOperationException("The bitmap tag does not contain a valid storage path.");
            }

            // Retrieve the original path of the image from the tag
            string originalPath = bitmap.Tag.ToString();
            // Get the directory of the original image
            string directory = Path.GetDirectoryName(originalPath);
            // Extract the filename without extension
            string filename = Path.GetFileNameWithoutExtension(originalPath);
            // Extract the file extension
            string extension = Path.GetExtension(originalPath);
            // Create the new filename with the additional text
            string newFileName = Path.Combine(directory, filename + modifiedText + extension);

            // Save the bitmap under the new filename as a PNG
            bitmap.Save(newFileName, ImageFormat.Png);
        }

        // Replaces the searched pixel with nearby colors or average colors if all are different.
        private void ReplacePixelWithDominantOrNearestAverageColor(byte[] pixelData, int stride, int position)
        {
            int[] neighboringColors = new int[4];
            int y = position / stride;
            int x = (position % stride) / 4;

            // Capture the colors of the surrounding pixels

            // Top
            if (y > 0)
            {
                neighboringColors[0] = BitConverter.ToInt32(pixelData, position - stride);
            }

            // Bottom
            if (y < spriteSize.Y - 1)
            {
                neighboringColors[1] = BitConverter.ToInt32(pixelData, position + stride);
            }

            // Left
            if (x > 0)
            {
                neighboringColors[2] = BitConverter.ToInt32(pixelData, position - 4);
            }

            // Right
            if (x < spriteSize.X - 1)
            {
                neighboringColors[3] = BitConverter.ToInt32(pixelData, position + 4);
            }


            // Filter out the target color from the surrounding colors
            var filteredColors = neighboringColors.Where(color => color != pivotColorInt).ToArray();

            // Group colors to determine the dominant color
            var groupedColors = filteredColors.GroupBy(color => color)
                                              .Select(group => new { Color = group.Key, Count = group.Count() })
                                              .OrderByDescending(group => group.Count);

            // Determine the dominant color if available
            var dominantColor = groupedColors.FirstOrDefault(group => group.Count >= 2);

            int colorToSet;
            if (dominantColor != null)
            {
                // Use the dominant color if one is found
                colorToSet = dominantColor.Color;
            }
            else
            {
                // No dominant color found, calculate the average and find the nearest color
                int averageColor = CalculateAverageColor(filteredColors);
                colorToSet = filteredColors.OrderBy(color => ColorDistance(averageColor, color)).FirstOrDefault();
            }

            // Set the selected color at the current pixel
            BitConverter.GetBytes(colorToSet).CopyTo(pixelData, position);
        }

        // Calculates the average value of colors in an array of integer color values.
        private static int CalculateAverageColor(int[] colors)
        {
            // Initialize sum of red, green, and blue values.
            int r = 0, g = 0, b = 0;
            // Counter for the number of processed colors.
            int count = 0;
            // Iterate over each color integer in the array.
            foreach (int color in colors)
            {
                // Extract the red component and add it to the sum.
                r += (color >> 16) & 0xFF;
                // Extract the green component and add it to the sum.
                g += (color >> 8) & 0xFF;
                // Extract the blue component and add it to the sum.
                b += color & 0xFF;
                // Increment the counter by one for each processed color.
                count++;
            }
            // Divide the sums by the number of colors to calculate the average.
            if (count > 0)
            {
                r /= count;
                g /= count;
                b /= count;
            }
            // Combine the average red, green, and blue values into a single color integer.
            return (r << 16) | (g << 8) | b;
        }

        // Calculates the distance between two colors based on their RGB values.
        private static int ColorDistance(int color1, int color2)
        {
            // Extract the RGB components of the first color.
            int r1 = (color1 >> 16) & 0xFF;
            int g1 = (color1 >> 8) & 0xFF;
            int b1 = color1 & 0xFF;

            // Extract the RGB components of the second color.
            int r2 = (color2 >> 16) & 0xFF;
            int g2 = (color2 >> 8) & 0xFF;
            int b2 = color2 & 0xFF;

            // Calculate the squared distance between the two colors.
            return (r1 - r2) * (r1 - r2) + (g1 - g2) * (g1 - g2) + (b1 - b2) * (b1 - b2);
        }

        // Changes alpha values of the image.
        private void ChangeAlpha(byte[] pixelData)
        {
            // Iterate over all pixels in the image
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                byte alpha = pixelData[i + 3];
                byte newAlpha;

                if (alpha >= alphaRange.X && alpha <= alphaRange.Y)
                {
                    if (changeAlphaToFullOpaque)
                    {
                        newAlpha = 255;
                    }
                    else if (changeAlphaToFullTransparent)
                    {
                        newAlpha = 0;
                    }
                    else
                    {
                        newAlpha = alpha;
                    }

                    if (changeAlphaToNewColor)
                    {
                        pixelData[i] = newAlphaColor.B;
                        pixelData[i + 1] = newAlphaColor.G;
                        pixelData[i + 2] = newAlphaColor.R;
                    }

                    pixelData[i + 3] = newAlpha;
                }
            }
        }

        // Sets pixels in the desired color at coordinates specified in a list.
        public void SetPixelFromList(Bitmap bitmap, List<Point> points)
        {
            // Determine the size of the spritesheet
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Lock bitmap data for read and write access
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Get the address of the first line
            IntPtr ptr = bitmapData.Scan0;

            // Declare an array to hold the bitmap bytes
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            int depth = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8; // Bytes per pixel

            // Calculate the number of sprites
            Point spriteAmount = new Point(bitmap.Width / spriteSize.X, bitmap.Height / spriteSize.Y);
            int pointListPosition = 0;

            // Loop over each row in the sprite grid
            for (int y = 0; y < spriteAmount.Y; y++)
            {
                // Loop over each column in the current grid row
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    // Check if the point is not at the origin (0,0)
                    if (points[pointListPosition].X != 0 && points[pointListPosition].Y != 0)
                    {
                        // Calculate the X-coordinate of the pixel in the bitmap
                        int pixelX = (spriteSize.X * x) + points[pointListPosition].X;

                        // Calculate the Y-coordinate of the pixel in the bitmap
                        int pixelY = (spriteSize.Y * y) + points[pointListPosition].Y;

                        // Offset
                        pixelX = pixelX - 1;
                        pixelY = pixelY - 1;

                        // Calculate the pixel position in the one-dimensional array `rgbValues` that contains the bitmap data
                        int position = (pixelY * bitmapData.Stride) + (pixelX * depth);

                        // Set the blue component of the pixel
                        rgbValues[position] = pivotColor.B;

                        // Set the green component of the pixel
                        rgbValues[position + 1] = pivotColor.G;

                        // Set the red component of the pixel
                        rgbValues[position + 2] = pivotColor.R;

                        // Check if the image format is 32 bits per pixel (RGBA) to set the alpha value
                        if (depth == 4)
                        {
                            // Set the alpha value (transparency) of the pixel
                            rgbValues[position + 3] = this.pivotColor.A;
                        }
                    }

                    // Increase the position in the list of points
                    pointListPosition++;
                }
            }

            // Copy the RGB values back into the bitmap
            Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock bitmap data
            bitmap.UnlockBits(bitmapData);
        }
        #endregion
    }
}
