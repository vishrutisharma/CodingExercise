using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AggregatorService.Models
{
    public class AlbumDTO
    {
        [JsonPropertyName("AlbumId")]
        public int Id { get; set; }
        [JsonPropertyName("AlbumTitle")]
        public string Title { get; set; }
        [JsonPropertyName("Photos")]
        public List<PhotoDTO> Photos { get; set; }

    }
}
