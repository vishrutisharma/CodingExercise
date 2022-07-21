using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AggregatorService.Models
{
    public class GetAllPhotosResponseModel
    {
        public List<User> Users { get; set; }
    }

    public class User
    {
        [JsonPropertyName("UserId")]
        public int Id { get; set; }
        [JsonPropertyName("Albums")]
        public List<AlbumDTO> Albums { get; set; }
    }
}
