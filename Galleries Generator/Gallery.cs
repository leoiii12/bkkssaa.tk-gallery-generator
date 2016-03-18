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

        [JsonProperty(PropertyName = "images")]
        public List<string> Images { get; set; }
    }
}