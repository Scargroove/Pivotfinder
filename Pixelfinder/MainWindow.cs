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



        public MainWindow()
        {
            InitializeComponent();
            InitializeTooltips();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.AllowDrop = true;

        }
        private void InitializeTooltips()
        {
            // Konfigurieren des ToolTip
            toolTip1.AutoPopDelay = 5000;    // Wie lange der Tooltip angezeigt wird
            toolTip1.InitialDelay = 1000;    // Wie lange bis der Tooltip erscheint
            toolTip1.ReshowDelay = 500;      // Wie lange bis der Tooltip nach dem ersten Anzeigen erneut angezeigt wird
            toolTip1.ShowAlways = true;      // Der Tooltip wird auch dann angezeigt, wenn das Formular nicht aktiv ist

            // Tooltip-Text festlegen
            toolTip1.SetToolTip(this.checkBoxRemovePivot, "Regeln");
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedImage();
                e.Handled = true;  // Markiert das Ereignis als behandelt
            }
        }


        private void Form1_DragEnter(object sender, DragEventArgs e)
        {

           
            HandleDragEnter(e);

        }
        private void Form1_DragLeave(object sender, EventArgs e)
        {
            listBox.BackColor = SystemColors.Window;

        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
            listBox.BackColor = SystemColors.Window;

        }

        private void listBox_DragEnter(object sender, DragEventArgs e)
        {
            
            HandleDragEnter(e);

        }

        private void listBox_DragLeave(object sender, EventArgs e)
        {
            listBox.BackColor = SystemColors.Window; 

        }
        private void listBox_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
            listBox.BackColor = SystemColors.Window;

        }

        private void PictureBox_DragEnter(object sender, DragEventArgs e)
        {
            
            HandleDragEnter(e);
        }

        private void PictureBox_DragLeave(object sender, EventArgs e)
        {
            listBox.BackColor = SystemColors.Window;

        }

        private void PictureBox_DragDrop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);
            listBox.BackColor = SystemColors.Window;

        }



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


        private void HandleDragDrop(DragEventArgs e)
        {
            // Die gezogenen Daten als Bild laden
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            LoadImages(files);

            // Fokus an das Hauptfenster zurückgeben
            this.Activate();
        }



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

        // Methode zur Überprüfung, ob die Datei eine Bilddatei ist
        private bool IsImageFile(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".bmp";
        }

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
        private void RemoveSelectedImage()
        {
            // Überprüfen, ob ein Bild in der ListBox ausgewählt ist
            int selectedIndex = listBox.SelectedIndex;

            // Wenn kein Bild ausgewählt ist, das letzte Bild in der Liste löschen
            if (selectedIndex == -1 && imagesList.Count > 0)
            {
                selectedIndex = imagesList.Count - 1; // Letztes Element der Liste
            }

            if (selectedIndex != -1)
            {
                // Bildressourcen freigeben
                Image imageToRemove = imagesList[selectedIndex];
                imageToRemove.Dispose();

                // Das ausgewählte Bild aus der ListBox und der Liste entfernen
                imagesList.RemoveAt(selectedIndex);

                // ListBox aktualisieren, um die Liste der verbleibenden Bilder anzuzeigen
                UpdateListBox();

                // Wenn keine Bilder mehr in der Liste sind, die PictureBox zurücksetzen
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

        private void buttonDeleteListItem_Click(object sender, EventArgs e)
        {
            RemoveSelectedImage();
        }


        private void startOperation_Click(object sender, EventArgs e)
        {
            // Prüft, ob Bilder vorhanden sind und ob eine Operation ausgewählt wurde
            if (imagesList.Count > 0 && (changeAlpha || removePixel || findCoordinates || removeAlpha))
            {
                Point spriteSize = new Point((int)numericUpDownSpriteWidth.Value, (int)numericUpDownSpriteHeight.Value);
                List<string> messages = new List<string>();  // Liste zum Speichern von Nachrichten für das Log

                // Iteriert über jedes Bild in der Liste
                foreach (Image img in imagesList)
                {
                    string imagePath = img.Tag.ToString();  // Speichert den Pfad des Bildes
                    try
                    {
                        Bitmap bitmap = new Bitmap(imagePath);  // Erstellt ein Bitmap-Objekt aus dem Bildpfad
                        bitmap.Tag = img.Tag;  // Setzt das Tag des Bitmaps

                        stopwatch.Restart();  // Startet die Stoppuhr neu

                        // Führt die Pixelbearbeitung durch und sammelt Koordinaten und fehlerhafte Bildpfade
                        var (coordinates, errorImagePaths) = FindPixel.FindPixelInSpriteSheet(bitmap, spriteSize, targetColor, changeAlphaTo, removePixel, changeAlpha, findCoordinates, removeAlpha);

                        stopwatch.Stop();  // Stoppt die Stoppuhr

                        string message = $"Verarbeitungszeit für {imagePath}: {stopwatch.ElapsedMilliseconds} ms";
                        messages.Add(message);  // Fügt die Nachricht der Liste hinzu

                        // Speichert Koordinaten, wenn die Option ausgewählt wurde und Koordinaten gefunden wurden
                        if (findCoordinates && coordinates.Any())
                        {
                            SaveCoordinatesToFile(imagePath, coordinates, messages);
                        }

                        // Fügt Fehlermeldungen zur Liste hinzu, wenn fehlerhafte Bildpfade vorhanden sind
                        if (errorImagePaths.Any())
                        {
                            foreach (var errorPath in errorImagePaths)
                            {
                                messages.Add($"Fehler im Bild am Pfad gefunden: {errorPath}");
                            }
                        }

                        bitmap.Dispose();  // Gibt die Ressourcen des Bitmap-Objekts frei
                    }
                    catch (Exception ex)
                    {
                        messages.Add($"Fehler beim Laden des Bildes: {imagePath}. Fehler: {ex.Message}");
                    }
                }
                messages.Add("Beendet");  // Fügt eine Abschlussnachricht hinzu

                LogForm logForm = new LogForm();  // Erstellt ein neues LogForm-Objekt
                logForm.AddMessagesToListBox(messages);  // Fügt die Nachrichten zur ListBox hinzu
                logForm.ShowDialog();  // Zeigt das LogForm-Fenster an
            }
            else
            {
                // Zeigt Fehlermeldungen, wenn keine Bilder vorhanden sind oder keine Option ausgewählt wurde
                if (imagesList.Count == 0)
                {
                    MessageBox.Show("Es sind keine Bilder in der Liste.");
                }
                else
                    MessageBox.Show("Bitte wählen Sie eine Option aus.");
            }
        }


        private void buttonAlphaToColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialogAlpha = new ColorDialog();

            // Definieren einer benutzerdefinierten Farbe als RGB-Wert 255, 0, 0, 0 (Schwarz)
            int[] customColors = new int[] { ColorTranslator.ToOle(Color.FromArgb(255, 0, 0 , 0)) };
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
        private void LoadImages(string[] fileNames)
        {
            foreach (string file in fileNames)
            {
                try
                {
                    // Überprüfen, ob das Bild bereits in der Liste vorhanden ist
                    if (!IsImageDuplicate(file))
                    {
                        // Bild aus der Datei laden
                        Image img = Image.FromFile(file);

                        // Den Dateipfad als Tag zum Bildobjekt hinzufügen
                        img.Tag = file;

                        // Bild zur PictureBox und zur Liste hinzufügen
                        AddImageToPictureBoxAndList(img);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Laden des Bildes: " + ex.Message);
                }
            }
        }

        private void CheckBoxRemovePixel_CheckStateChanged(object sender, EventArgs e)
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
            Console.WriteLine("pressed change alpha" + changeAlpha);

        }
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
            Console.WriteLine("pressed remove alpha"+ removeAlpha);
        }
        private void checkBoxFindCoordinates_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFindPivots.Checked)
            {
                findCoordinates = true;
            }
            else
            {
                findCoordinates= false;
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitApplication();
        }
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

        private void buttonCoordinatesToSpriteSheet_Click(object sender, EventArgs e)
        {
            ApplyPixelsFromFile();
        }
        public void ApplyPixelsFromFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmap Images (*.png)|*.png";
            openFileDialog.Title = "Select an Image File";

            // Öffnet das Dialogfenster für die Bitmap-Datei
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string bitmapPath = openFileDialog.FileName;
                Bitmap bitmap = new Bitmap(bitmapPath);

                openFileDialog.Filter = "Text Files (*.txt)|*.txt";
                openFileDialog.Title = "Select a Text File with Coordinates";

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
                    FindPixel.ChangePixelInSpriteSheet(bitmap, spriteSize, colorToDraw, coordinates);

                    stopwatch.Stop();
                    long processingTime = stopwatch.ElapsedMilliseconds;

                    // speichert das Bild
                    string modifiedPath = Path.Combine(Path.GetDirectoryName(bitmapPath), "Pivots_" + Path.GetFileName(bitmapPath));
                    bitmap.Save(modifiedPath);
                    bitmap.Dispose();

                    List<string> messages = new List<string>
            {
                $"Bitmap modified and saved successfully at {modifiedPath}.",
                $"Processing time: {processingTime} ms"
            };

                    LogForm logForm = new LogForm();
                    logForm.AddMessagesToListBox(messages);
                    logForm.ShowDialog();
                }
            }
        }


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
