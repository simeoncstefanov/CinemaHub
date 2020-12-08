namespace CinemaHub.Web.ViewModels.Discussions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CommentPageViewModel
    {
        public int CurrentPage { get; set; }

        public int DiscussionsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalResults { get; set; }

        public string MediaId { get; set; }

        public string DiscussionTitle { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }
    }
}
