namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using CinemaHub.Data.Common.Models;

    public class Genre : BaseModel<string>
    {
        public Genre()
        {
            this.Id = Guid.NewGuid().ToString();
            this.MediaGenres = new HashSet<MediaGenre>();
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ApiId { get; set; }

        public virtual ICollection<MediaGenre> MediaGenres { get; set; }
    }
}
