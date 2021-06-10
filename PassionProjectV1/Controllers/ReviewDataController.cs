using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PassionProjectV1.Models;
using System.Diagnostics;

namespace PassionProjectV1.Controllers
{
    public class ReviewDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //Shows all Reviews

        [HttpGet]
        [ResponseType(typeof(ReviewDto))]
        public IHttpActionResult ListReviews()
        {
            List<Reviews> Reviews = db.Reviews.ToList();
            List<ReviewDto> ReviewDtos = new List<ReviewDto>();

            Reviews.ForEach(r => ReviewDtos.Add(new ReviewDto()
            {
                ReviewId = r.ReviewId,
                ReviewText = r.ReviewText,
                fname = r.fname,
                lname = r.lname,
                SongName = r.Song.SongName
            }));

            return Ok(ReviewDtos);
        }


        // GET: api/ReviewData/ListReviews
        /// <summary>
        /// Gathers information about all reviews for a particular song ID
        /// </summary>
        /// <returns>
        /// <param name="id">Song ID.></param>
        /// </returns>
        /// <example>
        /// GET: api/ReviewData/ListReviewsForSong/3
        /// </example>
        /// 


        [HttpGet]
        [ResponseType(typeof(ReviewDto))]
        public IHttpActionResult ListReviewsForSongs(int id)
        {
            List<Reviews> Reviews = db.Reviews.Where(r => r.SongId == id).ToList();
            List<ReviewDto> ReviewDtos = new List<ReviewDto>();

            Reviews.ForEach(r => ReviewDtos.Add(new ReviewDto()
            {
                ReviewId = r.ReviewId,
                ReviewText = r.ReviewText,
                fname = r.fname,
                lname = r.lname,
                SongName = r.Song.SongName
            }));

            return Ok(ReviewDtos);
        }

        

        [HttpGet]
        [ResponseType(typeof(ReviewDto))]
        public IEnumerable<ReviewDto> ListReviewsForSong(int id)
        {
            List<Reviews> Reviews = db.Reviews.Where(r=>r.SongId==id).ToList();
            List<ReviewDto> ReviewDtos = new List<ReviewDto>();

            Reviews.ForEach(r => ReviewDtos.Add(new ReviewDto()
            {
                ReviewId = r.ReviewId,
                ReviewText = r.ReviewText,
                fname = r.fname,
                lname = r.lname,
                SongName = r.Song.SongName
            }));

            return ReviewDtos;
        }

        
        // GET: api/ReviewData/FindReview/5
        [ResponseType(typeof(Reviews))]
        [HttpGet]
        public IHttpActionResult FindReview(int id)
        {
            Reviews Review = db.Reviews.Find(id);
            ReviewDto ReviewDto = new ReviewDto()
            {
                ReviewId = Review.ReviewId,
                ReviewText = Review.ReviewText,
                fname = Review.fname,
                lname = Review.lname,
                SongName = Review.Song.SongName

            };
            if (Review == null)
            {
                return NotFound();
            }

            return Ok(ReviewDto);
        }

        // POST: api/ReviewData/UpdateReview/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateReview(int id, Reviews reviews)
        {
            Debug.WriteLine("I have reached the update genre method!");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != reviews.ReviewId)
            {
                Debug.WriteLine("ID mismatch");
                Debug.WriteLine("GET paramter" + id);
                Debug.WriteLine("POST paramter" + reviews.ReviewId);
                return BadRequest();
            }

            db.Entry(reviews).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            Debug.WriteLine("None of the conditions trigger");
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ReviewData/AddReview
        [ResponseType(typeof(Reviews))]
        [HttpPost]
        public IHttpActionResult AddReviews(Reviews reviews)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Reviews.Add(reviews);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = reviews.ReviewId }, reviews);
        }

        // POST: api/ReviewData/DeleteReview/5
        [ResponseType(typeof(Reviews))]
        [HttpPost]
        public IHttpActionResult DeleteReviews(int id)
        {
            Reviews reviews = db.Reviews.Find(id);
            if (reviews == null)
            {
                return NotFound();
            }

            db.Reviews.Remove(reviews);
            db.SaveChanges();

            return Ok(reviews);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReviewsExists(int id)
        {
            return db.Reviews.Count(e => e.ReviewId == id) > 0;
        }
    }
}