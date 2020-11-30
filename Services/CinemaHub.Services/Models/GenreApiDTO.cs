namespace CinemaHub.Services.Models
{
    using Newtonsoft.Json;

    public class GenreApiDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}