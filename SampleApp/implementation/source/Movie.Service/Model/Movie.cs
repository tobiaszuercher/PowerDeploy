using System;

namespace MovieFavorites.Service.Model
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Year { get; set; }
        public string Abstract { get; set; }
        public string Cover { get; set; }
        public string Fanart { get; set; }
    }
}