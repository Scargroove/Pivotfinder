﻿using System;
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
    public partial class MainWindow : Form
    {


        private List<Image> imagesList = new List<Image>();
        private bool removePixel = false;
        private Stopwatch stopwatch = new Stopwatch();


        public MainWindow()
        {
            InitializeComponent();
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.AllowDrop = true;

            
     
        }
        Color targetColor = Color.FromArgb(255, 255, 0, 255);

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
                        // Bild laden und Tag übertragen
                        Bitmap bitmap = new Bitmap(imagePath);
                        bitmap.Tag = img.Tag;  // Übertragen des Tags vom Image zum Bitmap

                        stopwatch.Restart();  // Start der Zeitmessung


                        // Pixel finden
                        List<string> coordinates = FindPixel.FindPixelInSpriteSheet(bitmap, spriteSize, targetColor, removePixel);

                        stopwatch.Stop();  // Zeitmessung stoppen


                        // Koordinaten ausgeben
                        foreach (string coordinate in coordinates)
                        {
                            Console.WriteLine(coordinate);
                        }

                        // Ausgabe der Verarbeitungszeit
                        Console.WriteLine($"Verarbeitungszeit für {imagePath}: {stopwatch.ElapsedMilliseconds} ms");

                        // Nach Verwendung das Bitmap wieder freigeben
                        bitmap.Dispose();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fehler beim Laden des Bildes: " + imagePath + " " + ex.Message);
                    }
                }
                //foreach (Image img in imagesList)
                //{
                //    img.Dispose();
                //}
                MessageBox.Show("Fertig");
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
            if (checkBoxRemovePixel.Checked)
            {
                removePixel = true;
            }
            else
            {
                removePixel = false;
            }
        }

        private void ButtonExit_Click(object sender, EventArgs e)
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
    }
}
