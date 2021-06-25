using System;
using System.IO;
using System.Web;
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

        /// <summary>
        /// Returns all songs in the system
        /// </summary>
        /// <returns>
        /// Header: 200 (OK)
        /// Content: all songs in the db
        /// </returns>
        /// <example>
        /// GET: api/SongData/ListSongs
        /// </example>

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
                Artist = s.Artist,
                AlbumHasPic = s.AlbumHasPic,
                PicExtention = s.PicExtention

            })); ; 

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

        /// <summary>
        /// Returns all songs in the system
        /// </summary>
        /// <param name="id">Primary key of a song</param>
        /// <returns>
        /// Header 200 (OK)
        /// Content: A song in the system matching up to the song id primary key
        /// or 
        /// Header 404 (Not found)
        /// </returns>
        /// <example>
        /// GET: api/SongData//FindSong/5
        /// </example>


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
                Artist = Song.Artist,
                PicExtention = Song.PicExtention,
                AlbumHasPic = Song.AlbumHasPic
               
            };
            if (Song == null)
            {
                return NotFound();
            }

            return Ok(SongDto);
        }

        /// <summary>
        /// Updates a particular song in the system with POST data input
        /// </summary>
        /// <param name="id">Represents the song id primrary key</param>
        /// <param name="song">JSON form data of a song</param>
        /// <returns>
        /// Header: 204 (Success, No content response)
        /// or
        /// Header: 400 (Bad Request)
        /// or
        /// Header: 404 (Not found)
        /// </returns>
        /// <example>
        /// POST: api/SongData/UpdateSong/5
        /// </example>

        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
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
            // Picture update is handled by another method
            db.Entry(song).Property(s => s.AlbumHasPic).IsModified = false;
            db.Entry(song).Property(s => s.PicExtention).IsModified = false; 

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


        /// <summary>
        /// Recieves album picture data, uploads it to the web server and updates the song's HasPic option
        /// </summary>
        /// <param name="id">song id</param>
        /// <returns>status code 200 if successful</returns>
        /// <example>
        /// POST: api/SongData/UpdateSongPic/5
        /// </example>


        [HttpPost]
        
        public IHttpActionResult UploadAlbumPic(int id)
        {
            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Recieved the multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Recieved: " + numfiles);

                //Check if file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var albumpic = HttpContext.Current.Request.Files[0];
                    //Check if file is empty
                    if (albumpic.ContentLength > 0)
                    {
                        //establish valid file types 
                        var valtypes = new[] { "jpeg", "jpg", "png", "PNG" };
                        var extension = Path.GetExtension(albumpic.FileName).Substring(1);
                        //Check the extension of the file

                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/songs/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/AlbumCovers/"), fn);

                                //save file
                                albumpic.SaveAs(path);

                                //if successful then we can set fields
                             
                                haspic = true;
                                Debug.WriteLine(haspic);
                                picextension = extension;

                                //Update the song haspic and picextension fields in the db
                                Song SelectedSong = db.Songs.Find(id);
                                SelectedSong.AlbumHasPic = haspic;
                                SelectedSong.PicExtention = extension;
                                db.Entry(SelectedSong).State = EntityState.Modified;

                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("album Image was not saved succesfully");
                                Debug.WriteLine("Exception: " + ex);
                                return BadRequest();
                            }
                        }
                    }
                }
                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();
            }
        }


        /// <summary>
        /// Add a song to the system
        /// </summary>
        /// <param name="song">JSON form data of a song</param>
        /// <returns>
        /// Header: 201 (Created)
        /// Content: Song ID, Song data
        /// </returns>
        /// <example>
        /// POST: api/SongData/AddSong
        /// FORM DATA: Song JSON Object
        /// </example>


        [ResponseType(typeof(Song))]
        [Authorize]
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

        /// <summary>
        /// Delete a song from the system by it's ID
        /// </summary>
        /// <param name="id">Primary key of a song</param>
        /// <returns>
        /// Header: 200 (OK)
        /// or 
        /// Header: 404 (Not found)
        /// </returns>
        /// <example>
        /// POST: api/SongData/DeleteSong/5
        /// Form data: (empty)
        /// </example>

        [ResponseType(typeof(Song))]    
        [Authorize]
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