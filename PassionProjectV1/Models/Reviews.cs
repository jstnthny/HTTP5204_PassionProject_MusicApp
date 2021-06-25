using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassionProjectV1.Models
{
    public class Reviews
    {
        [Key]
        public int ReviewId { get; set; }

        public string ReviewText { get; set; }

        public string fname { get; set; }

        public string lname { get; set; }

        // A review belongs to one song
        // A Song can have many reviews
        [ForeignKey("Song")]
        public int SongId { get; set; }
        public virtual Song Song { get; set; }
    }

    public class ReviewDto
    {
        public int ReviewId { get; set; }

        [DisplayName("Review Text")]
        public string ReviewText { get; set; }
        [DisplayName("First Name")]
        public string fname { get; set; }
        [DisplayName("Last Name")]
        public string lname { get; set; }

        public int SongId { get; set; }
        [DisplayName("Song Name")]
        public string SongName { get; set; }
    }
}