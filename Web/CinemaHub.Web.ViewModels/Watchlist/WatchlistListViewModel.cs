namespace CinemaHub.Web.ViewModels.Watchlist
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Mapping;

    public class WatchlistListViewModel : IMapFrom<MediaWatcher>
    {
         public string UserId { get; set; }

         public string MediaId { get; set; }

         public WatchType WatchType { get; set; }
    }
}
