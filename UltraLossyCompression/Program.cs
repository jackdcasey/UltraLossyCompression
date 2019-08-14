using System;
using System.IO;
using System.Linq;

namespace UltraLossyCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[] { "/Users/jack/Projects/UltraLossyCompression/UltraLossyCompression/TestImages/DogPhoto.png.ultralossy" };

            if (args.Length != 1 && !File.Exists(args[0]))
            {
                Console.WriteLine("Please specify a valid file");
                return;
            }

            string inputPath = args[0];

            if (!new[] { ".jpg", ".png", ".ultralossy" }.Contains(Path.GetExtension(inputPath).ToLower()))
            {
                Console.WriteLine("Image not a valid type, please select a .jpg or .png");
                return;
            }

            Console.WriteLine($"Converting {inputPath}");

            if (Path.GetExtension(inputPath).Equals(".ultralossy"))
            {
                string imageDescription = File.ReadAllText(inputPath);

                string outputPath = Path.ChangeExtension(inputPath, null);

                Console.WriteLine($"Writing output to {outputPath}");

                AzureFunctions.DownloadImageFromDescription(imageDescription, outputPath);

                File.Delete(inputPath);
            }
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
