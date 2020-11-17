namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    using CinemaHub.Data.Common.Models;

    public class MediaGenre : BaseModel<string>
    {
        public MediaGenre()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [ForeignKey(nameof(Genre))]
        public string GenreId { get; set; }

        public Genre Genre { get; set; }

        [ForeignKey(nameof(Media))]
        public string MediaId { get; set; }

        public Media Media { get; set; }
    }
}
