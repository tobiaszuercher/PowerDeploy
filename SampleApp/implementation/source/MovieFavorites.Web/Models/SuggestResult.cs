namespace MovieFavorites.Web.Models
{
    public class SuggestResult
    {
        public string Title { get; set; }
        public string ImdbId { get; set; }
        public int TmdbId { get; set; }
        public string Cast { get; set; }
        public string Thumb { get; set; }
        public string Q { get; set; }
    }
}