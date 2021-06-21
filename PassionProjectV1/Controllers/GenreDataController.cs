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

        /// Returns all Genres in the system not included in a song
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

        /// Returns all Genres in the system 
        /// </summary>
        /// <returns>
        /// Header: 200 (OK)
        /// Content: A genre in the system matching up to the Genre ID primary key
        /// </returns>
        /// <param name="id">Song Primary Key</param>
        /// <example>
        /// GET: api/GenreData/FindGenre/5
        /// </example>
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

        /// <summary>
        /// Updates a particular Genre in the system with POST data input
        /// </summary>
        /// <param name="id">Repersents the Genre ID primary key</param>
        /// <param name="genres">JSON form data of a genre</param>
        /// <returns>
        /// Header: 200 (OK)
        /// </returns>
        /// <example>
        /// POST: api/UpdateGenre/5
        /// FORM Data: Genre JSON object
        /// </example>
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
        /// <summary>
        /// Add a genre to the system
        /// </summary>
        /// <param name="genres">JSON From data of a genre</param>
        /// <returns>
        /// Header: 201 (Created)
        /// Content: Genre ID, Genre Data
        /// or
        /// Header: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/GenreData/AddGenre
        /// </example>

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

        /// <summary>
        /// Deletes a genre from the system by its ID
        /// </summary>
        /// <param name="id">Primary key of the genre</param>
        /// <returns>
        /// Header: 200 (OK)
        /// or
        /// Header: 404 (Not Found)
        /// </returns>
        /// <example>
        ///  POST: api/GenreData/DeleteGenre/5
        /// </example>

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