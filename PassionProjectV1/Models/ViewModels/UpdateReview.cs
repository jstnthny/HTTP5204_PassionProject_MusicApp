using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PassionProjectV1.Models.ViewModels
{
    public class UpdateReview
    {
        //This viewmodel is a class which stores information that we need to present to /Review/Update/{}

        //the existing review information

        public ReviewDto SelectedReview { get; set; }

        //also like to include all songs to choose from when updating review

        public IEnumerable<SongDto> SongOptions { get; set; }
    }
}