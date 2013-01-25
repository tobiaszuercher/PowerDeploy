using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using MovieFavorites.Contract;
using MovieFavorites.Entities;

using ServiceStack.ServiceInterface;

using TheMovieDb;

namespace MovieFavorites.Services
{
    public class MovieService : RestServiceBase<MovieDto>
    {
        public override object OnGet(MovieDto request)
        {
            using (var context = new MovieFavoritesModelContainer())
            {
                if (request.TmdbId == default(int))
                {
                    return context.Movies.ToList().Select(m => m.ConvertToDto()).ToList();
                }
                else
                {
                    return context.Movies.First(m => m.TmdbId == request.TmdbId).ConvertToDto();
                }
            }
        }

        public override object OnPost(MovieDto request)
        {
            var api = new TmdbApi("e772146a78c51bdc45f464d0910c7a60");
            WebRequest.DefaultWebProxy = new WebProxy("http://proxy.zuehlke.com:8080");
            var result = api.GetMovieInfo(request.TmdbId);

            request.Title = result.Name;

            var backdrop = result.Backdrops.Select(m => m.ImageInfo).FirstOrDefault(info => info.Type == "backdrop" && info.Size == "poster");
            var cover = result.Posters.Select(m => m.ImageInfo).FirstOrDefault(info => info.Type == "poster" && info.Size == "cover");

            var movie = new Movie() { ImdbId = result.ImdbId, Teaser = result.Overview, TmdbId = result.Id, Title = result.Name, Year = DateTime.Parse(result.Released).Year };

            if (backdrop != null)
            {
                movie.Backdrop = backdrop.Url;             
            }

            if (cover != null)
            {
                movie.Cover = cover.Url;
            }

            using (var context = new MovieFavoritesModelContainer())
            {
                context.Movies.Add(movie);
                context.SaveChanges();

                // return all elements... we are in a demo :)
                return context.Movies.ToList().Select(m => m.ConvertToDto()).ToList();
            }
        }
    }
}