namespace CinemaHub.Data.Models
{
    using System.Collections.Generic;

    public class Show : Media
    {
        public Show()
            : base()
        {
        }

        public virtual ICollection<Season> Seasons { get; set; }
    }
}
