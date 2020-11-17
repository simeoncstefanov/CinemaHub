namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    using CinemaHub.Data.Common.Models;

    public class Keyword : BaseDeletableModel<string>
    {
        public Keyword()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Media = new HashSet<MediaKeyword>();
        }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<MediaKeyword> Media { get; set; }
    }
}
