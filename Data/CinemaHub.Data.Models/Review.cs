namespace CinemaHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    using CinemaHub.Data.Common.Models;

    public class Review : BaseDeletableModel<string>, ICreatedByEntity
    {
        [Key]
        [ForeignKey(nameof(Rating))]
        public string RatingId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(10000)]
        public string ReviewText { get; set; }

        public Rating Rating { get; set; }

        [ForeignKey(nameof(Media))]
        public string MediaId { get; set; }

        public Media Media { get; set; }

        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; }

        public ApplicationUser Creator { get; set; }
    }
}
