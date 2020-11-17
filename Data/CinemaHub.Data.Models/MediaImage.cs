namespace CinemaHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using CinemaHub.Data.Common.Models;
    using CinemaHub.Data.Models.Enums;

    public class MediaImage : BaseDeletableModel<string>
    {
        public MediaImage()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Path { get; set; }

        [Required]
        public string Extension { get; set; }

        [Required]
        public ImageType ImageType { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [ForeignKey(nameof(Media))]
        public string MediaId { get; set; }

        public Media Media { get; set; }
    }
}
