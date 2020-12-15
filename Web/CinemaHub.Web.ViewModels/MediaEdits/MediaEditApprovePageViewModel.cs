using CinemaHub.Web.ViewModels.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHub.Web.ViewModels.MediaEdits
{
    public class MediaEditApprovePageViewModel
    {
        public int TotalResults { get; set; }

        public int CurrentPage { get; set; }

        public int ElementsPerPage { get; set; }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(this.TotalResults, this.ElementsPerPage));

        public List<Tuple<MediaDetailsInputModel, MediaDetailsInputModel>> ComparisonEdited { get; set; }
    }
}
