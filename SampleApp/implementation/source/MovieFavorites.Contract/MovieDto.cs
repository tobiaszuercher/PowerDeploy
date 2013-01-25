using ServiceStack.ServiceHost;

namespace MovieFavorites.Contract
{
    [Route("/movies")]
    [Route("/movies/{TmdbId}")]
    public class MovieDto
    {
        public string Title { get; set; }

        public int Year { get; set; }

        public string Teaser { get; set; }

        public int TmdbId { get; set; }

        public string Cover { get; set; }

        public string Fanart { get; set; }
    }
}