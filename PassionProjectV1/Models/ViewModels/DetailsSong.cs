using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProjectV1.Models.ViewModels
{
    public class DetailsSong
    {
        public SongDto SelectedSong { get; set; }
        public IEnumerable<ReviewDto> RelatedReviews { get; set; }

        public IEnumerable<GenreDto> GenreForSong { get; set; }

        public IEnumerable<GenreDto> AvailableGenres { get; set; }
    }
}