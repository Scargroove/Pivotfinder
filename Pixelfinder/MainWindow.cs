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
        private bool removeAlpha = false;
        private bool changeAlpha = false;
        private bool removePixel = false;
        private bool findCoordinates = false;
        private Stopwatch stopwatch = new Stopwatch();
        private Color targetColor = Color.FromArgb(255, 255, 0, 255);
        private Color changeAlphaTo = Color.FromArgb(255, 0, 0, 0);


        // Initialisiert das Hauptfenster und die GUI-Komponenten.
        public MainWindow()
        {
            InitializeComponent();
            InitializeTooltips();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.AllowDrop = true;

        }

        // Konfiguriert Tooltips für verschiedene Steuerelemente im Hauptfenster.
        private void InitializeTooltips()
        {
            // Konfigurieren des ToolTip
            toolTip1.AutoPopDelay = 5000;    // Wie lange der Tooltip angezeigt wird
            toolTip1.InitialDelay = 1000;    // Wie lange bis der Tooltip erscheint
            toolTip1.ReshowDelay = 500;      // Wie lange bis der Tooltip nach dem ersten Anzeigen erneut angezeigt wird
            toolTip1.ShowAlways = true;      // Der Tooltip wird auch dann angezeigt, wenn das Formular nicht aktiv ist

            // Tooltip-Text festlegen

            toolTip1.SetToolTip(this.checkBoxChangeAlpha, "Change pixels with alpha values from 1 to 254 to black (default color with RGB 0, 0, 0). Users can select a different color if preferred.");
            toolTip1.SetToolTip(this.checkBoxRemoveAlpha, "Make pixels with alpha values from 1 to 254 fully transparent.");
            toolTip1.SetToolTip(this.checkBoxFindPivots, "Identify pivot points using magenta (default color with RGB 255, 0, 255) and the sprite size (default size 128x128). Users can select a different color or sprite size if preferred.");
            toolTip1.SetToolTip(this.checkBoxRemovePivot, "Removes the pivot-point by changing it to the color of the dominant neighboring pixel.");
            toolTip1.SetToolTip(this.buttonPivotToSpriteSheet, "Draws the pivots from a .txt file onto an image, using the specified sprite size and pivot color options.");
            toolTip1.SetToolTip(this.buttonSelectPivotColor, "Selects the color to identify the pivot points.");

        }

        // Behandelt Tastatureingaben, insbesondere das Löschen von Bildern aus der Liste.
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedImages();
                e.Handled = true;  // Markiert das Ereignis als behandelt
            }
        }

        // Behandelt das Ereignis, wenn Dateien in das Formular gezogen werden.
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {


            HandleDragEnter(e);

        }

        // Behandelt das Ereignis, wenn gezogene Dateien das Formular verlassen.
        private void Form1_DragLeave(object sender, EventArgs e)
        {
            listBox.BackColor = SystemColors.Window;

        }

        // Behandelt das Ablegen von Dateien im Hauptfenster nach dem Ziehen.
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
            listBox.BackColor = SystemColors.Window;

        }

        // Behandelt das Ziehen von Dateien über die ListBox.
        private void listBox_DragEnter(object sender, DragEventArgs e)
        {

            HandleDragEnter(e);

        }

        // Behandelt das Verlassen gezogener Dateien aus der ListBox
        private void listBox_DragLeave(object sender, EventArgs e)
        {
            listBox.BackColor = SystemColors.Window;

        }

        // Behandelt das Ablegen von Dateien in der ListBox.
        private void listBox_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
            listBox.BackColor = SystemColors.Window;

        }

        // Behandelt das Ziehen von Dateien über das PictureBox-Steuerelement.
        private void PictureBox_DragEnter(object sender, DragEventArgs e)
        {

            HandleDragEnter(e);
        }

        // Behandelt das Verlassen gezogener Dateien aus dem PictureBox.
        private void PictureBox_DragLeave(object sender, EventArgs e)
        {
            listBox.BackColor = SystemColors.Window;

        }

        // Behandelt das Ablegen von Dateien im PictureBox.
        private void PictureBox_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
            listBox.BackColor = SystemColors.Window;

        }

        // Prüft die Ziehdaten und bestimmt, ob sie akzeptiert werden können.
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
                    listBox.BackColor = Color.LightGray; // Hintergrund färben, wenn alle Dateien Bilder sind
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    listBox.BackColor = SystemColors.Window; // Hintergrund zurücksetzen, wenn mindestens eine ungültige Datei gefunden wurde
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        // Verarbeitet das Ablegen von Dateien im Hauptfenster und lädt die Bilder.
        private void HandleDragDrop(DragEventArgs e)
        {
            // Die gezogenen Daten als Bild laden
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            LoadImages(files);

            // Fokus an das Hauptfenster zurückgeben
            this.Activate();
        }

        // Überprüft, ob eine Datei ein unterstütztes Bildformat hat.
        private bool IsImageFile(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".bmp";
        }

        // Überprüft, ob ein Bild bereits in der Liste vorhanden ist.
        private bool IsImageDuplicate(string filePath)
        {
            foreach (Image img in imagesList)
            {
                // Überprüfen, ob der Dateipfad bereits in der Liste vorhanden ist
                if (filePath.Equals((string)img.Tag))
                {
                    MessageBox.Show("The image with the same name and path already exists.");
                    return true;
                }
            }
            return false;
        }

        // Zeigt das ausgewählte Bild in der PictureBox an.
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                // Das ausgewählte Bild aus der ListBox abrufen
                Image selectedImage = imagesList[listBox.SelectedIndex];

                // Bild in die PictureBox anzeigen
                pictureBox.Image = selectedImage;
            }
        }

        // Löscht das ausgewählte Bild aus der Liste.
        private void buttonDeleteListItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedImages();
        }

        // Öffnet einen Dialog zur Auswahl einer Farbe, die für Alpha-Werte verwendet werden soll.
        private void buttonAlphaToColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialogAlpha = new ColorDialog();

            // Definieren einer benutzerdefinierten Farbe als RGB-Wert 255, 0, 0, 0 (Schwarz)
            int[] customColors = new int[] { ColorTranslator.ToOle(Color.FromArgb(255, 0, 0, 0)) };
            colorDialogAlpha.CustomColors = customColors;

            // Setzen der initial ausgewählten Farbe auf Schwarz
            colorDialogAlpha.Color = Color.FromArgb(255, 0, 0, 0);

            if (colorDialogAlpha.ShowDialog() == DialogResult.OK)
            {
                // Die ausgewählte Farbe verwenden
                Color selectedColor = colorDialogAlpha.Color;
                changeAlphaTo = selectedColor;
                checkBoxChangeAlpha.Checked = true;
            }
        }

        // Öffnet einen Dialog zur Auswahl einer Farbe, die als Zielfarbe für Pixeloperationen dient.
        private void buttonSelectPixelColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            // Definieren einer benutzerdefinierten Farbe als RGB-Wert 255, 0, 255 (Magenta)
            int[] customColors = new int[] { ColorTranslator.ToOle(Color.FromArgb(255, 0, 255)) };
            colorDialog.CustomColors = customColors;

            // Setzen der initial ausgewählten Farbe auf Magenta
            colorDialog.Color = Color.FromArgb(255, 0, 255);

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Die ausgewählte Farbe verwenden
                Color selectedColor = colorDialog.Color;
                targetColor = selectedColor;

            }
        }

        // Aktiviert oder deaktiviert die Entfernung von Pixeln basierend auf dem Zustand des entsprechenden Kontrollkästchens.
        private void checkBoxRemovePixel_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxRemovePivot.Checked)
            {
                removePixel = true;
            }
            else
            {
                removePixel = false;
            }
        }

        // Aktiviert oder deaktiviert die Änderung von Alpha-Werten basierend auf dem Zustand des entsprechenden Kontrollkästchens.
        private void checkBoxChangeAlpha_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxChangeAlpha.Checked)
            {
                changeAlpha = true;
                removeAlpha = false;
                checkBoxRemoveAlpha.Checked = false;
            }
            else
            {
                changeAlpha = false;
            }
        }

        // Aktiviert oder deaktiviert die Entfernung von Alpha-Werten basierend auf dem Zustand des entsprechenden Kontrollkästchens.
        private void checkBoxRemoveAlpha_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRemoveAlpha.Checked)
            {
                removeAlpha = true;
                changeAlpha = false;
                checkBoxChangeAlpha.Checked = false;
            }
            else
            {
                removeAlpha = false;
            }
        }

        // Aktiviert oder deaktiviert die Suche nach Koordinaten basierend auf dem Zustand des entsprechenden Kontrollkästchens.
        private void checkBoxFindCoordinates_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFindPivots.Checked)
            {
                findCoordinates = true;
            }
            else
            {
                findCoordinates = false;
            }
        }

        // Öffnet einen Dialog zur Auswahl von Bildern, die zur Liste hinzugefügt werden sollen.
        private void buttonAddListItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog.Multiselect = true; // Ermöglicht die Auswahl mehrerer Dateien

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Laden der ausgewählten Bilder
                LoadImages(openFileDialog.FileNames);
            }
        }

        // Beendet die Anwendung.
        private void buttonExit_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        // Wendet Pixeloperationen auf ein Bild basierend auf einer Liste von Koordinaten an.
        private void buttonCoordinatesToSpriteSheet_Click(object sender, EventArgs e)
        {
            ApplyPixelsFromFile();
        }

        // Startet die Bildverarbeitung basierend auf den ausgewählten Optionen.
        private void startOperation_Click(object sender, EventArgs e)
        {
            if (imagesList.Count > 0 && (changeAlpha || removePixel || findCoordinates || removeAlpha))
            {
                progressBar.Maximum = imagesList.Count;
                progressBar.Value = 0;

                Point spriteSize = new Point((int)numericUpDownSpriteWidth.Value, (int)numericUpDownSpriteHeight.Value);
                List<string> messages = new List<string>();

                foreach (Image img in imagesList)
                {
                    string imagePath = img.Tag.ToString();
                    try
                    {
                        Bitmap bitmap = new Bitmap(imagePath);
                        bitmap.Tag = img.Tag;

                        stopwatch.Restart();
                        var (coordinates, errorImagePaths) = ModifyImage.FindPixelInSpriteSheet(bitmap, spriteSize, targetColor, changeAlphaTo, removePixel, changeAlpha, findCoordinates, removeAlpha);
                        stopwatch.Stop();

                        string message = $"Processing time: {stopwatch.ElapsedMilliseconds} ms";
                        messages.Add(imagePath);
                        messages.Add(message);

                        bitmap.Dispose();

                        // ProgressBar aktualisieren
                        progressBar.Value++;

                        // Optional: Log-Form aktualisieren oder Nachrichten anzeigen
                    }
                    catch (Exception ex)
                    {
                        messages.Add($"Error loading image: {imagePath}. Error: {ex.Message}");
                    }
                }

                messages.Add("Operations finished.");
                LogForm logForm = new LogForm();
                logForm.AddMessagesToListBox(messages);
                logForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("There are no images in the list or no option selected.");
            }
            progressBar.Value = 0;
        }

        // Fügt ein Bild zur PictureBox und zur internen Bilderliste hinzu.
        private void AddImageToPictureBoxAndList(Image img)
        {
            // Bild in die PictureBox anzeigen
            pictureBox.Image = img;

            // Hintergrundbild ausblenden (auf null setzen)
            pictureBox.BackgroundImage = null;

            // Bild zur Liste hinzufügen
            imagesList.Add(img);

            // ListBox aktualisieren, um die Liste der hinzugefügten Bilder anzuzeigen
            UpdateListBox();
        }

        // Aktualisiert die ListBox, um die aktuelle Liste der Bilder anzuzeigen.
        private void UpdateListBox()
        {
            // ListBox leeren und die Liste der Bilder neu hinzufügen
            listBox.Items.Clear();
            foreach (Image img in imagesList)
            {
                // Den Dateipfad des Bildes holen und zur ListBox hinzufügen
                listBox.Items.Add((string)img.Tag);
            }
        }

        // Entfernt das ausgewählte Bild aus der ListBox und der internen Bilderliste.
        private void RemoveSelectedImages()
        {
            // Erstellen einer Liste, um die zu löschenden Bilder zu speichern
            var selectedItems = listBox.SelectedItems.Cast<string>().ToList();

            if (selectedItems.Count > 0)
            {
                foreach (var selectedItem in selectedItems)
                {
                    // Bild anhand des Tags finden
                    var imageToRemove = imagesList.FirstOrDefault(img => img.Tag.ToString() == selectedItem);
                    if (imageToRemove != null)
                    {
                        imageToRemove.Dispose(); // Freigeben der Ressourcen
                        imagesList.Remove(imageToRemove); // Aus der Liste entfernen
                    }
                }

                // ListBox aktualisieren
                UpdateListBox();

                // PictureBox zurücksetzen, wenn keine Bilder mehr vorhanden sind
                if (imagesList.Count == 0)
                {
                    pictureBox.Image = null;
                    pictureBox.BackgroundImage = Pixelfinder.Properties.Resources.ImageBoxBackground;
                    pictureBox.BackgroundImageLayout = ImageLayout.Center;
                }
                else
                {
                    // Das erste verbleibende Bild anzeigen
                    pictureBox.Image = imagesList[0];
                }
            }
        }


        // Lädt Bilder von den angegebenen Dateipfaden ind den Speicher und fügt sie zur Anwendung hinzu.
        private void LoadImages(string[] fileNames)
        {
            foreach (string file in fileNames)
            {
                try
                {
                    if (!IsImageDuplicate(file))
                    {
                        // Öffne einen Dateistream zum Lesen der Bilddatei
                        using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            // Kopiere Bilddaten in einen MemoryStream
                            using (MemoryStream ms = new MemoryStream())
                            {
                                fs.CopyTo(ms);
                                ms.Seek(0, SeekOrigin.Begin); // Spule den Stream zurück zum Anfang

                                // Lade das Bild aus dem MemoryStream
                                Image img = Image.FromStream(ms);

                                // Weise den Dateipfad der Tag-Eigenschaft zu, um ihn später referenzieren zu können
                                img.Tag = file;

                                // Füge das Bild der PictureBox und der Liste hinzu
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


        // Behandelt das Schließen des Hauptfensters und führt Bereinigungsoperationen durch.
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitApplication();
        }

        // Führt die notwendigen Schritte zum ordnungsgemäßen Beenden der Anwendung durch.
        private void ExitApplication()
        {
            // Alle Bilder in der imagesList freigeben
            foreach (Image img in imagesList)
            {
                img.Dispose();
            }
            imagesList.Clear();

            // Programm beenden
            System.Windows.Forms.Application.Exit();
        }

        // Wendet Pixeländerungen auf ein Bild basierend auf einer Datei mit Koordinaten an.
        public void ApplyPixelsFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images (*.png)|*.png";
            openFileDialog.Title = "Select an Image File";

            // Öffnet das Dialogfenster für die Image-Datei
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string bitmapPath = openFileDialog.FileName;
                Bitmap bitmap = new Bitmap(bitmapPath);

                openFileDialog.Filter = "Text Files (*.txt)|*.txt";
                openFileDialog.Title = "Select a Text File with pivot-points.";

                // Öffnet das Dialogfenster für die Textdatei, die Koordinaten enthält
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string textFilePath = openFileDialog.FileName;
                    List<Point> coordinates = ReadCoordinatesFromTextFile(textFilePath);

                    // Größe des Sprites festlegen
                    Point spriteSize = new Point((int)numericUpDownSpriteWidth.Value, (int)numericUpDownSpriteHeight.Value);

                    ColorDialog colorDialog = new ColorDialog();
                    int[] customColors = new int[] { ColorTranslator.ToOle(Color.FromArgb(255, 0, 255)) }; // Magenta als preset
                    colorDialog.CustomColors = customColors;
                    colorDialog.Color = Color.FromArgb(255, 0, 255); // Standardfarbe auf Magenta gesetzt

                    // Initialisiert colorToDraw mit einem Standardwert
                    Color colorToDraw = Color.FromArgb(255, 0, 255);

                    // Zeigt das Dialogfenster an, um dem Benutzer zu ermöglichen, die Farbe zu ändern
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        colorToDraw = colorDialog.Color; // Aktualisiert die zu zeichnende Farbe, falls der Benutzer eine neue wählt
                    }

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // wendet die Farbe auf die Coordinaten an
                    ModifyImage.SetPixelFromList(bitmap, spriteSize, colorToDraw, coordinates);

                    stopwatch.Stop();
                    long processingTime = stopwatch.ElapsedMilliseconds;

                    // speichert das Bild
                    string modifiedPath = Path.Combine(Path.GetDirectoryName(bitmapPath), "Pivots_" + Path.GetFileName(bitmapPath));
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

        // Speichert Koordinaten in einer Datei ab.
        private void SaveCoordinatesToFile(string imagePath, List<string> coordinates, List<string> messages)
        {
            // Dateinamen des Bildes ohne Erweiterung extrahieren
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(imagePath);

            // Pfad für die Textdatei erstellen (gleicher Ordner wie die Bilddatei)
            string directory = System.IO.Path.GetDirectoryName(imagePath);
            string textureFilePath = System.IO.Path.Combine(directory, fileNameWithoutExtension + ".txt");

            try
            {
                // Textdatei öffnen oder erstellen, um die Koordinaten zu schreiben
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(textureFilePath))
                {
                    // Koordinaten in die Textdatei schreiben
                    foreach (string coordinate in coordinates)
                    {
                        file.WriteLine(coordinate);
                    }
                }

                // Erfolgsmeldung zur Liste hinzufügen
                messages.Add($"Coordinates saved in {textureFilePath}.");
            }
            catch (Exception ex)
            {
                // Fehlermeldung zur Liste hinzufügen, wenn das Speichern fehlschlägt
                messages.Add($"Error saving coordinates to {textureFilePath}: {ex.Message}");
            }
        }

        // Liest Koordinatenpunkte aus einer Textdatei.
        private static List<Point> ReadCoordinatesFromTextFile(string filePath)
        {
            List<Point> coordinates = new List<Point>();  // Erstellt eine Liste für Koordinatenpunkte
            string[] lines = File.ReadAllLines(filePath); // Liest alle Zeilen aus der Datei

            // Durchläuft jede Zeile in der Datei
            foreach (string line in lines)
            {
                string[] parts = line.Trim().Split(','); // Teilt die Zeile an den Kommas und entfernt Leerzeichen
                if (parts.Length == 2) // Überprüft, ob die Zeile genau zwei Teile hat
                {
                    // Versucht, die Teile der Zeile als Koordinaten x und y zu interpretieren
                    if (int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                    {
                        coordinates.Add(new Point(x, y)); // Fügt den Punkt zur Liste hinzu
                        Console.WriteLine("coords" + x + "," + y);  // Gibt die Koordinaten in der Konsole aus
                    }
                }
            }

            return coordinates; // Gibt die Liste der Koordinaten zurück
        }

    }
}
