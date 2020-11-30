namespace CinemaHub.Services.Models
{
    using Newtonsoft.Json;

    public class KeywordApiDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
