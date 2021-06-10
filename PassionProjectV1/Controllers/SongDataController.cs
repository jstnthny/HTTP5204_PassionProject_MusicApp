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
    public class SongDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/SongData/ListSongs
        [HttpGet]
        public IEnumerable<SongDto> ListSongs()
        {
            List<Song> Songs = db.Songs.ToList();
            List<SongDto> SongDtos = new List<SongDto>();

            Songs.ForEach(s => SongDtos.Add(new SongDto()
            {
                SongId = s.SongId,
                SongName = s.SongName,
                Album = s.Album,
                AlbumUrl = s.AlbumUrl,
                Artist = s.Artist
                
            })); 

            return SongDtos;

        }
        /// <summary>
        /// Gathers information about songs related to a particular genre
        /// </summary>
        /// <param name="id">Genre ID</param>
        /// <returns>
        /// Content: All songs in the database,  that match to a particular genre id
        /// </returns>
        /// <example>
        /// GET: api/SongData/ListSongsForGenre/5
        /// </example>

        [HttpGet]
        [ResponseType(typeof(SongDto))]
        public IHttpActionResult ListSongsForGenre(int id)
        {
            //all songs that have genres which match with our ID
            List<Song> Songs = db.Songs.Where(
                s =>s.Genres.Any(
                g=>g.GenreId==id
                )).ToList();
            List<SongDto> SongDtos = new List<SongDto>();

            Songs.ForEach(s => SongDtos.Add(new SongDto()
            {
                SongId = s.SongId,
                SongName = s.SongName,
                Album = s.Album,
                AlbumUrl = s.AlbumUrl,
                Artist = s.Artist

            }));

            return Ok(SongDtos);


        }
        /// <summary>
        /// Add a new genre to a existing song
        /// </summary>
        /// <param name="songid">The Song ID primary key</param>
        /// <param name="genreid">The Genre ID primary key</param>
        /// <returns>
        /// Header: 200 (Ok)
        /// or
        /// Header: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST api/SongData/AddGenreToSong/6/5
        /// </example>
        [HttpPost]
        [Route("api/songdata/AddGenreToSong/{songid}/{genreid}")]
        public IHttpActionResult AddGenreToSong(int songid, int genreid)
        {
            Song SelectedSong = db.Songs.Include(s=>s.Genres).Where(s=>s.SongId==songid).FirstOrDefault();
            Genres SelectedGenre = db.Genres.Find(genreid);

            if(SelectedSong ==null || SelectedGenre==null)
            {
                return NotFound();
            }

            SelectedSong.Genres.Add(SelectedGenre);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Remove a genre from a song
        /// </summary>
        /// <param name="songid">The Song ID primary key</param>
        /// <param name="genreid">The Genre ID primary key</param>
        /// <returns>
        /// Header: 200 (Ok)
        /// or
        /// Header: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST api/SongData/RemoveGenreFromSong/6/5
        /// </example>
        [HttpPost]
        [Route("api/songdata/RemoveGenreFromSong/{songid}/{genreid}")]
        public IHttpActionResult RemoveGenreFromSong(int songid, int genreid)
        {
            Song SelectedSong = db.Songs.Include(s => s.Genres).Where(s => s.SongId == songid).FirstOrDefault();
            Genres SelectedGenre = db.Genres.Find(genreid);

            if (SelectedSong == null || SelectedGenre == null)
            {
                return NotFound();
            }

            SelectedSong.Genres.Remove(SelectedGenre);
            db.SaveChanges();

            return Ok();
        }

        // GET: api/SongData//FindSong/5
        [ResponseType(typeof(Song))]
        [HttpGet]
        public IHttpActionResult FindSong(int id)
        {
            Song Song = db.Songs.Find(id);
            SongDto SongDto = new SongDto()
            {
                SongId = Song.SongId,
                SongName = Song.SongName,
                Album = Song.Album,
                AlbumUrl = Song.AlbumUrl,
                Artist = Song.Artist
            };
            if (Song == null)
            {
                return NotFound();
            }

            return Ok(SongDto);
        }

        // POST: api/SongData/UpdateSong/5
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateSong(int id, Song song)
        {
            Debug.WriteLine("I have reached the update song method!");
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model State is invalid");
                return BadRequest(ModelState);
            }

            if (id != song.SongId)
            {
                Debug.WriteLine("ID mismatch");
                Debug.WriteLine("GET paramter" + id);
                Debug.WriteLine("POST paramter" + song.SongId);
                return BadRequest();
            }

            db.Entry(song).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
                {
                    Debug.WriteLine("Song not found");
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

        // POST: api/SongData/AddSong
        [ResponseType(typeof(Song))]
        [HttpPost]
        public IHttpActionResult AddSong(Song song)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Songs.Add(song);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = song.SongId }, song);
        }

        // POST: api/SongData/DeleteAnimal/5
        [ResponseType(typeof(Song))]
        [HttpPost]
        public IHttpActionResult DeleteSong(int id)
        {
            Song song = db.Songs.Find(id);
            if (song == null)
            {
                return NotFound();
            }

            db.Songs.Remove(song);
            db.SaveChanges();

            return Ok(song);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SongExists(int id)
        {
            return db.Songs.Count(e => e.SongId == id) > 0;
        }
    }
}