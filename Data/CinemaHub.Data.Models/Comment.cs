namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using CinemaHub.Data.Common.Models;

    public class Comment : BaseDeletableModel<string>
    {
        public Comment()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Content { get; set; }

        [ForeignKey(nameof(Discussion))]
        public string DiscussionId { get; set; }

        public virtual Discussion Discussion { get; set; }

        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; }

        public virtual ApplicationUser Creator { get; set; }
    }
}
