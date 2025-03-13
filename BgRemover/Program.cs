using System;
using System.IO;
using System.Windows.Forms; // Für OpenFileDialog
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BackgroundRemoval
{
    class Program
    {
        static void Main(string[] args)
        {
            // Öffnen des FileDialogs zum Auswählen einer Bilddatei
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Bilddatei einlesen
                string inputFilePath = openFileDialog.FileName;
                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputFilePath))
                {
                    // Hintergrundfarbe (weiß oder hellgrau) definieren
                    var whiteThreshold = 200; // Schwellenwert für die Farbe (für graue/weiße Hintergründe)

                    // Pixel bearbeiten
                    image.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgba32> row = accessor.GetRowSpan(y);
                            for (int x = 0; x < row.Length; x++)
                            {
                                if (IsBackgroundColor(row[x], whiteThreshold))
                                {
                                    row[x] = new Rgba32(0, 0, 0, 0); // Transparent
                                }
                            }
                        }
                    });

                    // Speichern der neuen Datei mit transparentem Hintergrund im PNG-Format
                    string outputFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath), "output.png");
                    image.Save(outputFilePath); // Bild im gleichen Verzeichnis speichern

                    Console.WriteLine($"Bild gespeichert: {outputFilePath}");
                }
            }
        }

        // Hilfsfunktion zur Bestimmung, ob der Pixel zur Hintergrundfarbe gehört
        static bool IsBackgroundColor(Rgba32 pixel, int threshold)
        {
            // Einfache Annahme: ein Pixel ist weiß oder grau, wenn der Rot-, Grün- und Blau-Wert hoch genug sind
            return (pixel.R > threshold && pixel.G > threshold && pixel.B > threshold);
        }
    }
}
