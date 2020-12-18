namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    using CinemaHub.Data.Common.Models;

    public class Rating : BaseModel<string>, ICreatedByEntity
    {
        public Rating()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        [Range(1, 10)]
        public byte Score { get; set; }

        public bool IsTrained { get; set; } = false; // Is it added to the recommendation model.

        [ForeignKey(nameof(Media))]
        public string MediaId { get; set; }

        public Media Media { get; set; }

        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; }

        public ApplicationUser Creator { get; set; }

        public Review Review { get; set; }
    }
}
