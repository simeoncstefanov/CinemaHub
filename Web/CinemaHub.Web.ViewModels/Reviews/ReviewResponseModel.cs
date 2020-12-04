namespace CinemaHub.Web.ViewModels.Reviews
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutoMapper;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Mapping;

    public class ReviewResponseModel
    {
        public int ReviewCount { get; set; }

        public IEnumerable<ReviewViewModel> Reviews { get; set; }
    }
}
