namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    using CinemaHub.Data.Common.Models;

    public class Season : BaseDeletableModel<string>
    {
        public Season()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [ForeignKey(nameof(Show))]
        public string ShowId { get; set; }

        public Show Show { get; set; }

        [Required]
        public int SeasonOrder { get; set; }

        [Required]
        public int EpisodeCount { get; set; }

        public string Overview { get; set; }

        public DateTime? ReleaseTime { get; set; }

        public virtual ICollection<Episode> Episodes { get; set; }
    }
}
