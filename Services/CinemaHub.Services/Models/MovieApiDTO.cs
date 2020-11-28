namespace CinemaHub.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using Newtonsoft.Json;

    public class MovieApiDTO
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("original_language")]
        public string Language { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("id")]
        public int MovieApiId { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("genre_ids")]
        public List<int> GenresIds { get; set; }

        public List<string> KeywordsList { get; set; }

        public string YoutubeTrailerUrl { get; set; }

        public string BackdropApiName { get; set; }

        public string PosterApiName { get; set; }


    }
}
