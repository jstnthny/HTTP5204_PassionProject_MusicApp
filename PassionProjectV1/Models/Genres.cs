using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassionProjectV1.Models
{
    public class Genres
    {

        [Key]
        public int GenreId { get; set; }

        public string Genre { get; set; }

        //A Genre can belong to many songs

        public ICollection<Song> Songs { get; set; }
    }

    public class GenreDto
    {
        public int GenreId { get; set; }

        public string Genre { get; set; }

    }
}