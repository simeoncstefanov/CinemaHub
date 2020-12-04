namespace CinemaHub.Web.ViewModels.Media
{
    using Newtonsoft.Json;

    public class KeywordViewModel
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }
}