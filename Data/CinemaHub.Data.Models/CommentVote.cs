namespace CinemaHub.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using CinemaHub.Data.Common.Models;

    public class CommentVote : BaseModel<string>
    {
        public CommentVote()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Required]
        public bool IsUpvote { get; set; }

        [ForeignKey(nameof(Comment))]
        public string CommentId { get; set; }

        [Required]
        public Comment Comment { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }
    }
}