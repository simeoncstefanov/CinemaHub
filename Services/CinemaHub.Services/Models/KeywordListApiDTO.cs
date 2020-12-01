namespace CinemaHub.Services.Models
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class KeywordListApiDTO
    {
        [JsonProperty("keywords")]
        public List<KeywordApiDTO> KeywordsList { get; set; }

        [JsonProperty("results")]
        public List<KeywordApiDTO> KeywordsList2
        {
            set
            {
                this.KeywordsList = value;
            }
        }
    }
}