using System;
using System.IO;
using System.Linq;

namespace UltraLossyCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            // Verify we have 1 argument specified, and that it's an existing file
            if (args.Length != 1 && !File.Exists(args[0]))
            {
                Console.WriteLine("Please specify a valid file");
                return;
            }

            string inputPath = args[0];

            // Verify the extension of the file
            if (!new[] { ".jpg", ".png", ".ultralossy" }.Contains(Path.GetExtension(inputPath).ToLower()))
            {
                Console.WriteLine("Image not a valid type, please select a .jpg or .png");
                return;
            }

            Console.WriteLine($"Converting {inputPath}");

            // If compressed, convert to full size image 
            if (Path.GetExtension(inputPath).Equals(".ultralossy"))
            {
                string imageDescription = File.ReadAllText(inputPath);

                string outputPath = Path.ChangeExtension(inputPath, null);

                Console.WriteLine($"Writing output to {outputPath}");

                AzureFunctions.DownloadImageFromDescription(imageDescription, outputPath);

                File.Delete(inputPath);
            }

            // If uncompressed, convert to .ultralossy compressed format
            else
            {
                string imageDescription = AzureFunctions.GetDescriptionFromImage(inputPath).Result;

                string outputPath = String.Concat(inputPath, ".ultralossy");

                Console.WriteLine($"Writing output to {outputPath}");

                File.WriteAllText(outputPath, imageDescription);

                File.Delete(inputPath);
            }
        }
    }
}
