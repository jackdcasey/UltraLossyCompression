using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;


namespace UltraLossyCompression
{
    public static class AzureFunctions
    {
        const string VisionSubscriptionKey = "f587f069585b4fee982c8022d7622fd8";
        const string SearchSubscriptionKey = "f9b51224746a4f2497c8d37695bf335e";

        // Given an image file, converts into a string description of that image
        public static async Task<string> GetDescriptionFromImage(string filePath)
        {
            ComputerVisionClient computerVision =
                new ComputerVisionClient(new Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials(VisionSubscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            computerVision.Endpoint = "https://westus.api.cognitive.microsoft.com";

            var features = new List<VisualFeatureTypes>() { VisualFeatureTypes.Description };

            using (Stream imageStream = File.OpenRead(filePath))
            {
                ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream, features);

                return analysis.Description.Captions.FirstOrDefault().Text;
            }
        }

        // Given a description and filepath, converts a description into a full size image
        public static void DownloadImageFromDescription(string description, string filePath)
        {
            var client = new ImageSearchClient(new Microsoft.Azure.CognitiveServices.Search.ImageSearch.ApiKeyServiceClientCredentials(SearchSubscriptionKey));

            var imageresults = client.Images.SearchAsync(description).Result;

            string url = imageresults.Value.ElementAt(new Random().Next(imageresults.Value.Count)).ContentUrl;

            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(new Uri(url));

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (var newImage = Image.FromStream(mem))
                    {
                        switch (Path.GetExtension(filePath).ToLower())
                        {
                            case ".jpg":
                                newImage.Save(filePath, ImageFormat.Jpeg);
                                break;
                            case ".png":
                                newImage.Save(filePath, ImageFormat.Png);
                                break;
                        }
                    }
                }
            }
        }
    }
}
