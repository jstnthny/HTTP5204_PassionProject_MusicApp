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
    public class GenreDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all Genres in the system
        /// </summary>
        /// <returns>
        /// Header: 200 (OK)
        /// Content: all genres in the database
        /// </returns>
        /// <example>
        /// GET: api/GenreData/ListSongs
        /// </example>

        [HttpGet]
        [ResponseType(typeof(GenreDto))]
        public IHttpActionResult ListGenre()
        {
            List<Genres> Genres = db.Genres.ToList();
            List<GenreDto> GenreDtos = new List<GenreDto>();

            Genres.ForEach(g => GenreDtos.Add(new GenreDto()
                {
                GenreId = g.GenreId,
                Genre = g.Genre

            }));

            return Ok(GenreDtos);
        }


        /// <summary>
        /// Returns all Genres in the system
        /// </summary>
        /// <returns>
        /// Header: 200 (OK)
        /// Content: all genres in the database, associated with the song that belong in that genre
        /// </returns>
        /// <param name="id">Song Primary Key</param>
        /// <example>
        /// GET: api/GenreData/ListGenresForSong/3
        /// </example>
        /// 
        [HttpGet]
        [ResponseType(typeof(GenreDto))]
        public IHttpActionResult ListGenresForSong(int id)
        {
            List<Genres> Genres = db.Genres.Where(
                g=>g.Songs.Any(
                    s=>s.SongId==id)
                ).ToList();
            List<GenreDto> GenreDtos = new List<GenreDto>();

            Genres.ForEach(g => GenreDtos.Add(new GenreDto()
            {
                GenreId = g.GenreId,
                Genre = g.Genre

            }));

            return Ok(GenreDtos);
        }

        /// Returns all Genres in the system
        /// </summary>
        /// <returns>
        /// Header: 200 (OK)
        /// Content: all genres in the database, not associated with a particular song
        /// </returns>
        /// <param name="id">Song Primary Key</param>
        /// <example>
        /// GET: api/GenreData/ListGenresNotIncludedInSong/3
        /// </example>
        /// 
        [HttpGet]
        [ResponseType(typeof(GenreDto))]
        public IHttpActionResult ListGenresNotIncludedInSong(int id)
        {
            List<Genres> Genres = db.Genres.Where(
                g => !g.Songs.Any(
                    s => s.SongId == id)
                ).ToList();
            List<GenreDto> GenreDtos = new List<GenreDto>();

            Genres.ForEach(g => GenreDtos.Add(new GenreDto()
            {
                GenreId = g.GenreId,
                Genre = g.Genre

            }));

            return Ok(GenreDtos);
        }

        /*
      
        */
        // GET: api/GenreData/FindGenre/5
        [ResponseType(typeof(Genres))]
        [HttpGet]
        public IHttpActionResult FindGenre(int id)
        {
            Genres Genre = db.Genres.Find(id);
            GenreDto GenreDto = new GenreDto()
            {
                GenreId = Genre.GenreId,
                Genre = Genre.Genre

            };
            if (Genre == null)
            {
                return NotFound();
            }

            return Ok(GenreDto);
        }

        // POST: api/UpdateGenre/5
        [ResponseType(typeof(void))]
        [Authorize]
        [HttpPost]
        public IHttpActionResult UpdateGenre(int id, Genres genres)
        {
            Debug.WriteLine("I have reached the update genre method!");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != genres.GenreId)
            {
                Debug.WriteLine("ID mismatch");
                Debug.WriteLine("GET paramter" + id);
                Debug.WriteLine("POST paramter" + genres.GenreId);
                return BadRequest();
            }

            db.Entry(genres).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenresExists(id))
                {
                    Debug.WriteLine("Genre not found");
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

        // POST: api/GenreData/AddGenre
        [ResponseType(typeof(Genres))]
        [Authorize]
        [HttpPost]
        public IHttpActionResult AddGenre(Genres genres)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Genres.Add(genres);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = genres.GenreId }, genres);
        }

        // POST: api/GenreData/DeleteGenre/5
        [ResponseType(typeof(Genres))]
        [Authorize]
        [HttpPost]
        public IHttpActionResult DeleteGenre(int id)
        {
            Genres genres = db.Genres.Find(id);
            if (genres == null)
            {
                return NotFound();
            }

            db.Genres.Remove(genres);
            db.SaveChanges();

            return Ok(genres);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GenresExists(int id)
        {
            return db.Genres.Count(e => e.GenreId == id) > 0;
        }
    }
}