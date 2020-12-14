namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using CinemaHub.Data.Common;
    using CinemaHub.Data.Common.Models;


    public class Comment : BaseDeletableModel<string>, ICreatedByEntity
    {
        public Comment()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public string Content { get; set; }

        [ForeignKey(nameof(Discussion))]
        public string DiscussionId { get; set; }

        public Discussion Discussion { get; set; }

        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; }

        public ApplicationUser Creator { get; set; }

        public virtual ICollection<CommentVote> CommentVotes { get; set; }

    }
}
