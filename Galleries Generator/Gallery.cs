using System.Collections.Generic;
using Newtonsoft.Json;

namespace Galleries_Generator
{
    internal class Gallery
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; } = 0;

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; } = "";

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; } = "";

        public int Year { get; set; }

        [JsonProperty(PropertyName = "images")]
        public List<Image> Images { get; set; } = new List<Image>();
    }

    internal class Image
    {
        [JsonProperty(PropertyName = "real")]
        public string RealPath { get; set; }

        [JsonProperty(PropertyName = "thumbnail")]
        public string ThumbnailPath { get; set; }
    }
}