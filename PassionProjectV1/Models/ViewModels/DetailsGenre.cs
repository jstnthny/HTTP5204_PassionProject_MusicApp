using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProjectV1.Models.ViewModels
{
    public class DetailsGenre
    {
      
            public GenreDto SelectedGenre { get; set; }

            public IEnumerable<SongDto> SongGenre { get; set; }

       

      
    }
}