namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    using CinemaHub.Data.Common.Models;

    public class MediaEdit : BaseDeletableModel<string>
    {
        public MediaEdit()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Overview { get; set; }

        public string Language { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string PosterPath { get; set; }

        [Required]
        public int Runtime { get; set; }

        public int Budget { get; set; }

        [Required]
        public bool IsDetailFull { get; set; }

        public string Genres { get; set; }

        public string KeywordsJson { get; set; }

        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; }

        [Required]
        public ApplicationUser Creator { get; set; }

        public string YoutubeTrailerUrl { get; set; }

        [ForeignKey(nameof(Media))]
        public string MediaId { get; set; }

        public Media Media { get; set; }
    }
}
