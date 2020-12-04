namespace CinemaHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using CinemaHub.Data.Common.Models;

    public class AvatarImage : BaseDeletableModel<string>
    {
        public AvatarImage()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public string Extension { get; set; }

    }
}
