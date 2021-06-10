using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using PassionProjectV1.Models;
using System.Web.Script.Serialization;
using PassionProjectV1.Models.ViewModels;

namespace PassionProjectV1.Controllers
{
    public class GenreController : Controller
    {

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static GenreController()
        {
            client = new HttpClient();
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

        // GET: Genre/List
        public ActionResult List()
        {
            //objective: communicate with our genre data api to retrieve a list of genres
            //curl https://localhost:44300/api/reviewdata/listgenres


            string url = "genredata/listgenres";
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Debug.WriteLine("The response code is ");
            // Debug.WriteLine(response.StatusCode);

            IEnumerable<GenreDto> genres = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;



            return View(genres);
        }

        // GET: Genre/Details/5
        public ActionResult Details(int id)
        {
            DetailsGenre ViewModel = new DetailsGenre();

            //objective: communicate with our genre data api to retrieve one genre
            //curl https://localhost:44300/api/findgenres/{id}


            string url = "genredata/findgenres/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);
            Debug.WriteLine(id);

            GenreDto SelectedGenre = response.Content.ReadAsAsync<GenreDto>().Result;
            Debug.WriteLine("Genre Recieved : ");
            Debug.WriteLine(SelectedGenre.Genre);

            ViewModel.SelectedGenre = SelectedGenre;

            //show all songs that belong to this particular genre
            url = "songdata/listsongsforgenre/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<SongDto> SongGenre = response.Content.ReadAsAsync<IEnumerable<SongDto>>().Result;

            ViewModel.SongGenre = SongGenre;

            return View(ViewModel);

        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Genre/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Genre/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Genres genres)
        {
            GetApplicationCookie();
            Debug.WriteLine("the json payload is:");
            Debug.WriteLine(genres.GenreId);
            //objective: add a new song into our system using the API
            //curl -H "Content-Type:application/json" -d @genre.json https://localhost:44300/api/genredata/addgenres
            string url = "genredata/addgenres";


            string jsonpayload = jss.Serialize(genres);

            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Errors");
            }
        }

        // GET: Genre/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            string url = "genredata/findgenres/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            GenreDto selectedgenre = response.Content.ReadAsAsync<GenreDto>().Result;
            return View(selectedgenre);
        }

        // POST: Genre/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Genres genres)
        {
            GetApplicationCookie();
            string url = "genredata/updategenre/" + id;

            string jsonpayload = jss.Serialize(genres);

            Debug.WriteLine("Updated data" + jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine("This is " + content);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Errors");
            }

        }

        // GET: Genre/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            GetApplicationCookie();
            string url = "genredata/findgenres/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            GenreDto selectedgenre = response.Content.ReadAsAsync<GenreDto>().Result;
            return View(selectedgenre);
        }

        // POST: Genre/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            GetApplicationCookie();
            string url = "genredata/deletegenres/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "applicaiton/json";
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
