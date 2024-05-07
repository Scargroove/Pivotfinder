using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Pixelfinder
{
    internal class ModifyImage
    {

        // Findet Pixel einer bestimmten Farbe in einem Bitmap und kann optional diese Pixel auch entfernen.
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
                Color transparent = Color.FromArgb(0, 0, 0, 0);
                ChangeAlphaToColor(pixelData, transparent);
            }

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

        // Sucht das Pixel mit der Ziel-Farbe in einem einzelnen Sprite.
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
                                // Falls mehrere Pivots gefunden wurden, wird auf Gruppierung beding durch upscaling gesucht.
                                // Bei Gruppierung von 4 oder weniger Pivots wird der erste zurückgegeben, der gefunden wurde.
                                // Falls keine Gruppierung, wird ein falscher Wert zurückgegeben.
                                result = FindPixelGroupInSprite(pixelData, stride, spriteSize, targetColorInt, startPos);

                                return result; // Abbrechen und ungültiges Ergebnis zurückgeben.
                            }

                            result = new Point(x, y);
                        }

                        if (removePixel)
                        {
                            ReplacePixelWithDominantOrNearestAverageColor(pixelData, stride, spriteSize, position, targetColorInt);
                        }
                        foundPixelCount++; // Inkrementieren des Zählers für gefundene Pixel.
                    }
                }
            }

            return result;
        }

        // Sucht nach Gruppen der Pixel und gibt die Koordinate des ersten Pixel zurück, wenn es eine Gruppe unter 4 ist.
        private static Point FindPixelGroupInSprite(byte[] pixelData, int stride, Point spriteSize, int targetColorInt, Point startPos)
        {
            int width = spriteSize.X; // Breite des Sprites
            int height = spriteSize.Y; // Höhe des Sprites
            bool[,] visited = new bool[height, width]; // Array zum Speichern der besuchten Pixel
            Point result = new Point(-1, -1); // Fehlerpunkt als Standardwert, wenn keine passende Gruppe gefunden wird

            Func<int, int, List<Point>> dfs = null; // Funktion für die Tiefensuche
            dfs = (x, y) =>
            {
                List<Point> component = new List<Point>(); // Liste der Punkte in der aktuellen Komponente
                if (x < 0 || x >= width || y < 0 || y >= height || visited[y, x]) // Überprüfung der Grenzen und ob bereits besucht
                    return component;

                int position = ((startPos.Y + y) * stride) + ((startPos.X + x) * 4); // Berechnung der Position im Byte-Array
                int color = BitConverter.ToInt32(pixelData, position); // Umwandlung von Byte-Daten in einen Integer-Wert für die Farbe
                visited[y, x] = true; // Markierung des Punktes als besucht

                if (color == targetColorInt) // Überprüfung, ob die Farbe mit der Ziel-Farbe übereinstimmt
                {
                    component.Add(new Point(x, y)); // Hinzufügen des Punktes zur Komponente
                    foreach (var dir in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) }) // Erkundung in vier Richtungen
                    {
                        component.AddRange(dfs(x + dir.Item1, y + dir.Item2)); // Rekursiver Aufruf der Tiefensuche
                    }
                }
                return component;
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!visited[y, x]) // Überprüfung, ob der Punkt noch nicht besucht wurde
                    {
                        var component = dfs(x, y); // Start der Tiefensuche von diesem Punkt
                        if (component.Count > 0 && component.Count <= 4) // Überprüfung der Größe der Komponente
                        {
                            var minX = component.Min(p => p.X); // Berechnung des kleinsten X-Wertes
                            var minY = component.Min(p => p.Y); // Berechnung des kleinsten Y-Wertes
                                                                // Rückgabe des ersten gefundenen Punktes der Komponente mit Anpassung durch startPos
                            return new Point(minX + startPos.X, minY + startPos.Y);
                        }
                        else if (component.Count > 4) // Falls die Komponente zu groß ist
                        {
                            // Rückgabe eines Fehlerpunkts
                            return new Point(-1, -1);
                        }
                    }
                }
            }

            return result; // Rückgabe des Ergebnis-Punktes, wenn keine geeignete Komponente gefunden wurde
        }

        // Speichert das veränderte Bitmap unter einem neuen Dateinamen.
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

        // Ersetzt die gesuchten Pixel mit Farben in der Nähe oder Durchschnittsfarben wenn es alle verschiedene sind.
        private static void ReplacePixelWithDominantOrNearestAverageColor(byte[] pixelData, int stride, Point spriteSize, int position, int targetColorInt)
        {
            int[] neighboringColors = new int[4];
            int y = position / stride;
            int x = (position % stride) / 4;

            // Erfassen der Farben der umliegenden Pixel
            if (y > 0) neighboringColors[0] = BitConverter.ToInt32(pixelData, position - stride); // oben
            if (y < spriteSize.Y - 1) neighboringColors[1] = BitConverter.ToInt32(pixelData, position + stride); // unten
            if (x > 0) neighboringColors[2] = BitConverter.ToInt32(pixelData, position - 4); // links
            if (x < spriteSize.X - 1) neighboringColors[3] = BitConverter.ToInt32(pixelData, position + 4); // rechts

            // Filtern der Ziel-Farbe aus den umgebenden Farben
            var filteredColors = neighboringColors.Where(color => color != targetColorInt).ToArray();

            // Gruppieren der Farben, um die dominante Farbe zu ermitteln
            var groupedColors = filteredColors.GroupBy(color => color)
                                              .Select(group => new { Color = group.Key, Count = group.Count() })
                                              .OrderByDescending(group => group.Count);

            // Bestimme die dominante Farbe, falls vorhanden
            var dominantColor = groupedColors.FirstOrDefault(group => group.Count >= 2);

            int colorToSet;
            if (dominantColor != null)
            {
                // Verwende die dominante Farbe, wenn eine gefunden wurde
                colorToSet = dominantColor.Color;
            }
            else
            {
                // Keine dominante Farbe gefunden, berechne den Durchschnitt und finde die nächstgelegene Farbe
                int averageColor = CalculateAverageColor(filteredColors);
                colorToSet = filteredColors.OrderBy(color => ColorDistance(averageColor, color)).FirstOrDefault();
            }

            // Setzen der ausgewählten Farbe am aktuellen Pixel
            BitConverter.GetBytes(colorToSet).CopyTo(pixelData, position);
        }

        // Berechnet den Durchschnittswert der Farben in einem Array von Farb-Integer-Werten.
        private static int CalculateAverageColor(int[] colors)
        {
            // Initialisiere die Summe der Rot-, Grün- und Blauwerte.
            int r = 0, g = 0, b = 0;
            // Zähler für die Anzahl der verarbeiteten Farben.
            int count = 0;
            // Durchlaufe jedes Farb-Integer im Array.
            foreach (int color in colors)
            {
                // Extrahiere den Rotanteil der Farbe und addiere ihn zur Summe.
                r += (color >> 16) & 0xFF;
                // Extrahiere den Grünanteil der Farbe und addiere ihn zur Summe.
                g += (color >> 8) & 0xFF;
                // Extrahiere den Blauanteil der Farbe und addiere ihn zur Summe.
                b += color & 0xFF;
                // Erhöhe den Zähler um eins für jede verarbeitete Farbe.
                count++;
            }
            // Teile die Summen durch die Anzahl der Farben, um den Durchschnitt zu berechnen.
            if (count > 0)
            {
                r /= count;
                g /= count;
                b /= count;
            }
            // Kombiniere die Durchschnittswerte von Rot, Grün und Blau zu einem einzigen Farb-Integer.
            return (r << 16) | (g << 8) | b;
        }

        // Berechnet die Distanz zwischen zwei Farben, basierend auf ihren RGB-Werten.
        private static int ColorDistance(int color1, int color2)
        {
            // Extrahiere die RGB-Komponenten der ersten Farbe.
            int r1 = (color1 >> 16) & 0xFF;
            int g1 = (color1 >> 8) & 0xFF;
            int b1 = color1 & 0xFF;

            // Extrahiere die RGB-Komponenten der zweiten Farbe.
            int r2 = (color2 >> 16) & 0xFF;
            int g2 = (color2 >> 8) & 0xFF;
            int b2 = color2 & 0xFF;

            // Berechne das quadratische Abstandsmaß zwischen den beiden Farben.
            return (r1 - r2) * (r1 - r2) + (g1 - g2) * (g1 - g2) + (b1 - b2) * (b1 - b2);
        }

        // Setzt alle Alpha-Werte zwischen 1 und 254 auf einen anderen Wert.
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

        // Setzt Pixel in der gewünschten Farbe auf die Koordinaten der Liste.
        public static void SetPixelFromList(Bitmap bitmap, Point spriteSize, Color targetColor, List<Point> points)
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

            // Schleife über jede Zeile im Sprite-Raster
            for (int y = 0; y < spriteAmount.Y; y++)
            {
                // Schleife über jede Spalte im aktuellen Raster der Zeile
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    // Überprüfe, ob der Punkt nicht am Ursprung (0,0) liegt
                    if (points[pointListPosition].X != 0 && points[pointListPosition].Y != 0)
                    {
                        // Berechne die X-Koordinate des Pixels in der Bitmap
                        int pixelX = (spriteSize.X * x) + points[pointListPosition].X;

                        // Berechne die Y-Koordinate des Pixels in der Bitmap
                        int pixelY = (spriteSize.Y * y) + points[pointListPosition].Y;

                        // Berechne die Position des Pixels im eindimensionalen Array 'rgbValues', das die Bitmap-Daten enthält
                        int position = (pixelY * bitmapData.Stride) + (pixelX * depth);

                        // Setze den Blauanteil des Pixels
                        rgbValues[position] = targetColor.B;

                        // Setze den Grünanteil des Pixels
                        rgbValues[position + 1] = targetColor.G;

                        // Setze den Rotanteil des Pixels
                        rgbValues[position + 2] = targetColor.R;

                        // Überprüfe, ob das Bildformat 32 Bits pro Pixel hat (RGBA), um den Alphawert zu setzen
                        if (depth == 4)
                        {
                            // Setze den Alphawert (Transparenz) des Pixels
                            rgbValues[position + 3] = targetColor.A;
                        }
                    }

                    // Erhöhe die Position in der Liste der Punkte
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


