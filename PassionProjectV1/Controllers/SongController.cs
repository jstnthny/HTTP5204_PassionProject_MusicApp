using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using PassionProjectV1.Models;
using PassionProjectV1.Models.ViewModels;
using System.Web.Script.Serialization;

namespace PassionProjectV1.Controllers
{
    public class SongController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static SongController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
            //cookies are manually set in RequestHeader
            UseCookies = false
            };        
            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44300/api/");
        }


        /// <summary>
        /// Grabs the authentication cookie sent to this controller. Allows for authenticated users to make administrative changes 
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";

            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
        // GET: Song/List
        //adding search function (string search) 
        public ActionResult List(string search)
        {
            //objective: communicate with or song data api to retrieve a list of songs
            //curl https://localhost:44300/api/songdata/listsongs

           
            string url = "songdata/listsongs";
            HttpResponseMessage response = client.GetAsync(url).Result;

           // Debug.WriteLine("The response code is ");
           // Debug.WriteLine(response.StatusCode);

          ///  IEnumerable<SongDto> songs = response.Content.ReadAsAsync<IEnumerable<SongDto>>().Result;
          //  Debug.WriteLine("Number of songs recieved");
          //  Debug.WriteLine(songs.Count());
            
            /// Adding search feature
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<SongDto> SelectedSong = response.Content.ReadAsAsync<IEnumerable<SongDto>>().Result;
                return View(search == null ? SelectedSong :
                    SelectedSong.Where(x => x.Artist.IndexOf(search, StringComparison.OrdinalIgnoreCase) >=0).ToList());
            }
            else
            {
                return RedirectToAction("Error");
            }

           /// return View(songs);
        }

        // GET: Song/Details/5
        public ActionResult Details(int id)

        {
            //objective: communicate with or song data api to retrieve one song
            //curl https://localhost:44300/api/songdata/findsong/{id}

            DetailsSong ViewModel = new DetailsSong();
        

       
            string url = "songdata/findsong/"+ id;
            HttpResponseMessage response = client.GetAsync(url).Result;

           // Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            SongDto SelectedSong = response.Content.ReadAsAsync<SongDto>().Result;
            //  Debug.WriteLine("Song recieved");
            // Debug.WriteLine(selectedsong.SongName);

            ViewModel.SelectedSong = SelectedSong; 

            //showcase information of reviews to this song 
            // send a request to gather information of reviews related to a patricular song id
            url = "reviewdata/listreviewsforsong/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ReviewDto> RelatedReviews = response.Content.ReadAsAsync<IEnumerable<ReviewDto>>().Result;


            ViewModel.RelatedReviews = RelatedReviews;

            //show associated genres with this song
            url = "genredata/listgenresforsong/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable <GenreDto> GenreForSong = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            ViewModel.GenreForSong = GenreForSong;

            url = "genredata/listgenresnotincludedinsong/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<GenreDto> AvailableGenres = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            ViewModel.AvailableGenres = AvailableGenres;



            return View(ViewModel);
        }

        //POST: Song/AddNew/{songid}{genreid}
        //Enabling authorize makes it so only registered users (admins) can make changes to data (Create new songs, update songs & delete songs) 
        [HttpPost]
        [Authorize]
   
        public ActionResult AddNew(int id, int genreId)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("Attempting to add new genre to song: " + id + " Genre: " + genreId);

            //call our api to add genre to song
            string url = "songdata/addgenretosong/" + id+"/"+ genreId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //GET: Song/Remove/{id}?genreid={genreid}
        [HttpGet]
        [Authorize]
        public ActionResult Remove(int id, int genreId)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("Attempting to remove genre from song: " + id + " Genre: " + genreId);

            //call our api to add genre to song
            string url = "songdata/removegenrefromsong/" + id + "/" + genreId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Song/New
        [Authorize]
        public ActionResult New()
       
        {
            return View();
        }

        // POST: Song/Create
        [HttpPost]
        [Authorize]
        
        public ActionResult Create(Song song)
        {
            GetApplicationCookie();//get token credentials
            Debug.WriteLine("the json payload is:");
            Debug.WriteLine(song.SongName);
            //objective: add a new song into our system using the API
            //curl -H "Content-Type:application/json" -d @song.json https://localhost:44300/api/songdata/addsong
            string url = "songdata/addsong";

            
            string jsonpayload = jss.Serialize(song);

            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if(response.IsSuccessStatusCode)
            {

                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Errors");
            }

        }

        // GET: Song/Update/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            string url = "songdata/findsong/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SongDto selectedsong = response.Content.ReadAsAsync<SongDto>().Result;
            return View(selectedsong);
        }

        // POST: Song/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Song song, HttpPostedFileBase AlbumPic)
        {
            GetApplicationCookie();//get token credentials
            string url = "songdata/updatesong/"+ id;

            string jsonpayload = jss.Serialize(song);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);


            // update request is succesful, and image data is recieved
            if (response.IsSuccessStatusCode && AlbumPic != null)
            {
                //Updating the song picture as a seperate request
                Debug.WriteLine("Calling Update Image method");
                //Send over image data
                url = "SongData/UploadAlbumPic/" + id;
                Debug.WriteLine("Recieved album picture " + AlbumPic.FileName);

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(AlbumPic.InputStream);
                requestcontent.Add(imagecontent, "AlbumPic", AlbumPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;

                return RedirectToAction("List");
            }

            else if(response.IsSuccessStatusCode)
            {
                //No image uploaded, but the update was still succesful
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Errors");
            }
        }

        // GET: Song/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
           
            string url = "songdata/findsong/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            SongDto selectedsong = response.Content.ReadAsAsync<SongDto>().Result;
            return View(selectedsong);
        }

        // POST: Song/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
            string url = "songdata/deletesong/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Errors");
            }
            
        }
    }
}
