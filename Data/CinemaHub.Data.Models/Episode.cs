namespace CinemaHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using CinemaHub.Data.Common.Models;

    public class Episode : BaseDeletableModel<string>
    {
        public Episode()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public int EpisodeNumber { get; set; }

        [Required]
        public string Title { get; set; }

        public string Overview { get; set; }

        [ForeignKey(nameof(Season))]
        public string SeasonId { get; set; }

        public Season Season { get; set; }
    }
}
