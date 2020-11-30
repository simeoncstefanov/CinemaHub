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
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(10000)]
        public string ReviewText { get; set; }

        [ForeignKey(nameof(Rating))]
        public string RatingId { get; set; }

        public Rating Rating { get; set; }
    }
}
