namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using CinemaHub.Data.Common.Models;
    using Microsoft.EntityFrameworkCore.Storage;

    public class Discussion : BaseDeletableModel<string>, ICreatedByEntity
    {
        public Discussion()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Comments = new HashSet<Comment>();
        }

        [Required]
        public string Title { get; set; }

        [Required]
        public bool IsLocked { get; set; }

        [ForeignKey(nameof(Media))]
        public string MediaId { get; set; }

        [Required]
        public virtual Media Media { get; set; }

        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; }

        [Required]
        public virtual ApplicationUser Creator { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
