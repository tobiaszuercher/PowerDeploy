using System;
using System.Collections.Generic;
using System.Linq;

using MovieFavorites.Service.Model;

namespace MovieFavorites.Service
{
    /// <summary>
    /// In-memory repository, so we can run the TODO app without any external dependencies
    /// Registered in Funq as a singleton, auto injected on every request
    /// </summary>
    public class MovieRepository
    {
        private readonly List<Movie> _movies = new List<Movie>();

        public MovieRepository()
        {
            _movies.Add(new Movie()
            {
                Abstract = "blabla",
                Cover = "cover.png",
                Fanart = "fanart.png",
                Title = "Taken 2",
                Year = new DateTime(2012, 1, 1)
            });
        }

        public void DeleteById(long id)
        {
            _movies.RemoveAll(x => x.Id == id);
        }

        public List<Movie> GetAll()
        {
            return _movies;
        }

        public Movie GetById(long id)
        {
            return _movies.FirstOrDefault(x => x.Id == id);
        }

        public Movie Store(Movie movie)
        {
            if (movie.Id == default(long))
            {
                movie.Id = _movies.Count == 0 ? 1 : _movies.Max(x => x.Id) + 1;
            }
            else
            {
                for (var i = 0; i < _movies.Count; i++)
                {
                    if (_movies[i].Id != movie.Id)
                    {
                        continue;
                    }

                    _movies[i] = movie;

                    return movie;
                }
            }

            _movies.Add(movie);

            return movie;
        }
    }
}