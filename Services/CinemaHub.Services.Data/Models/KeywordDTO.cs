namespace CinemaHub.Services.Data.Models
{
    using Newtonsoft.Json;

    public class KeywordDTO
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
