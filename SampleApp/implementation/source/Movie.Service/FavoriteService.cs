using System;

using MovieFavorites.Service.Model;

using ServiceStack.ServiceInterface;

namespace MovieFavorites.Service
{
    public class FavoriteService : RestServiceBase<Movie>
    {
        public MovieRepository Repository { get; set; }

        public override object OnGet(Movie request)
        {
            if (request.Id == default(long))
            {
                return Repository.GetAll();
            }

            return Repository.GetById(request.Id);
        }
    }
}