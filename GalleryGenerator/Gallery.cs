using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace GalleryGenerator
{
    internal class Gallery
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = "";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        [JsonProperty(PropertyName = "year")]
        public int Year { get; set; }

        [JsonProperty(PropertyName = "images")]
        public List<Image> Images { get; set; } = new List<Image>();
    }

    internal class Image
    {
        public Image(string path)
        {
            if (Path.GetExtension(path).ToLowerInvariant() != ".jpg")
                throw new Exception("Only jpg file can be processed.");

            RealPath = path;
            ThumbnailPath = Path.ChangeExtension(path, "thumb.jpg");
        }

        [JsonProperty(PropertyName = "real")]
        public string RealPath { get; set; }

        [JsonProperty(PropertyName = "thumbnail")]
        public string ThumbnailPath { get; set; }

        public void GenerateThumbnail()
        {
            if (File.Exists(ThumbnailPath)) return;

            using (var image = SixLabors.ImageSharp.Image.Load(RealPath))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Min,
                    Size = new Size(250, 250)
                }));
                image.Save(ThumbnailPath, new JpegEncoder {Quality = 85, IgnoreMetadata = true});
            }
        }

        public void DeleteThumbnail()
        {
            File.Delete(ThumbnailPath);
        }
    }
}