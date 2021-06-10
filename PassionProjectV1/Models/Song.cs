using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassionProjectV1.Models
{
    public class Song
    {
        [Key]
        public int SongId { get; set; }
        public string SongName { get; set; }

        public string Album { get; set; }

        public string AlbumUrl { get; set; }

        public string Artist { get; set; }

        //A song can have many genres
        public ICollection<Genres> Genres { get; set; }

 
    }

    public class SongDto
    {
        public int SongId { get; set; }
        public string SongName { get; set; }

        public string Album { get; set; }

        public string AlbumUrl { get; set; }

        public string Artist { get; set; }

        public string SongGenre { get; set; }
    }
}