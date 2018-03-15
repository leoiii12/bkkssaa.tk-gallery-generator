using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GalleryGenerator
{
    class Program
    {
        private const string ContainingDirectoryName = "galleries";

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            var targetRootDirectory = configuration["targetRootDirectory"];

            try
            {
                Run(targetRootDirectory);

                Console.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} Done all.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
                Console.WriteLine("Press any key to exit...");

                Console.ReadKey();
            }
        }

        private static void Run(string targetRootDirectory)
        {
            var runningDirectory = Path.Combine(Directory.GetCurrentDirectory(), targetRootDirectory);
            var neighbourDirectories = Directory.EnumerateDirectories(runningDirectory).ToList();

            if (!runningDirectory.EndsWith(ContainingDirectoryName))
            {
                runningDirectory = neighbourDirectories.FirstOrDefault(nd => nd.EndsWith(ContainingDirectoryName)) ?? throw new Exception("Cannot find the directory \"galleries\".");
                neighbourDirectories = Directory.EnumerateDirectories(runningDirectory).ToList();
            }

            ConvertToGalleries(neighbourDirectories);
        }

        private static void ConvertToGalleries(IReadOnlyList<string> directories)
        {
            var galleries = new List<Gallery>();

            for (var index = 0; index < directories.Count; index++)
            {
                var directory = directories[index];
                var directoryName = Path.GetFileName(directory);

                if (directoryName == null) continue;

                Console.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} Processing {directoryName}");

                var year = new Regex("((?:(?:[1]{1}\\d{1}\\d{1}\\d{1})|(?:[2]{1}\\d{3})))(?![\\d])", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(directoryName).Groups[1].ToString();

                var gallery = new Gallery
                {
                    Id = index + 1,
                    Name = directoryName,
                    Year = int.Parse(year)
                };

                var imagePaths = Directory.GetFiles(directory)
                    .Where(path =>
                    {
                        var fileName = Path.GetFileName(path).ToLowerInvariant();

                        var isThumb = fileName.Contains("thumb");
                        var isJpg = fileName.Contains("jpg");

                        return !isThumb && isJpg;
                    })
                    .ToArray();

                Parallel.ForEach(imagePaths, new ParallelOptions {MaxDegreeOfParallelism = 4}, imagePath =>
                {
                    var image = new Image(imagePath);
                    image.GenerateThumbnail();

                    gallery.Images.Add(image);
                });

                Console.WriteLine($"{DateTime.Now.ToString(CultureInfo.InvariantCulture)} Processd {directoryName}, total {imagePaths.Length} images.");

                galleries.Add(gallery);
            }
        }
    }
}