namespace CinemaHub.Data.Seeding.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Newtonsoft.Json;

    public class ResultDTO
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("results")]
        public List<MovieDTO> Movies { get; set; }
    }
}
