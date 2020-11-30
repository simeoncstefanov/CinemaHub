namespace CinemaHub.Services.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class KeywordListApiDTO
    {
        [JsonProperty("keywords")]
        public List<KeywordApiDTO> KeywordsList { get; set; }
    }
}