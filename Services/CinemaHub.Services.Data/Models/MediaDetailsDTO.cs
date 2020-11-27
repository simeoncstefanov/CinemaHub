namespace CinemaHub.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class MediaDetailsDTO
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Overview { get; set; }

        public string Language { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int Runtime { get; set; }

        public int Budget { get; set; }

        public int Imdb { get; set; }

        public int MovieApiId { get; set; }

        public bool IsDetailFull { get; set; }

        public string YoutubeLink { get; set; }

        public string PosterPath { get; set; }

        public string MediaType { get; set; }

        public List<string> Genres { get; set; }

        public List<string> Keywords { get; set; }
    }
}
