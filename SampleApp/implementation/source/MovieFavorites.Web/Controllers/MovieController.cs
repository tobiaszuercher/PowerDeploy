using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;

using MovieFavorites.Contract;
using MovieFavorites.Web.Models;

using ServiceStack.ServiceClient.Web;

using TheMovieDb;

namespace MovieFavorites.Web.Controllers
{
    public class MovieController : Controller
    {
        public ActionResult List()
        {
            var list = new List<MovieDto>();

            using (var client = new XmlServiceClient(ConfigurationManager.AppSettings["MovieServiceUrl"]))
            {
                var response = client.Get<List<MovieDto>>("movies");

                return View(response);
            }
        }

        public ActionResult Show(int id)
        {
            using (var client = new XmlServiceClient(ConfigurationManager.AppSettings["MovieServiceUrl"]))
            {
                var response = client.Get<MovieDto>("movies/" + id);

                return View(response);
            }
        }

        public ActionResult Suggest(string term = "")
        {
            if (term == "") return Json(new List<SuggestResult>(), JsonRequestBehavior.AllowGet);

            var api = new TmdbApi("e772146a78c51bdc45f464d0910c7a60");

            try
            {
                WebRequest.DefaultWebProxy = new WebProxy("http://proxy.zuehlke.com:8080");
                var tmdbResults = api.MovieSearch(term);
                

                var jsonResults = new List<SuggestResult>();

                foreach (var tmdbMovie in tmdbResults)
                {
                    jsonResults.Add(new SuggestResult()
                    {
                        Title = tmdbMovie.Name,
                        Cast = "Tobias Zürcher, Leonardo DiCaprio, Matt Damon",
                        Thumb = (tmdbMovie.Posters.Count > 0) ? tmdbMovie.Posters.First().ImageInfo.Url : string.Empty,
                        ImdbId = tmdbMovie.ImdbId,
                        TmdbId = tmdbMovie.Id,
                    });
                }

                return Json(jsonResults, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return Json(new List<SuggestResult> (), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Add(string movieTmdbId, string searchFor)
        {
            using (var client = new XmlServiceClient(ConfigurationManager.AppSettings["MovieServiceUrl"]))
            {
                var response = client.Post<List<MovieDto>>("movies", new MovieDto { TmdbId = Convert.ToInt32(movieTmdbId) });

                return View("List", response);
            }
        }
    }
}
