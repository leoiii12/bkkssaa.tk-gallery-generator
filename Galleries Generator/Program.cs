using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Galleries_Generator
{
    internal class Program
    {
        private const string GALLERIES_CONTAINER_NAME = "galleries";

        private static void Main(string[] args)
        {
            var currenctDirectoryPath = Directory.GetCurrentDirectory();
            var subDirectoriesPaths = Directory.EnumerateDirectories(currenctDirectoryPath).ToList();
            var galleries = new List<Gallery>();

            for (var index = 1; index <= subDirectoriesPaths.Count; index++)
            {
                var subDirectoriesPath = subDirectoriesPaths[index - 1];
                var subDirectoryLength = subDirectoriesPath.Length;

                string currentDirectoryName;
                try
                {
                    currentDirectoryName = subDirectoriesPath.Substring(subDirectoriesPath.IndexOf(GALLERIES_CONTAINER_NAME, StringComparison.Ordinal) + GALLERIES_CONTAINER_NAME.Length + 1);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Please run in the correct folder !");
                    break;
                }

                var images = Directory
                    .GetFiles(subDirectoriesPath)
                    .Select(path => currentDirectoryName + path.Substring(subDirectoryLength))
                    .ToList();

                var gallery = new Gallery
                {
                    Id = index,
                    Name = currentDirectoryName,
                    Images = images
                };

                galleries.Add(gallery);
            }

            var output = JsonConvert.SerializeObject(galleries);
            File.WriteAllText(currenctDirectoryPath + "\\galleries.json", output);

            Console.WriteLine("Finished ! Press any key to exit ...");
            Console.ReadLine();
        }
    }
}