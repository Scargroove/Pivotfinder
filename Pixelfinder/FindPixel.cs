using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Pixelfinder
{
    internal class FindPixel
    {


        // Diese Methode findet Pixel einer bestimmten Farbe in einem Bitmap und kann optional diese Pixel auch entfernen.
        public static (List<string> resultsList, List<string> errorImagePaths) FindPixelInSpriteSheet(Bitmap bitmap, Point spriteSize, Color targetColor, Color changeAlphaTo, bool removePixel = false, bool changeAlpha = false, bool findCoordinates = false, bool removeAlpha = false)
        {
            // Überprüft, ob die Größe eines einzelnen Sprites kleiner oder gleich der Größe des gesamten Bitmaps ist.
            if (spriteSize.X > bitmap.Width || spriteSize.Y > bitmap.Height)
            {
                throw new ArgumentException("The sprite size must not be larger than the spritesheet size.");
            }

            // Konvertiert die Ziel-Farbe in ein ARGB-Integer-Format für einfache Vergleiche.
            int targetColorInt = targetColor.ToArgb();

            // Berechnet die Anzahl der Sprites in X- und Y-Richtung auf dem Spritesheet.
            Point spriteAmount = new Point(bitmap.Width / spriteSize.X, bitmap.Height / spriteSize.Y);

            List<string> resultsList = new List<string>();
            List<string> errorImagePaths = new List<string>(); // Liste der Bildpfade mit dem Fehler

            // Sperren des Bitmaps im Speicher für schnellen Zugriff.
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] pixelData = new byte[bitmapData.Stride * bitmapData.Height];
            Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);

            // Durchläuft jedes Sprite auf dem Spritesheet.
            for (int y = 0; y < spriteAmount.Y; y++)
            {
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    // Findet die Position des gesuchten Pixels im Sprite.
                    Point result = FindPixelInSprite(pixelData, bitmapData.Stride, spriteSize, targetColorInt, new Point(spriteSize.X * x, spriteSize.Y * y), removePixel, findCoordinates, bitmap.Tag.ToString());
                    if (findCoordinates)
                    {
                        if (result.X != -1 && result.Y != -1)
                        {
                            resultsList.Add(result.X + "," + result.Y);
                        }
                        else
                        {
                            // Fehler: Mehrere Pixel gefunden
                            errorImagePaths.Add(bitmap.Tag.ToString());
                        }
                    }

                }
            }
            if (changeAlpha)
            {

                ChangeAlphaToColor(pixelData, changeAlphaTo);
            }
            else if (removeAlpha)
            {
                Color transparent = Color.FromArgb(0,0,0,0);
                ChangeAlphaToColor(pixelData, transparent);
            }
            Console.WriteLine("remove: "+ removeAlpha + " change: " + changeAlpha);

            // Kopiert die veränderten Pixeldaten zurück ins Bitmap.
            Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            // Optional: Speichert das veränderte Bitmap.
            if (removePixel || changeAlpha || removeAlpha)
            {
                SaveModifiedBitmap(bitmap);
            }

            // Gibt das Bitmap-Speicher frei.
            bitmap.Dispose();
            return (resultsList, errorImagePaths);
        }

        // Diese Hilfsmethode sucht das Pixel mit der Ziel-Farbe in einem einzelnen Sprite.
        private static Point FindPixelInSprite(byte[] pixelData, int stride, Point spriteSize, int targetColorInt, Point startPos, bool removePixel = false, bool findCoordinates = false, string imagePath = "")
        {
            Point result = new Point(0, 0);
            int foundPixelCount = 0; // Zähler für die Anzahl der gefundenen Pixel.

            // Durchsucht das Sprite Pixel für Pixel.
            for (int y = 0; y < spriteSize.Y; y++)
            {
                int rowStart = (startPos.Y + y) * stride + startPos.X * 4;

                for (int x = 0; x < spriteSize.X; x++)
                {
                    int position = rowStart + x * 4;
                    int color = BitConverter.ToInt32(pixelData, position);

                    if (color == targetColorInt)
                    {
                        if (findCoordinates == true)
                        {
                            if (foundPixelCount > 0)
                            {
                                // Warnung ausgeben, dass mehr als ein Pixel gefunden wurde.
                                MessageBox.Show($"Multiple pixels found in {imagePath}. Only one pixel should exist.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                result = new Point(-1, -1); // falsche Werte setzen.
                                return result; // Abbrechen und ungültiges Ergebnis zurückgeben.
                            }

                            result = new Point(x, y);
                        }

                        if (removePixel)
                        {
                            ReplacePixelWithDominantNeighboringColor(pixelData, stride, spriteSize, position);
                        }
                        foundPixelCount++; // Inkrementieren des Zählers für gefundene Pixel.
                    }
                }
            }

            return result;
        }


        // Diese Methode speichert das veränderte Bitmap unter einem neuen Dateinamen.
        public static void SaveModifiedBitmap(Bitmap bitmap, string modifiedText = "_modified")
        {
            // Prüft, ob das Tag des Bitmaps einen gültigen Speicherpfad enthält
            if (bitmap.Tag == null || string.IsNullOrEmpty(bitmap.Tag.ToString()))
            {
                throw new InvalidOperationException("The bitmap tag does not contain a valid storage path.");
            }

            // Holt den ursprünglichen Pfad des Bildes aus dem Tag
            string originalPath = bitmap.Tag.ToString();
            // Bestimmt das Verzeichnis des Originalbildes
            string directory = Path.GetDirectoryName(originalPath);
            // Extrahiert den Dateinamen ohne Erweiterung
            string filename = Path.GetFileNameWithoutExtension(originalPath);
            // Extrahiert die Dateierweiterung
            string extension = Path.GetExtension(originalPath);
            // Erstellt den neuen Dateinamen mit dem Zusatztext
            string newFileName = Path.Combine(directory, filename + modifiedText + extension);

            // Speichert das Bitmap unter dem neuen Dateinamen als PNG
            bitmap.Save(newFileName, ImageFormat.Png);
        }

        // Diese Methode ersetzt ein gefundenes Pixel durch die dominante Farbe der benachbarten Pixel, falls erforderlich.
        private static void ReplacePixelWithDominantNeighboringColor(byte[] pixelData, int stride, Point spriteSize, int position)
        {
            int[] neighboringColors = new int[4];
            int y = position / stride;
            int x = (position % stride) / 4;

            // Überprüfen der benachbarten Pixel und Sammeln ihrer Farben
            if (y > 0) neighboringColors[0] = BitConverter.ToInt32(pixelData, position - stride); // oben
            if (y < spriteSize.Y - 1) neighboringColors[1] = BitConverter.ToInt32(pixelData, position + stride); // unten
            if (x > 0) neighboringColors[2] = BitConverter.ToInt32(pixelData, position - 4); // links
            if (x < spriteSize.X - 1) neighboringColors[3] = BitConverter.ToInt32(pixelData, position + 4); // rechts

            // Gruppierung der Farben und Überprüfung auf dominante Gruppen
            var colorGroups = neighboringColors.GroupBy(c => c);
            var dominantGroup = colorGroups.FirstOrDefault(g => g.Count() >= 3)
                                ?? colorGroups.FirstOrDefault(g => g.Count() == 2);

            if (dominantGroup == null && colorGroups.Count() > 0) // Keine dominante Gruppe gefunden
            {
                // Berechnung des Durchschnittsfarbwerts
                int averageColor = CalculateAverageColor(neighboringColors);
                // Wähle die Farbe, die dem Durchschnittswert am nächsten liegt
                int nearestColor = FindNearestColor(neighboringColors, averageColor);
                BitConverter.GetBytes(nearestColor).CopyTo(pixelData, position);
            }
            else if (dominantGroup != null)
            {
                // Setze die dominante Farbe, falls vorhanden
                int newColor = dominantGroup.Key;
                BitConverter.GetBytes(newColor).CopyTo(pixelData, position);
            }
        }

        // Diese Methode berechnet den Durchschnittswert der Farben in einem Array.
        private static int CalculateAverageColor(int[] colors)
        {
            int r = 0, g = 0, b = 0; // Variablen zur Speicherung der summierten Farbkomponenten Rot, Grün, Blau.
            int count = 0; // Zähler für die Anzahl der Farben im Array.

            // Durchlaufen jedes Farbwertes im übergebenen Array.
            foreach (int color in colors)
            {
                r += (color >> 16) & 0xFF; // Extrahieren und Aufsummieren der Rot-Komponente.
                g += (color >> 8) & 0xFF;  // Extrahieren und Aufsummieren der Grün-Komponente.
                b += color & 0xFF;         // Extrahieren und Aufsummieren der Blau-Komponente.
                count++;                   // Inkrementieren des Zählers.
            }

            // Vermeidung der Division durch Null, falls das Array leer ist.
            if (count > 0)
            {
                r /= count; // Berechnen des Durchschnitts für Rot.
                g /= count; // Berechnen des Durchschnitts für Grün.
                b /= count; // Berechnen des Durchschnitts für Blau.
            }

            // Rückgabe des Durchschnittswerts als eine einzige Farbe im ARGB-Format.
            return (r << 16) | (g << 8) | b;
        }

        // Diese Methode findet die Farbe, die einer Ziel-Farbe am nächsten liegt, aus einem Array von Farben.
        private static int FindNearestColor(int[] colors, int targetColor)
        {
            int minDistance = int.MaxValue; // Variable zur Speicherung der kleinsten gefundenen Distanz.
            int nearestColor = 0;           // Variable zur Speicherung der Farbe mit der kleinsten Distanz.

            // Durchlaufen aller Farben im Array.
            foreach (int color in colors)
            {
                int distance = ColorDistance(color, targetColor); // Berechnen der Distanz zur Ziel-Farbe.

                // Aktualisieren der minimalen Distanz und der nächsten Farbe, falls die aktuelle Distanz kleiner ist.
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestColor = color;
                }
            }
            return nearestColor; // Rückgabe der Farbe mit der geringsten Distanz zur Ziel-Farbe.
        }

        // Diese Methode berechnet die Distanz zwischen zwei Farben im RGB-Farbraum.
        private static int ColorDistance(int color1, int color2)
        {
            // Extrahieren der Rot-, Grün- und Blau-Komponenten der ersten Farbe.
            int r1 = (color1 >> 16) & 0xFF;
            int g1 = (color1 >> 8) & 0xFF;
            int b1 = color1 & 0xFF;

            // Extrahieren der Rot-, Grün- und Blau-Komponenten der zweiten Farbe.
            int r2 = (color2 >> 16) & 0xFF;
            int g2 = (color2 >> 8) & 0xFF;
            int b2 = color2 & 0xFF;

            // Berechnen des quadratischen Abstands zwischen den Farben im RGB-Raum.
            return (r2 - r1) * (r2 - r1) + (g2 - g1) * (g2 - g1) + (b2 - b1) * (b2 - b1);
        }

        private static void ChangeAlphaToColor(byte[] pixelData, Color targetColor)
        {
            // Durchlaufe alle Pixel im Bild
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                // Alpha-Wert des Pixels abrufen
                byte alpha = pixelData[i + 3];

                // auf Alpha-Wert prüfen
                if (alpha != 0 && alpha != 255)
                {
                    // Die Farbkomponenten auf die Ziel-Farbe setzen
                    pixelData[i] = targetColor.B;
                    pixelData[i + 1] = targetColor.G;
                    pixelData[i + 2] = targetColor.R;
                    pixelData[i + 3] = targetColor.A;
                }
            }
        }


        public static void ChangePixelInSpriteSheet(Bitmap bitmap, Point spriteSize, Color targetColor, List<Point> points)
        {
            // Ermittelt die Größe des Spritesheets
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Sperrt die Bitmap-Daten für den Schreib- und Lesezugriff
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Erhält die Adresse der ersten Zeile
            IntPtr ptr = bitmapData.Scan0;

            // Deklariert ein Array, um die Bytes der Bitmap zu halten
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];

            // Kopiert die RGB-Werte in das Array
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            int depth = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8; // Bytes pro Pixel

            // Berechnet die Anzahl der Sprites
            Point spriteAmount = new Point(bitmap.Width / spriteSize.X, bitmap.Height / spriteSize.Y);
            int pointListPosition = 0;

            for (int y = 0; y < spriteAmount.Y; y++)
            {
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    if (points[pointListPosition].X != 0 && points[pointListPosition].Y != 0)
                    {
                        int pixelX = (spriteSize.X * x) + points[pointListPosition].X;
                        int pixelY = (spriteSize.Y * y) + points[pointListPosition].Y;
                        int position = (pixelY * bitmapData.Stride) + (pixelX * depth);

                        rgbValues[position] = targetColor.B; // Blau
                        rgbValues[position + 1] = targetColor.G; // Grün
                        rgbValues[position + 2] = targetColor.R; // Rot
                        if (depth == 4) // Wenn das PixelFormat 32bppArgb ist, wird auch der Alphawert gesetzt
                        {
                            rgbValues[position + 3] = targetColor.A; // Alpha
                        }
                    }
                    pointListPosition++;
                }
            }

            // Kopiert die RGB-Werte zurück in die Bitmap
            Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Entsperren der Bitmap-Daten
            bitmap.UnlockBits(bitmapData);
        }
    }
}


