using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pixelfinder
{
    public partial class Form1 : Form
    {


        private List<Image> imagesList = new List<Image>();

        public Form1()
        {
            InitializeComponent();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            // Drag-and-Drop für das Formular aktivieren
            this.AllowDrop = true;
            // Farbcode definieren (RGB-Wert)

        }
        Color targetColor = Color.FromArgb(255, 255, 0, 255);

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox.AllowDrop = true;
            listBox.AllowDrop = true ;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {

            listBox.BackColor = Color.LightGray;
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
            listBox.BackColor = Color.LightGray; 
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
            listBox.BackColor = Color.LightGray;
            HandleDragEnter(e);
        }

        private void pictureBox_DragLeave(object sender, EventArgs e)
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
                bool allFilesAreImages = files.All(file => IsImageFile(file));
                if (allFilesAreImages)
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }
        private void HandleDragDrop(DragEventArgs e)
        {
            // Die gezogenen Daten als Bild laden
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
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
                    MessageBox.Show("Das Bild mit demselben Namen und Pfad existiert bereits.");
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


        private void startPixelfind_Click(object sender, EventArgs e)
        {
            if (imagesList.Count > 0)
            {
                

                // Breite und Höhe des Sprites definieren
                Point spriteSize = new Point((int)numericUpDownSpriteWidth.Value, (int)numericUpDownSpriteHeight.Value);

                foreach (Image img in imagesList)
                {
                    // Pfad aus dem Tag des Bildes holen
                    string imagePath = img.Tag.ToString();

                    try
                    {
                        // Bild laden
                        Bitmap bitmap = new Bitmap(imagePath);

                        // Pixel finden
                        List<string> coordinates = FindPixel.FindPixelInSpriteSheet(bitmap, spriteSize, targetColor);
                        
                        // Koordinaten ausgeben
                        foreach (string coordinatesItem in coordinates)
                        {
                            Console.WriteLine(coordinatesItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler beim Laden des Bildes: " + imagePath + " " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Es sind keine Bilder in der Liste.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
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
    }
}
