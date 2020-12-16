namespace CinemaHub.Services.Models
{
    using Newtonsoft.Json;

    public class MediaIdDTO
    {
        [JsonProperty("id")]
        public int MediaApiId { get; set; }
    }
}
