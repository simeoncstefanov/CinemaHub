namespace CinemaHub.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text;

    public class MediaKeyword
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Keyword))]
        public string KeywordId { get; set; }

        public Keyword Keyword { get; set; }

        [ForeignKey(nameof(Media))]
        public string MediaId { get; set; }

        public Media Media { get; set; }
    }
}
