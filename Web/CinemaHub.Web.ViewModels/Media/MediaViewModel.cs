namespace CinemaHub.Web.ViewModels.Media
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class MediaViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Overview { get; set; }

        public string Language { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int RuntimeSeconds { get; set; }

        public int Budget { get; set; }

        public int Imdb { get; set; }

        public int MovieApiId { get; set; }

        public bool IsDetailFull { get; set; }
    }
}
