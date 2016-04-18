using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Galleries_Generator
{
    internal class Program
    {
        private const string GALLERIES_CONTAINER_NAME = "galleries";

        private static void Main(string[] args)
        {
            var rootDirectory = Directory.GetCurrentDirectory();

            if (!IsCorrectRootDirectory(rootDirectory))
            {
                Console.WriteLine("Please run in the correct folder !");
                Console.WriteLine("Press any key to exit ...");

                Console.ReadKey();

                return;
            }

            var galleryDirectories = Directory.EnumerateDirectories(rootDirectory).ToList();

            var galleries = new List<Gallery>();

            for (var index = 0; index < galleryDirectories.Count; index++)
            {
                var galleryDirectory = galleryDirectories[index];
                var galleryName = Path.GetFileName(galleryDirectory);

                if (galleryName == null) continue;

                var galleryYear = new Regex("((?:(?:[1]{1}\\d{1}\\d{1}\\d{1})|(?:[2]{1}\\d{3})))(?![\\d])", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(galleryName).Groups[1].ToString();

                var gallery = new Gallery
                {
                    Id = index + 1,
                    Name = galleryName,
                    Year = int.Parse(galleryYear)
                };

                var imagePaths = Directory.GetFiles(galleryDirectory).Where(path =>
                {
                    var fileName = Path.GetFileName(path);
                    return fileName != null && !fileName.Contains("thumb") && fileName.ToLower().Contains("jpg");
                });

                foreach (var imagePath in imagePaths)
                {
                    var image = System.Drawing.Image.FromFile(imagePath);

                    var thumb = ResizeImage(image, 250, image.Width*250/image.Height);
                    thumb.Save(Path.ChangeExtension(imagePath, "thumb.jpg"));

                    image.Dispose();
                    thumb.Dispose();

                    var imageName = Path.GetFileName(imagePath);

                    gallery.Images.Add(new Image
                    {
                        RealPath = galleryName + "/" + imageName,
                        ThumbnailPath = galleryName + "/" + Path.GetFileNameWithoutExtension(imageName) + ".thumb.jpg"
                    });
                }

                galleries.Add(gallery);
            }

            var output = JsonConvert.SerializeObject(galleries);
            File.WriteAllText(rootDirectory + "\\galleries.json", output);

            Console.WriteLine("Finished !");
            Console.WriteLine("Press any key to exit ...");
            Console.ReadLine();
        }

        private static bool IsCorrectRootDirectory(string rootDirectory)
        {
            return Path.GetFileName(rootDirectory) == GALLERIES_CONTAINER_NAME;
        }

        private static Bitmap ResizeImage(System.Drawing.Image image, int height, int width)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}