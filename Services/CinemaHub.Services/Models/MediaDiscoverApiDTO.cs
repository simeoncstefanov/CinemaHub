namespace CinemaHub.Services.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    class MediaDiscoverApiDTO
    {
        [JsonProperty("results")]
        public IEnumerable<MediaIdDTO> Results { get; set; }
    }
}
