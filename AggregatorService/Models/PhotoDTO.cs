using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AggregatorService.Models
{
    public class PhotoDTO
    {
        [JsonPropertyName("PhotoId")]
        public int Id { get; set; }
        [JsonPropertyName("PhotoTitle")]
        public string Title { get; set; }
        [JsonPropertyName("URL")]
        public string URL { get; set; }
        [JsonPropertyName("ThumbnailUrl")]
        public string ThumbnailURL { get; set; }
    }
}
