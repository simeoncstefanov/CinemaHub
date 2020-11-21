namespace CinemaHub.Web.ViewModels.Media
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using CinemaHub.Services.Data.Models;

    public class MediaGridViewModel
    {
        public IEnumerable<MediaGridDTO> Medias { get; set; }

        public int ResultsFound { get; set; }

        public int ElementsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public string MediaType { get; set; }
    }
}
