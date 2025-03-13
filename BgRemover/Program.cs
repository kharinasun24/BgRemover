using System;
using System.IO;
using System.Windows.Forms; // F�r OpenFileDialog
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace BackgroundRemoval
{
    class Program
    {
        static void Main(string[] args)
        {
            // �ffnen des FileDialogs zum Ausw�hlen einer Bilddatei
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Bilddatei einlesen
                string inputFilePath = openFileDialog.FileName;
                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputFilePath))
                {
                    // Hintergrundfarbe (wei� oder hellgrau) definieren
                    var whiteThreshold = 200; // Schwellenwert f�r die Farbe (f�r graue/wei�e Hintergr�nde)

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

        // Hilfsfunktion zur Bestimmung, ob der Pixel zur Hintergrundfarbe geh�rt
        static bool IsBackgroundColor(Rgba32 pixel, int threshold)
        {
            // Einfache Annahme: ein Pixel ist wei� oder grau, wenn der Rot-, Gr�n- und Blau-Wert hoch genug sind
            return (pixel.R > threshold && pixel.G > threshold && pixel.B > threshold);
        }
    }
}
