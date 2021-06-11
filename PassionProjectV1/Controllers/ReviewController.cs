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
    public class ReviewController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ReviewController()
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
        // GET: Review/List
        public ActionResult List()
        {
            //objective: communicate with our review data api to retrieve a list of reviews
            //curl https://localhost:44300/api/reviewdata/listreviews


            string url = "reviewdata/listreviews";
            HttpResponseMessage response = client.GetAsync(url).Result;

             Debug.WriteLine("The response code is ");
             Debug.WriteLine(response.StatusCode);

            IEnumerable<ReviewDto> reviews = response.Content.ReadAsAsync<IEnumerable<ReviewDto>>().Result;
          


            return View(reviews);
        }

        // GET: Review/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our review data api to retrieve one reciew
            //curl https://localhost:44300/api/reviewdata/findreview/{id}


            string url = "reviewdata/findreview/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            ReviewDto selectedreview = response.Content.ReadAsAsync<ReviewDto>().Result;
           ;


            return View(selectedreview);
        }

        public ActionResult Error()
        {
            return View();
        }


        //Users will be able to add new reviews without an account 
        // GET: Review/New
      
        public ActionResult New()
        {
            //information about all songs in the system
            //GET api/songdata/listsongs

            string url = "songdata/listsongs";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<SongDto> songsoptions = response.Content.ReadAsAsync<IEnumerable<SongDto>>().Result;

            return View(songsoptions);
        }

        // POST: Review/Create
        [HttpPost]
    
        public ActionResult Create(Reviews review)
        {
            GetApplicationCookie(); 
            Debug.WriteLine("the json payload is:");
            Debug.WriteLine(review.ReviewId);
            //objective: add a new song into our system using the API
            //curl -H "Content-Type:application/json" -d @review.json https://localhost:44300/api/reviewdata/addreviews
            string url = "reviewdata/addreviews";


            string jsonpayload = jss.Serialize(review);

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

        // GET: Review/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            UpdateReview ViewModel = new UpdateReview();

            //the existing review information
            string url = "reviewdata/findreview/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ReviewDto SelectedReview = response.Content.ReadAsAsync<ReviewDto>().Result;
            ViewModel.SelectedReview = SelectedReview;

            //also like to include all songs to choose from when updating review
            url = "songdata/listsongs/";
            response = client.GetAsync(url).Result;
            IEnumerable<SongDto> SongOptions = response.Content.ReadAsAsync<IEnumerable<SongDto>>().Result;

            ViewModel.SongOptions = SongOptions;


            return View(ViewModel);
        }

        // POST: Review/Edit/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Reviews review)
        {
            GetApplicationCookie();
            string url = "reviewdata/updatereview/" + id;

            string jsonpayload = jss.Serialize(review);

            Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
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

        // GET: Review/Delete/5
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "reviewdata/findreview/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ReviewDto selectedreview = response.Content.ReadAsAsync<ReviewDto>().Result;
            return View(selectedreview);
        }

        // POST: Review/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id, FormCollection collection)
        {
            GetApplicationCookie();
            string url = "reviewdata/deletereviews/" + id;
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
