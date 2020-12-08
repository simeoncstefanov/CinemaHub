namespace CinemaHub.Web.ViewModels.Discussions
{
    using System.Collections.Generic;

    public class DiscussionPageViewModel
    {
        public int CurrentPage { get; set; }

        public int DiscussionsPerPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalResults { get; set; }

        public string MediaId { get; set; }

        public IEnumerable<DiscussionViewModel> Discussions { get; set; }
    }
}
