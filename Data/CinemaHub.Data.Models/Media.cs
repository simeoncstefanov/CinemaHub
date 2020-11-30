namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Text;

    using CinemaHub.Data.Models.Enums;
    using Newtonsoft.Json;

    public abstract class Media
    {
        public Media()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Images = new HashSet<MediaImage>();
            this.Keywords = new HashSet<MediaKeyword>();
            this.Watchers = new HashSet<MediaWatcher>();
            this.Reviews = new HashSet<Review>();
            this.Discussions = new HashSet<Discussion>();
            this.Genres = new HashSet<MediaGenre>();
            this.IsDetailFull = false;
        }

        [Key]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Overview { get; set; }

        public string Language { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Required]
        public int Runtime { get; set; }

        public int Budget { get; set; }

        public int Imdb { get; set; }

        [Required]
        public int MovieApiId { get; set; }

        [Required]
        public bool IsDetailFull { get; set; }

        public string YoutubeTrailerUrl { get; set; }

        public virtual ICollection<MediaGenre> Genres { get; set; }

        public virtual ICollection<MediaImage> Images { get; set; }

        public virtual ICollection<MediaKeyword> Keywords { get; set; }

        public virtual ICollection<MediaWatcher> Watchers { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<Discussion> Discussions { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
