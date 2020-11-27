namespace CinemaHub.Services.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class MediaResultDTO
    {
        public int ResultCount { get; set; }

        public int ResultsPerPage { get; set; }

        public int Pages { get => ((this.ResultCount - 1) / this.ResultsPerPage) + 1; } // round up the pages if there are leftover results

        public IEnumerable<MediaGridDTO> Results { get; set; }

        public string ResultType { get; set; }

        public int CurrentPage { get; set; }
    }
}
