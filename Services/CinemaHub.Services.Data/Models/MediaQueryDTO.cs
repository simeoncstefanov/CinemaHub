namespace CinemaHub.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class MediaQueryDTO
    {
        public string MediaType { get; set; }

        public string SearchQuery { get; set; }

        public string Keywords { get; set; }

        public string SortType { get; set; }

        public int Page { get; set; }

        public string Genres { get; set; }

        public int ElementsPerPage { get; set; }
    }
}
