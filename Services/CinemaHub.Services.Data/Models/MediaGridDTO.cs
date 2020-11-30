namespace CinemaHub.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using CinemaHub.Data.Models;
    using CinemaHub.Services.Mapping;

    public class MediaGridDTO
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Overview { get; set; }

        public int MovieApiId { get; set; }

        public bool IsDetailFull { get; set; }

        public string ImagePath { get; set; }

        public string MediaType { get; set; }

        public double Rating { get; set; }
    }
}
