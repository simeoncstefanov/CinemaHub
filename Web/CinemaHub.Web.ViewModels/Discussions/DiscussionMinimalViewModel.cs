namespace CinemaHub.Web.ViewModels.Discussions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using CinemaHub.Data.Models;
    using CinemaHub.Services.Mapping;

    public class DiscussionMinimalViewModel : IMapFrom<Discussion>
    {
        public string Title { get; set; }

        public bool IsLocked { get; set; }
    }
}
