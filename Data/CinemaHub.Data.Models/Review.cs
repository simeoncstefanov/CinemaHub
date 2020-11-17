namespace CinemaHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    using CinemaHub.Data.Common.Models;

    public class Review : BaseDeletableModel<string>
    {
        public Review()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(10000)]
        public string ReviewText { get; set; }

        [ForeignKey(nameof(Media))]
        public string MediaId { get; set; }

        public Media Media { get; set; }

        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; }

        public ApplicationUser Creator { get; set; }
    }
}
